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

namespace ASCOM.EnhancedCanonEF2 {

    [ComVisible(false)]
    public class SetupDialogForm : Form {
        private IContainer components = null;

        private Button cmdOK;

        private Button cmdCancel;

        private Label label1;

        private PictureBox picASCOM;

        private Label label2;

        private CheckBox chkTrace;

        private ComboBox comboBoxComPort;

        private Label label4;

        private ComboBox comboBoxLensModel;

        private Label label5;

        private ComboBox comboBoxAppValue;

        private Label label3;

        private Label label6;

        private Label label7;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label8;

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
                chkTrace.Checked = Focuser.traceState;
            } catch {
            }
        }

        private void cmdOK_Click(object sender, EventArgs e) {
            try {
                Focuser.comPort = (string)comboBoxComPort.SelectedItem;
                Focuser.LensModel = comboBoxLensModel.Text;
                Focuser.traceState = chkTrace.Checked;
                if (comboBoxAppValue.SelectedItem.ToString() != "f/-1") {
                    Focuser.Aperture = Convert.ToInt32(comboBoxAppValue.SelectedIndex);
                } else {
                    Focuser.Aperture = -1;
                }
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

        private void label6_Click(object sender, EventArgs e) {
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

        private void InitializeComponent() {
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.picASCOM = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkTrace = new System.Windows.Forms.CheckBox();
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxLensModel = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxAppValue = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.SuspendLayout();
            //
            // cmdOK
            //
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(246, 279);
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
            this.cmdCancel.Location = new System.Drawing.Point(311, 278);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(59, 25);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            //
            // label1
            //
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(160, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "Setup";
            //
            // picASCOM
            //
            this.picASCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picASCOM.Image = global::ASCOM.EnhancedCanonEF2.Properties.Resources.ASCOM;
            this.picASCOM.Location = new System.Drawing.Point(333, 9);
            this.picASCOM.Name = "picASCOM";
            this.picASCOM.Size = new System.Drawing.Size(36, 36);
            this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picASCOM.TabIndex = 3;
            this.picASCOM.TabStop = false;
            this.picASCOM.Click += new System.EventHandler(this.BrowseToAscom);
            this.picASCOM.DoubleClick += new System.EventHandler(this.BrowseToAscom);
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(55, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "COM Port";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            //
            // chkTrace
            //
            this.chkTrace.AutoSize = true;
            this.chkTrace.Location = new System.Drawing.Point(123, 232);
            this.chkTrace.Name = "chkTrace";
            this.chkTrace.Size = new System.Drawing.Size(99, 17);
            this.chkTrace.TabIndex = 6;
            this.chkTrace.Text = "Debug Logging";
            this.chkTrace.UseVisualStyleBackColor = true;
            this.chkTrace.Visible = false;
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
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(47, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Lens Model";
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
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(14, 194);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Default Aperture";
            //
            // comboBoxAppValue
            //
            this.comboBoxAppValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAppValue.FormattingEnabled = true;
            this.comboBoxAppValue.Location = new System.Drawing.Point(123, 191);
            this.comboBoxAppValue.Name = "comboBoxAppValue";
            this.comboBoxAppValue.Size = new System.Drawing.Size(242, 21);
            this.comboBoxAppValue.TabIndex = 15;
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(121, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(230, 36);
            this.label3.TabIndex = 16;
            this.label3.Text = "If this list does not contain your lens,\r\nadd it to            in the format desc" +
    "ribed\r\nin that file.";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            //
            // label6
            //
            this.label6.AutoSize = true;
            this.label6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label6.Font = new System.Drawing.Font("Verdana", 7.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(171, 155);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 17;
            this.label6.Text = "lens.txt";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            //
            // label7
            //
            this.label7.AutoSize = true;
            this.label7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label7.Enabled = false;
            this.label7.Font = new System.Drawing.Font("Verdana", 7.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(35, 279);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 12);
            this.label7.TabIndex = 18;
            this.label7.Text = "Product page";
            this.label7.Visible = false;
            //
            // label8
            //
            this.label8.AutoSize = true;
            this.label8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label8.Enabled = false;
            this.label8.Font = new System.Drawing.Font("Verdana", 7.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(41, 291);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 12);
            this.label8.TabIndex = 19;
            this.label8.Text = "PDF manual";
            this.label8.Visible = false;
            //
            // label9
            //
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(34, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(293, 18);
            this.label9.TabIndex = 20;
            this.label9.Text = "Enhanced Astromechanics CanonEF Driver";
            this.label9.Click += new System.EventHandler(this.label9_Click);
            //
            // label10
            //
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(47, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(293, 18);
            this.label10.TabIndex = 21;
            this.label10.Text = "An unofficial driver for Astromechanics focusers";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            //
            // label11
            //
            this.label11.AutoSize = true;
            this.label11.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label11.Enabled = false;
            this.label11.Font = new System.Drawing.Font("Verdana", 7.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(17, 267);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(96, 12);
            this.label11.TabIndex = 22;
            this.label11.Text = "About this driver";
            this.label11.Visible = false;
            //
            // SetupDialogForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 315);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxAppValue);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxLensModel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBoxComPort);
            this.Controls.Add(this.chkTrace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.picASCOM);
            this.Controls.Add(this.label1);
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

        private void label2_Click(object sender, EventArgs e) {
        }

        private void label9_Click(object sender, EventArgs e) {
        }

        private void label3_Click(object sender, EventArgs e) {
        }

        private void label10_Click(object sender, EventArgs e) {
        }
    }
}