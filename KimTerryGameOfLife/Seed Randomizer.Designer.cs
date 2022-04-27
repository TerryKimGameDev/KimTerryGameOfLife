
namespace KimTerryGameOfLife
{
    partial class Seed_Randomizer
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
            this.label1 = new System.Windows.Forms.Label();
            this.SeedUpDown = new System.Windows.Forms.NumericUpDown();
            this.Randomize = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SeedUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Seed";
            // 
            // SeedUpDown
            // 
            this.SeedUpDown.Location = new System.Drawing.Point(93, 48);
            this.SeedUpDown.Name = "SeedUpDown";
            this.SeedUpDown.Size = new System.Drawing.Size(120, 20);
            this.SeedUpDown.TabIndex = 1;
            // 
            // Randomize
            // 
            this.Randomize.Location = new System.Drawing.Point(219, 45);
            this.Randomize.Name = "Randomize";
            this.Randomize.Size = new System.Drawing.Size(75, 23);
            this.Randomize.TabIndex = 2;
            this.Randomize.Text = "Randomize";
            this.Randomize.UseVisualStyleBackColor = false;
            this.Randomize.Click += new System.EventHandler(this.Randomize_Click);
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(86, 95);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 3;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(188, 95);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 4;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // Seed_Randomizer
            // 
            this.AcceptButton = this.OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(349, 130);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Randomize);
            this.Controls.Add(this.SeedUpDown);
            this.Controls.Add(this.label1);
            this.Name = "Seed_Randomizer";
            this.Text = "Seed_Randomizer";
            ((System.ComponentModel.ISupportInitialize)(this.SeedUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown SeedUpDown;
        private System.Windows.Forms.Button Randomize;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
    }
}