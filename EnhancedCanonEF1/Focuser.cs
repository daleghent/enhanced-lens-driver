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
        internal static int Aperture;
        internal static List<string> FocalRatioList = new List<string>();
        internal static string apertureDefault = "0";
        internal static string apertureProfileName = "Aperture";
        internal static string comPort;
        internal static string comPortDefault = "COM7";
        internal static string comPortProfileName = "COM Port";
        internal static string driverID = "ASCOM.EnhancedCanonEF.Focuser";
        internal static string LensModel;
        internal static string LensModelDefault = "";
        internal static string LensModelProfileName = "LensModel";
        internal static bool traceState;
        internal static string traceStateDefault = "false";
        internal static string traceStateProfileName = "Trace Level";
        private static string driverDescription = "Enhanced ASCOM Lens Driver (Focuser 1)";
        private bool connectedState;
        private readonly object serialLock = new object();

        private int focuserPosition = 5000;
        private Serial serialPort;
        private TraceLogger tl;

        public Focuser() {
            ReadProfile();
            tl = new TraceLogger("", "EnhancedCanonEF") {
                Enabled = traceState
            };

            tl.LogMessage("Focuser", "Starting initialization");
            connectedState = false;
            tl.LogMessage("Focuser", "Completed initialization");
        }

        public bool Absolute => true;

        public bool Connected {
            get {
                return serialPort != null && serialPort.Connected;
            }
            set {
                if (value) {
                    if (serialPort == null || !serialPort.Connected) {
                        string text = comPort;
                        if (string.IsNullOrEmpty(text)) {
                            throw new NotConnectedException("No COM port has been selected");
                        }
                        try {
                            serialPort = new Serial {
                                PortName = text,
                                Speed = SerialSpeed.ps38400,
                                DTREnable = true,
                                Connected = true
                            };
                        } catch (Exception ex) {
                            throw new NotConnectedException(ex.Message.ToString(), (Exception)(object)ex);
                        }
                    }
                } else if (serialPort != null && serialPort.Connected) {
                    serialPort.Connected = false;
                }
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
                tl.LogMessage("Link Get", Connected.ToString());
                return Connected;
            }
            set {
                tl.LogMessage("Link Set", value.ToString());
                Connected = value;
            }
        }

        public int MaxIncrement {
            get {
                tl.LogMessage("MaxIncrement Get", 10000.ToString());
                return 10000;
            }
        }

        public int MaxStep {
            get {
                tl.LogMessage("MaxStep Get", 10000.ToString());
                return 10000;
            }
        }

        public string Name => driverDescription;

        public int Position {
            get {
                if (Aperture != -1) {
                    CommandBlind($"A{Aperture:D2}#", raw: false);
                }

                return focuserPosition;
            }
        }

        public double StepSize => throw new PropertyNotImplementedException("StepSize", accessorSet: false);

        public ArrayList SupportedActions {
            get {
                var actions = new ArrayList() {
                    "GetApertureIndex",
                    "GetFocalRatioList",
                    "GetLensModel",
                    "SetAperture",
                };

                tl.LogMessage("SupportedActions Get", actions.ToString());
                return actions;
            }
        }

        public bool TempComp {
            get => false;
            set => throw new PropertyNotImplementedException("TempComp", accessorSet: true);
        }

        public bool TempCompAvailable => false;

        public double Temperature => throw new PropertyNotImplementedException("Temperature", accessorSet: false);

        private bool IsConnected => connectedState;

        [ComRegisterFunction]
        public static void RegisterASCOM(Type t) {
            RegUnregASCOM(bRegister: true);
        }

        [ComUnregisterFunction]
        public static void UnregisterASCOM(Type t) {
            RegUnregASCOM(bRegister: false);
        }

        public string Action(string actionName, string actionParameters) {
            tl.LogMessage("Action", $"{actionName} {actionParameters}");
            var response = string.Empty;

            // ASCOM spec requires that the Action be accepted regardless of case, so
            // normalize the supplied Action name to lower-case
            var actionNameLC = actionName.ToLower();

            switch (actionNameLC) {
                case "getlensmodel":
                    response = GetLensModel();
                    break;

                case "setaperture":
                    var aperture = int.Parse(actionParameters);
                    SetApertureAction(aperture);
                    break;

                case "getapertureindex":
                    response = Aperture.ToString();
                    break;

                case "getfocalratiolist":
                    response = string.Join(",", FocalRatioList);
                    break;

                default:
                    throw new ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
            }

            return response;
        }

        public void CommandBlind(string command, bool raw) {
            if (!Connected) {
                throw new NotConnectedException();
            }

            lock (serialLock) {
                serialPort.ClearBuffers();
                serialPort.Transmit(command);
            }
        }

        public bool CommandBool(string command, bool raw) {
            throw new MethodNotImplementedException("CommandBool");
        }

        public string CommandString(string command, bool raw) {
            if (!Connected) {
                throw new NotConnectedException();
            }

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

        public void Move(int Value) {
            CommandBlind("M" + Value + "#", raw: true);
            string s = CommandString("P#", raw: false);
            focuserPosition = int.Parse(s);
        }

        public void SetupDialog() {
            if (IsConnected) {
                MessageBox.Show("This driver is already connected to the focuser. Just press OK");
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

        private string GetLensModel() {
            return LensModel;
        }

        private void SetApertureAction(int aperture) {
            Aperture = aperture;
            CommandBlind("A00#", true);
            CommandBlind($"A{Aperture:D2}#", true);
        }
    }
}