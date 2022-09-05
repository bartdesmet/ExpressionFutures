namespace RoslynPad
{
    partial class FontPicker
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPick = new System.Windows.Forms.Button();
            this.lblCaption = new System.Windows.Forms.Label();
            this.rtfSample = new System.Windows.Forms.RichTextBox();
            this.dlgFont = new System.Windows.Forms.FontDialog();
            this.SuspendLayout();
            // 
            // btnPick
            // 
            this.btnPick.Location = new System.Drawing.Point(456, 10);
            this.btnPick.Name = "btnPick";
            this.btnPick.Size = new System.Drawing.Size(31, 26);
            this.btnPick.TabIndex = 2;
            this.btnPick.Text = "...";
            this.btnPick.UseVisualStyleBackColor = true;
            this.btnPick.Click += new System.EventHandler(this.btnPick_Click);
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(8, 15);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(46, 20);
            this.lblCaption.TabIndex = 0;
            this.lblCaption.Text = "Font:";
            // 
            // rtfSample
            // 
            this.rtfSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtfSample.Location = new System.Drawing.Point(160, 3);
            this.rtfSample.MaximumSize = new System.Drawing.Size(290, 40);
            this.rtfSample.Multiline = false;
            this.rtfSample.Name = "rtfSample";
            this.rtfSample.ReadOnly = true;
            this.rtfSample.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rtfSample.Size = new System.Drawing.Size(290, 40);
            this.rtfSample.TabIndex = 1;
            this.rtfSample.Text = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.";
            this.rtfSample.WordWrap = false;
            // 
            // FontPicker
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.rtfSample);
            this.Controls.Add(this.btnPick);
            this.Controls.Add(this.lblCaption);
            this.Name = "FontPicker";
            this.Size = new System.Drawing.Size(500, 50);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPick;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.RichTextBox rtfSample;
        private System.Windows.Forms.FontDialog dlgFont;
    }
}
