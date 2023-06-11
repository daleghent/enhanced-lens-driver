#region "copyright"

/*
    Copyright Dale Ghent <daleg@elemental.org>
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/
*/

#endregion "copyright"

using ASCOM.Utilities;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ASCOM.EnhancedCanonEF {

    [ComVisible(false)]
    public class SetupDialogForm : Form {
        private IContainer components = null;

        private Button cmdOK;

        private Button cmdCancel;

        private Label setupLabel;

        private PictureBox picASCOM;

        private Label COMPortLabel;

        private CheckBox checkBoxDebugLog;

        private ComboBox comboBoxComPort;

        private Label lensModelLabel;

        private ComboBox comboBoxLensModel;

        private Label defaultApertureLabel;

        private ComboBox comboBoxAppValue;

        private Label aboutLensTxtLabel;

        private Label label6;
        private Label titleLabel;
        private Label explainerLabel;
        private Label about;
        private Label label_MaxFocuserPos;
        private TextBox textBox_MaxFocusPosition;
        private Label lensTxtLink;

        public SetupDialogForm() {
            InitializeComponent();
            comboBoxComPort.Items.Clear();

            using (Serial serial = new Serial()) {
                string[] availableCOMPorts = serial.AvailableCOMPorts;
                foreach (string item in availableCOMPorts) {
                    comboBoxComPort.Items.Add(item);
                }
            }

            StreamReader streamReader = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86) + @"\ASCOM\Focuser\ASCOM.EnhancedLens.Controller\lens.txt");
            string text;

            while ((text = streamReader.ReadLine()) != null) {
                comboBoxLensModel.Items.Add(text.Substring(0, text.IndexOf('|') - 1));
            }

            while ((text = streamReader.ReadLine()) != null) {
                if (text.Substring(0, text.IndexOf('|') - 1) == comboBoxLensModel.Text) {
                    string text2 = text.Substring(text.IndexOf('|') + 2, text.Length - 1 - (text.IndexOf('|') + 1));
                    string[] array = text2.Split(' ', ',', ':', '\t');
                    comboBoxAppValue.Items.Clear();
                    comboBoxAppValue.ResetText();
                    string[] array2 = array;

                    Focuser.FocalRatioList.Clear();

                    foreach (string text3 in array2) {
                        var entry = "f/" + text3;
                        comboBoxAppValue.Items.Add(entry);
                        Focuser.FocalRatioList.Add(entry);
                    }
                }
            }

            streamReader.Close();

            try {
                comboBoxComPort.SelectedItem = Focuser.comPort;
                comboBoxLensModel.Text = Focuser.LensModel;
                comboBoxAppValue.Text = comboBoxAppValue.Items[Focuser.Aperture].ToString();
                textBox_MaxFocusPosition.Text = Focuser.MaxFocuserPos.ToString();
                checkBoxDebugLog.Checked = Focuser.traceState;
            } catch {
            }
        }

        private void cmdOK_Click(object sender, EventArgs e) {
            try {
                Focuser.comPort = (string)comboBoxComPort.SelectedItem;
                Focuser.LensModel = comboBoxLensModel.Text;
                Focuser.traceState = checkBoxDebugLog.Checked;
                if (comboBoxAppValue.SelectedItem.ToString() != "f/-1") {
                    Focuser.Aperture = Convert.ToInt32(comboBoxAppValue.SelectedIndex);
                } else {
                    Focuser.Aperture = -1;
                }
                Focuser.MaxFocuserPos = Convert.ToInt32(textBox_MaxFocusPosition.Text);
            } catch {
            }
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void BrowseToAscom(object sender, EventArgs e) {
            try {
                Process.Start("http://ascom-standards.org/");
            } catch (Win32Exception ex) {
                if (ex.ErrorCode == -2147467259) {
                    MessageBox.Show(ex.Message);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void BrowseToWebsite(object sender, EventArgs e) {
            try {
                Process.Start("https://daleghent.com/astromechanics-aperture-control");
            } catch (Win32Exception ex) {
                if (ex.ErrorCode == -2147467259) {
                    MessageBox.Show(ex.Message);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBoxLensModel_SelectedIndexChanged(object sender, EventArgs e) {
            StreamReader streamReader = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86) + @"\ASCOM\Focuser\ASCOM.EnhancedLens.Controller\lens.txt");
            string text;
            while ((text = streamReader.ReadLine()) != null) {
                if (text.Substring(0, text.IndexOf('|') - 1) == comboBoxLensModel.Text) {
                    string text2 = text.Substring(text.IndexOf('|') + 2, text.Length - 1 - (text.IndexOf('|') + 1));
                    string[] array = text2.Split(' ', ',', ':', '\t');
                    comboBoxAppValue.Items.Clear();
                    comboBoxAppValue.ResetText();
                    string[] array2 = array;

                    Focuser.FocalRatioList.Clear();

                    foreach (string text3 in array2) {
                        var entry = "f/" + text3;
                        comboBoxAppValue.Items.Add(entry);
                        Focuser.FocalRatioList.Add(entry);
                    }

                    comboBoxAppValue.Text = comboBoxAppValue.Items[0].ToString();
                }
            }
            streamReader.Close();
        }

        private void lensTxtLink_Click(object sender, EventArgs e) {
            try {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86) + @"\ASCOM\Focuser\ASCOM.EnhancedLens.Controller\lens.txt");
            } catch {
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing && components != null) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void textBox_MaxFocuserPos_KeyPress(object sender, KeyPressEventArgs e) {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.')) {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1)) {
                e.Handled = true;
            }
        }

        private void InitializeComponent() {
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.setupLabel = new System.Windows.Forms.Label();
            this.picASCOM = new System.Windows.Forms.PictureBox();
            this.COMPortLabel = new System.Windows.Forms.Label();
            this.checkBoxDebugLog = new System.Windows.Forms.CheckBox();
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.lensModelLabel = new System.Windows.Forms.Label();
            this.comboBoxLensModel = new System.Windows.Forms.ComboBox();
            this.defaultApertureLabel = new System.Windows.Forms.Label();
            this.comboBoxAppValue = new System.Windows.Forms.ComboBox();
            this.aboutLensTxtLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.explainerLabel = new System.Windows.Forms.Label();
            this.about = new System.Windows.Forms.Label();
            this.lensTxtLink = new System.Windows.Forms.Label();
            this.label_MaxFocuserPos = new System.Windows.Forms.Label();
            this.textBox_MaxFocusPosition = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(257, 341);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(59, 24);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(322, 340);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(59, 25);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // setupLabel
            // 
            this.setupLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.setupLabel.Location = new System.Drawing.Point(160, 27);
            this.setupLabel.Name = "setupLabel";
            this.setupLabel.Size = new System.Drawing.Size(44, 18);
            this.setupLabel.TabIndex = 2;
            this.setupLabel.Text = "Setup";
            // 
            // picASCOM
            // 
            this.picASCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picASCOM.Image = global::ASCOM.EnhancedCanonEF.Properties.Resources.ASCOM;
            this.picASCOM.Location = new System.Drawing.Point(333, 9);
            this.picASCOM.Name = "picASCOM";
            this.picASCOM.Size = new System.Drawing.Size(48, 56);
            this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picASCOM.TabIndex = 3;
            this.picASCOM.TabStop = false;
            this.picASCOM.Click += new System.EventHandler(this.BrowseToAscom);
            this.picASCOM.DoubleClick += new System.EventHandler(this.BrowseToAscom);
            // 
            // COMPortLabel
            // 
            this.COMPortLabel.AutoSize = true;
            this.COMPortLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.COMPortLabel.Location = new System.Drawing.Point(55, 87);
            this.COMPortLabel.Name = "COMPortLabel";
            this.COMPortLabel.Size = new System.Drawing.Size(60, 15);
            this.COMPortLabel.TabIndex = 5;
            this.COMPortLabel.Text = "COM Port";
            // 
            // checkBoxDebugLog
            // 
            this.checkBoxDebugLog.AutoSize = true;
            this.checkBoxDebugLog.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBoxDebugLog.Location = new System.Drawing.Point(124, 242);
            this.checkBoxDebugLog.Name = "checkBoxDebugLog";
            this.checkBoxDebugLog.Size = new System.Drawing.Size(108, 19);
            this.checkBoxDebugLog.TabIndex = 6;
            this.checkBoxDebugLog.Text = "Debug Logging";
            this.checkBoxDebugLog.UseVisualStyleBackColor = true;
            // 
            // comboBoxComPort
            // 
            this.comboBoxComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxComPort.FormattingEnabled = true;
            this.comboBoxComPort.Location = new System.Drawing.Point(123, 84);
            this.comboBoxComPort.Name = "comboBoxComPort";
            this.comboBoxComPort.Size = new System.Drawing.Size(242, 21);
            this.comboBoxComPort.TabIndex = 7;
            // 
            // lensModelLabel
            // 
            this.lensModelLabel.AutoSize = true;
            this.lensModelLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lensModelLabel.Location = new System.Drawing.Point(47, 122);
            this.lensModelLabel.Name = "lensModelLabel";
            this.lensModelLabel.Size = new System.Drawing.Size(68, 15);
            this.lensModelLabel.TabIndex = 12;
            this.lensModelLabel.Text = "Lens Model";
            // 
            // comboBoxLensModel
            // 
            this.comboBoxLensModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLensModel.FormattingEnabled = true;
            this.comboBoxLensModel.Location = new System.Drawing.Point(123, 119);
            this.comboBoxLensModel.MaxDropDownItems = 20;
            this.comboBoxLensModel.Name = "comboBoxLensModel";
            this.comboBoxLensModel.Size = new System.Drawing.Size(242, 21);
            this.comboBoxLensModel.TabIndex = 13;
            this.comboBoxLensModel.SelectedIndexChanged += new System.EventHandler(this.comboBoxLensModel_SelectedIndexChanged);
            // 
            // defaultApertureLabel
            // 
            this.defaultApertureLabel.AutoSize = true;
            this.defaultApertureLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.defaultApertureLabel.Location = new System.Drawing.Point(21, 207);
            this.defaultApertureLabel.Name = "defaultApertureLabel";
            this.defaultApertureLabel.Size = new System.Drawing.Size(94, 15);
            this.defaultApertureLabel.TabIndex = 14;
            this.defaultApertureLabel.Text = "Default Aperture";
            // 
            // comboBoxAppValue
            // 
            this.comboBoxAppValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAppValue.FormattingEnabled = true;
            this.comboBoxAppValue.Location = new System.Drawing.Point(124, 205);
            this.comboBoxAppValue.Name = "comboBoxAppValue";
            this.comboBoxAppValue.Size = new System.Drawing.Size(242, 21);
            this.comboBoxAppValue.TabIndex = 15;
            // 
            // aboutLensTxtLabel
            // 
            this.aboutLensTxtLabel.AutoSize = true;
            this.aboutLensTxtLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.aboutLensTxtLabel.Location = new System.Drawing.Point(121, 148);
            this.aboutLensTxtLabel.Name = "aboutLensTxtLabel";
            this.aboutLensTxtLabel.Size = new System.Drawing.Size(216, 45);
            this.aboutLensTxtLabel.TabIndex = 16;
            this.aboutLensTxtLabel.Text = "If this list does not contain your lens,\r\nadd it to              in the format de" +
    "scribed\r\nin that file.";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 23);
            this.label6.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.titleLabel.Location = new System.Drawing.Point(64, 9);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(234, 18);
            this.titleLabel.TabIndex = 21;
            this.titleLabel.Text = "Enhanced Astromechanics Lens Driver";
            // 
            // explainerLabel
            // 
            this.explainerLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.explainerLabel.Location = new System.Drawing.Point(47, 45);
            this.explainerLabel.Name = "explainerLabel";
            this.explainerLabel.Size = new System.Drawing.Size(269, 18);
            this.explainerLabel.TabIndex = 18;
            this.explainerLabel.Text = "An unofficial driver for Astromechanics focusers";
            // 
            // about
            // 
            this.about.AutoSize = true;
            this.about.Cursor = System.Windows.Forms.Cursors.Hand;
            this.about.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.about.ForeColor = System.Drawing.Color.Red;
            this.about.Location = new System.Drawing.Point(22, 312);
            this.about.Name = "about";
            this.about.Size = new System.Drawing.Size(95, 15);
            this.about.TabIndex = 17;
            this.about.Text = "About this driver";
            this.about.Click += new System.EventHandler(this.BrowseToWebsite);
            this.about.DoubleClick += new System.EventHandler(this.BrowseToWebsite);
            // 
            // lensTxtLink
            // 
            this.lensTxtLink.AutoSize = true;
            this.lensTxtLink.BackColor = System.Drawing.Color.Transparent;
            this.lensTxtLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lensTxtLink.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lensTxtLink.ForeColor = System.Drawing.Color.Red;
            this.lensTxtLink.Location = new System.Drawing.Point(172, 164);
            this.lensTxtLink.Name = "lensTxtLink";
            this.lensTxtLink.Size = new System.Drawing.Size(45, 15);
            this.lensTxtLink.TabIndex = 18;
            this.lensTxtLink.Text = "lens.txt";
            this.lensTxtLink.Click += new System.EventHandler(this.lensTxtLink_Click);
            // 
            // label_MaxFocuserPos
            // 
            this.label_MaxFocuserPos.AutoSize = true;
            this.label_MaxFocuserPos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_MaxFocuserPos.Location = new System.Drawing.Point(22, 276);
            this.label_MaxFocuserPos.Name = "label_MaxFocuserPos";
            this.label_MaxFocuserPos.Size = new System.Drawing.Size(152, 15);
            this.label_MaxFocuserPos.TabIndex = 22;
            this.label_MaxFocuserPos.Text = "Maximum Focuser Position";
            // 
            // textBox_MaxFocusPosition
            // 
            this.textBox_MaxFocusPosition.Location = new System.Drawing.Point(180, 274);
            this.textBox_MaxFocusPosition.Name = "textBox_MaxFocusPosition";
            this.textBox_MaxFocusPosition.Size = new System.Drawing.Size(185, 20);
            this.textBox_MaxFocusPosition.TabIndex = 23;
            this.textBox_MaxFocusPosition.Text = "10000";
            this.textBox_MaxFocusPosition.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_MaxFocuserPos_KeyPress);
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 377);
            this.Controls.Add(this.textBox_MaxFocusPosition);
            this.Controls.Add(this.label_MaxFocuserPos);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.explainerLabel);
            this.Controls.Add(this.picASCOM);
            this.Controls.Add(this.lensTxtLink);
            this.Controls.Add(this.aboutLensTxtLabel);
            this.Controls.Add(this.comboBoxAppValue);
            this.Controls.Add(this.defaultApertureLabel);
            this.Controls.Add(this.comboBoxLensModel);
            this.Controls.Add(this.lensModelLabel);
            this.Controls.Add(this.comboBoxComPort);
            this.Controls.Add(this.checkBoxDebugLog);
            this.Controls.Add(this.COMPortLabel);
            this.Controls.Add(this.setupLabel);
            this.Controls.Add(this.about);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupDialogForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}