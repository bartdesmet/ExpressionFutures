namespace RoslynPad
{
    partial class OptionsForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.fntTree = new RoslynPad.FontPicker();
            this.fntSyntax = new RoslynPad.FontPicker();
            this.fntDebug = new RoslynPad.FontPicker();
            this.fntEditor = new RoslynPad.FontPicker();
            this.fntDetails = new RoslynPad.FontPicker();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.fntDetails);
            this.groupBox1.Controls.Add(this.fntTree);
            this.groupBox1.Controls.Add(this.fntSyntax);
            this.groupBox1.Controls.Add(this.fntDebug);
            this.groupBox1.Controls.Add(this.fntEditor);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(754, 344);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Display settings";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(291, 492);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(95, 35);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(392, 492);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(95, 35);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // fntTree
            // 
            this.fntTree.AutoSize = true;
            this.fntTree.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fntTree.Caption = "Tree font:";
            this.fntTree.Location = new System.Drawing.Point(6, 214);
            this.fntTree.Name = "fntTree";
            this.fntTree.SelectedFont = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fntTree.Size = new System.Drawing.Size(490, 46);
            this.fntTree.TabIndex = 7;
            // 
            // fntSyntax
            // 
            this.fntSyntax.AutoSize = true;
            this.fntSyntax.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fntSyntax.Caption = "Syntax font:";
            this.fntSyntax.Location = new System.Drawing.Point(6, 154);
            this.fntSyntax.Name = "fntSyntax";
            this.fntSyntax.SelectedFont = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fntSyntax.Size = new System.Drawing.Size(490, 46);
            this.fntSyntax.TabIndex = 6;
            // 
            // fntDebug
            // 
            this.fntDebug.AutoSize = true;
            this.fntDebug.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fntDebug.Caption = "Debug view font:";
            this.fntDebug.Location = new System.Drawing.Point(6, 94);
            this.fntDebug.Name = "fntDebug";
            this.fntDebug.SelectedFont = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fntDebug.Size = new System.Drawing.Size(490, 46);
            this.fntDebug.TabIndex = 5;
            // 
            // fntEditor
            // 
            this.fntEditor.AutoSize = true;
            this.fntEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fntEditor.Caption = "Editor font:";
            this.fntEditor.Location = new System.Drawing.Point(6, 34);
            this.fntEditor.Name = "fntEditor";
            this.fntEditor.SelectedFont = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fntEditor.Size = new System.Drawing.Size(490, 46);
            this.fntEditor.TabIndex = 4;
            // 
            // fntDetails
            // 
            this.fntDetails.AutoSize = true;
            this.fntDetails.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fntDetails.Caption = "Details font:";
            this.fntDetails.Location = new System.Drawing.Point(6, 274);
            this.fntDetails.Name = "fntDetails";
            this.fntDetails.SelectedFont = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fntDetails.Size = new System.Drawing.Size(490, 46);
            this.fntDetails.TabIndex = 8;
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(778, 544);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private FontPicker fntTree;
        private FontPicker fntSyntax;
        private FontPicker fntDebug;
        private FontPicker fntEditor;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private FontPicker fntDetails;
    }
}