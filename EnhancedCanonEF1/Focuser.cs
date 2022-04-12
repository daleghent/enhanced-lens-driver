#region "copyright"

/*
    Copyright Dale Ghent <daleg@elemental.org>
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/
*/

#endregion "copyright"

using ASCOM.DeviceInterface;
using ASCOM.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ASCOM.EnhancedCanonEF {

    [Guid("41EED37F-26F2-483D-95DA-D8A23E4B4944")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Focuser : IFocuserV2 {
        internal static string driverID = "ASCOM.EnhancedCanonEF.Focuser";

        internal static string apertureDefault = "0";
        internal static string apertureProfileName = "Aperture";
        internal static string comPortDefault = "COM1";
        internal static string comPortProfileName = "COM Port";
        internal static string LensModelDefault = "";
        internal static string LensModelProfileName = "LensModel";
        internal static string traceStateDefault = "false";
        internal static string traceStateProfileName = "Trace Level";

        private static string driverDescription = "Enhanced ASCOM Lens Driver (Focuser 1)";
        private int focuserPosition = 5000;

        internal static bool traceState;
        internal static string LensModel;
        internal static string comPort;
        private Serial serialPort;
        private TraceLogger tl;
        internal static int Aperture;
        internal static List<string> FocalRatioList = new List<string>();
        private readonly object apertureLock = new object();
        private readonly object serialLock = new object();
        private bool connected = false;

        public Focuser() {
            ReadProfile();
            tl.LogMessage("Focuser", "Completed initialization");
        }

        public bool Absolute => true;

        public bool Connected {
            get => connected;
            set {
                tl.LogMessage("Connected Set", value.ToString());

                if (value) {
                    if (serialPort == null || !serialPort.Connected) {
                        if (string.IsNullOrEmpty(comPort)) {
                            throw new InvalidOperationException("No COM port has been selected");
                        }

                        if (string.IsNullOrEmpty(LensModel)) {
                            throw new InvalidOperationException("No lens has been configured");
                        }

                        try {
                            serialPort = new Serial {
                                PortName = comPort,
                                Speed = SerialSpeed.ps38400,
                                DTREnable = true,
                            };

                            if (!serialPort.AvailableCOMPorts.Contains(comPort)) {
                                serialPort.Dispose();
                                throw new InvalidOperationException($"{comPort} does not exist");
                            }

                            serialPort.Connected = true;
                        } catch (Exception ex) {
                            throw new InvalidOperationException(ex.Message.ToString(), (Exception)(object)ex);
                        }

                        connected = true;
                    }
                } else if (serialPort != null && serialPort.Connected) {
                    connected = false;
                    serialPort.Connected = false;
                    serialPort.Dispose();
                }

                tl.LogMessage("Connected Set", "Finished");
            }
        }

        public string Description => driverDescription;

        public string DriverInfo {
            get {
                var descriptionAttr = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).Cast<AssemblyDescriptionAttribute>().FirstOrDefault();
                return descriptionAttr != null ? descriptionAttr.Description : Name;
            }
        }

        public string DriverVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public short InterfaceVersion => 2;

        public bool IsMoving => false;

        public bool Link {
            get {
                CheckConnection();
                tl.LogMessage("Link Get", Connected.ToString());
                return Connected;
            }
            set {
                CheckConnection();
                tl.LogMessage("Link Set", value.ToString());
                Connected = value;
            }
        }

        public int MaxIncrement {
            get {
                CheckConnection();
                tl.LogMessage("MaxIncrement Get", 10000.ToString());
                return 10000;
            }
        }

        public int MaxStep {
            get {
                CheckConnection();
                tl.LogMessage("MaxStep Get", 10000.ToString());
                return 10000;
            }
        }

        public string Name => driverDescription;

        public int Position {
            get {
                CheckConnection();
                tl.LogMessage("Position Get", focuserPosition.ToString());

                SetApertureInternal(Aperture, false);
                return focuserPosition;
            }
        }

        public double StepSize => throw new PropertyNotImplementedException("StepSize", accessorSet: false);

        public ArrayList SupportedActions {
            get {
                CheckConnection();

                var actions = new ArrayList() {
                    "GetApertureIndex",
                    "SetApertureIndex",
                    "GetFocalRatioList",
                    "GetLensModel",
                };

                tl.LogMessage("SupportedActions Get", string.Join(", ", actions.Cast<string>().ToArray()));
                return actions;
            }
        }

        public bool TempComp {
            get {
                CheckConnection();
                return false;
            }
            set => throw new PropertyNotImplementedException("TempComp", accessorSet: true);
        }

        public bool TempCompAvailable {
            get {
                CheckConnection();
                return false;
            }
        }

        public double Temperature => throw new PropertyNotImplementedException("Temperature", accessorSet: false);

        [ComRegisterFunction]
        public static void RegisterASCOM(Type t) {
            RegUnregASCOM(bRegister: true);
        }

        [ComUnregisterFunction]
        public static void UnregisterASCOM(Type t) {
            RegUnregASCOM(bRegister: false);
        }

        public string Action(string actionName, string actionParameters) {
            CheckConnection();

            tl.LogMessage("Action", $"{actionName} {actionParameters}");
            var response = string.Empty;

            // ASCOM spec requires that the Action be accepted regardless of case, so
            // normalize the supplied Action name to lower-case
            var actionNameLC = actionName.ToLower(System.Globalization.CultureInfo.InvariantCulture);

            switch (actionNameLC) {
                case "getlensmodel":
                    response = LensModel;
                    break;

                case "setapertureindex":
                    SetApertureInternal(int.Parse(actionParameters), true);
                    break;

                case "getapertureindex":
                    response = Aperture.ToString();
                    break;

                case "getfocalratiolist":
                    response = string.Join(":", FocalRatioList);
                    break;

                default:
                    throw new ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
            }

            return response;
        }

        public void CommandBlind(string command, bool raw) {
            CheckConnection();
            tl.LogMessage("CommandBlind", $"Command: {command}, Raw: {raw}");

            lock (serialLock) {
                serialPort.ClearBuffers();
                serialPort.Transmit(command);
            }
        }

        public bool CommandBool(string command, bool raw) {
            throw new MethodNotImplementedException("CommandBool");
        }

        public string CommandString(string command, bool raw) {
            CheckConnection();
            tl.LogMessage("CommandString", $"Command: {command}, Raw: {raw}");

            string text = string.Empty;

            lock (serialLock) {
                serialPort.ClearBuffers();
                serialPort.Transmit(command);
                text = serialPort.ReceiveTerminated("#");
            }

            return text.Remove(text.Length - 1);
        }

        public void Dispose() {
            tl.Enabled = false;
            tl.Dispose();
            tl = null;
        }

        public void Halt() {
            throw new MethodNotImplementedException("Halt");
        }

        public void Move(int position) {
            CheckConnection();
            tl.LogMessage("Move", position.ToString());

            CommandBlind($"M{position}#", raw: true);
            string s = CommandString("P#", raw: false);
            focuserPosition = int.Parse(s);
        }

        public void SetupDialog() {
            if (connected) {
                MessageBox.Show("The driver is already connected to the focuser.\nDisconnect to change any configuration parameters.");
                return;
            }

            using SetupDialogForm setupDialogForm = new SetupDialogForm();
            DialogResult dialogResult = setupDialogForm.ShowDialog();

            if (dialogResult == DialogResult.OK) {
                WriteProfile();
            }
        }

        internal void ReadProfile() {
            using Profile profile = new Profile();
            profile.DeviceType = "Focuser";
            traceState = Convert.ToBoolean(profile.GetValue(driverID, traceStateProfileName, string.Empty, traceStateDefault));
            comPort = profile.GetValue(driverID, comPortProfileName, string.Empty, comPortDefault);
            Aperture = Convert.ToInt32(profile.GetValue(driverID, apertureProfileName, string.Empty, apertureDefault));
            LensModel = profile.GetValue(driverID, LensModelProfileName, string.Empty, LensModelDefault);
            FocalRatioList = Utility.GetFocalRatios(LensModel);

            tl = new TraceLogger("", "EnhancedCanonEF") {
                Enabled = traceState
            };

            tl.LogMessage("ReadProfile", comPort);
            tl.LogMessage("ReadProfile", Aperture.ToString());
            tl.LogMessage("ReadProfile", LensModel);
            tl.LogMessage("ReadProfile", string.Join(", ", FocalRatioList));
        }

        internal void WriteProfile() {
            using Profile profile = new Profile();
            profile.DeviceType = "Focuser";
            profile.WriteValue(driverID, traceStateProfileName, traceState.ToString());
            profile.WriteValue(driverID, comPortProfileName, comPort.ToString());
            profile.WriteValue(driverID, apertureProfileName, Aperture.ToString());
            profile.WriteValue(driverID, LensModelProfileName, LensModel.ToString());
        }

        private static void RegUnregASCOM(bool bRegister) {
            using Profile profile = new Profile();
            profile.DeviceType = "Focuser";

            if (bRegister) {
                profile.Register(driverID, driverDescription);
            } else {
                profile.Unregister(driverID);
            }
        }

        private bool CheckConnection() {
            if (!connected || !serialPort.Connected) {
                throw new NotConnectedException("Driver is not connected to the hardware.");
            }

            return true;
        }

        private void SetApertureInternal(int aperture, bool withReset) {
            tl.LogMessage("SetApertureInternal", $"{aperture}, {withReset}");

            if (Aperture >= 0) {
                lock (apertureLock) {
                    if (withReset) {
                        CommandBlind($"A00#", raw: false);
                    }

                    CommandBlind($"A{aperture:D2}#", raw: false);
                    Aperture = aperture;
                }
            }
        }
    }
}