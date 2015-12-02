namespace RoslynPad
{
    partial class EvalForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EvalForm));
            this.dgvParams = new System.Windows.Forms.DataGridView();
            this.Parameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnEval = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabResult = new System.Windows.Forms.TabPage();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.tabConsole = new System.Windows.Forms.TabPage();
            this.txtLive = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParams)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabResult.SuspendLayout();
            this.tabConsole.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvParams
            // 
            this.dgvParams.AllowUserToAddRows = false;
            this.dgvParams.AllowUserToDeleteRows = false;
            this.dgvParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Parameter,
            this.Value});
            this.dgvParams.Location = new System.Drawing.Point(12, 12);
            this.dgvParams.Name = "dgvParams";
            this.dgvParams.RowTemplate.Height = 28;
            this.dgvParams.Size = new System.Drawing.Size(654, 150);
            this.dgvParams.TabIndex = 0;
            // 
            // Parameter
            // 
            this.Parameter.DataPropertyName = "Parameter";
            this.Parameter.HeaderText = "Parameter";
            this.Parameter.Name = "Parameter";
            this.Parameter.ReadOnly = true;
            // 
            // Value
            // 
            this.Value.DataPropertyName = "Value";
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            this.Value.Width = 250;
            // 
            // btnEval
            // 
            this.btnEval.Location = new System.Drawing.Point(12, 168);
            this.btnEval.Name = "btnEval";
            this.btnEval.Size = new System.Drawing.Size(98, 35);
            this.btnEval.TabIndex = 1;
            this.btnEval.Text = "Evaluate";
            this.btnEval.UseVisualStyleBackColor = true;
            this.btnEval.Click += new System.EventHandler(this.btnEval_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabResult);
            this.tabControl1.Controls.Add(this.tabConsole);
            this.tabControl1.Location = new System.Drawing.Point(12, 209);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(654, 223);
            this.tabControl1.TabIndex = 4;
            // 
            // tabResult
            // 
            this.tabResult.Controls.Add(this.txtResult);
            this.tabResult.Location = new System.Drawing.Point(4, 29);
            this.tabResult.Name = "tabResult";
            this.tabResult.Padding = new System.Windows.Forms.Padding(3);
            this.tabResult.Size = new System.Drawing.Size(646, 190);
            this.tabResult.TabIndex = 0;
            this.tabResult.Text = "Result";
            this.tabResult.UseVisualStyleBackColor = true;
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.Location = new System.Drawing.Point(3, 3);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(640, 184);
            this.txtResult.TabIndex = 3;
            // 
            // tabConsole
            // 
            this.tabConsole.Controls.Add(this.txtLive);
            this.tabConsole.Location = new System.Drawing.Point(4, 29);
            this.tabConsole.Name = "tabConsole";
            this.tabConsole.Padding = new System.Windows.Forms.Padding(3);
            this.tabConsole.Size = new System.Drawing.Size(646, 190);
            this.tabConsole.TabIndex = 1;
            this.tabConsole.Text = "Console";
            this.tabConsole.UseVisualStyleBackColor = true;
            // 
            // txtLive
            // 
            this.txtLive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLive.Location = new System.Drawing.Point(3, 3);
            this.txtLive.Multiline = true;
            this.txtLive.Name = "txtLive";
            this.txtLive.ReadOnly = true;
            this.txtLive.Size = new System.Drawing.Size(640, 184);
            this.txtLive.TabIndex = 4;
            // 
            // EvalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 444);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnEval);
            this.Controls.Add(this.dgvParams);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EvalForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Expression evaluator";
            this.Load += new System.EventHandler(this.EvalForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParams)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabResult.ResumeLayout(false);
            this.tabResult.PerformLayout();
            this.tabConsole.ResumeLayout(false);
            this.tabConsole.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvParams;
        private System.Windows.Forms.Button btnEval;
        private System.Windows.Forms.DataGridViewTextBoxColumn Parameter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabResult;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TabPage tabConsole;
        private System.Windows.Forms.TextBox txtLive;
    }
}