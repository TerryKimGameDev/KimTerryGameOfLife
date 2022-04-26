
namespace KimTerryGameOfLife
{
    partial class Options_Dialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.TimeInterval = new System.Windows.Forms.NumericUpDown();
            this.UniWidth = new System.Windows.Forms.NumericUpDown();
            this.UniHeight = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.TimeInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UniWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UniHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.ButtonOK.Location = new System.Drawing.Point(107, 212);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 0;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(188, 212);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 1;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // TimeInterval
            // 
            this.TimeInterval.Location = new System.Drawing.Point(232, 58);
            this.TimeInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.TimeInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TimeInterval.Name = "TimeInterval";
            this.TimeInterval.Size = new System.Drawing.Size(70, 20);
            this.TimeInterval.TabIndex = 2;
            this.TimeInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // UniWidth
            // 
            this.UniWidth.Location = new System.Drawing.Point(232, 84);
            this.UniWidth.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.UniWidth.Name = "UniWidth";
            this.UniWidth.Size = new System.Drawing.Size(70, 20);
            this.UniWidth.TabIndex = 3;
            this.UniWidth.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // UniHeight
            // 
            this.UniHeight.Location = new System.Drawing.Point(232, 110);
            this.UniHeight.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.UniHeight.Name = "UniHeight";
            this.UniHeight.Size = new System.Drawing.Size(70, 20);
            this.UniHeight.TabIndex = 4;
            this.UniHeight.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Timer Interval in Milliseconds";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(69, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Width of Universe in Cells";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(69, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Height of Universe in Cells";
            // 
            // Options_Dialog
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(370, 247);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UniHeight);
            this.Controls.Add(this.UniWidth);
            this.Controls.Add(this.TimeInterval);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options_Dialog";
            this.Text = "Options Dialog";
            ((System.ComponentModel.ISupportInitialize)(this.TimeInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UniWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UniHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.NumericUpDown TimeInterval;
        private System.Windows.Forms.NumericUpDown UniWidth;
        private System.Windows.Forms.NumericUpDown UniHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}