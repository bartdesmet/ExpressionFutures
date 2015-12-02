namespace RoslynPad
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chkModern = new System.Windows.Forms.CheckBox();
            this.btnReduce = new System.Windows.Forms.Button();
            this.btnEval = new System.Windows.Forms.Button();
            this.cmbProgs = new System.Windows.Forms.ComboBox();
            this.btnCompile = new System.Windows.Forms.Button();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.rtf = new System.Windows.Forms.RichTextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pnlTree = new System.Windows.Forms.SplitContainer();
            this.trvExpr = new System.Windows.Forms.TreeView();
            this.pnlDetail = new System.Windows.Forms.SplitContainer();
            this.prgNode = new System.Windows.Forms.PropertyGrid();
            this.txtNode = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlTree)).BeginInit();
            this.pnlTree.Panel1.SuspendLayout();
            this.pnlTree.Panel2.SuspendLayout();
            this.pnlTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlDetail)).BeginInit();
            this.pnlDetail.Panel1.SuspendLayout();
            this.pnlDetail.Panel2.SuspendLayout();
            this.pnlDetail.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 33);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chkModern);
            this.splitContainer1.Panel1.Controls.Add(this.btnReduce);
            this.splitContainer1.Panel1.Controls.Add(this.btnEval);
            this.splitContainer1.Panel1.Controls.Add(this.cmbProgs);
            this.splitContainer1.Panel1.Controls.Add(this.btnCompile);
            this.splitContainer1.Panel1.Controls.Add(this.txtCode);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1002, 679);
            this.splitContainer1.SplitterDistance = 236;
            this.splitContainer1.TabIndex = 0;
            // 
            // chkModern
            // 
            this.chkModern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkModern.AutoSize = true;
            this.chkModern.Checked = true;
            this.chkModern.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkModern.Location = new System.Drawing.Point(136, 193);
            this.chkModern.Name = "chkModern";
            this.chkModern.Size = new System.Drawing.Size(304, 24);
            this.chkModern.TabIndex = 4;
            this.chkModern.Text = "Include Microsoft.CSharp.Expressions";
            this.chkModern.UseVisualStyleBackColor = true;
            // 
            // btnReduce
            // 
            this.btnReduce.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReduce.Enabled = false;
            this.btnReduce.Location = new System.Drawing.Point(788, 187);
            this.btnReduce.Name = "btnReduce";
            this.btnReduce.Size = new System.Drawing.Size(98, 35);
            this.btnReduce.TabIndex = 5;
            this.btnReduce.Text = "Reduce";
            this.btnReduce.UseVisualStyleBackColor = true;
            this.btnReduce.Click += new System.EventHandler(this.btnReduce_Click);
            // 
            // btnEval
            // 
            this.btnEval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEval.Enabled = false;
            this.btnEval.Location = new System.Drawing.Point(892, 187);
            this.btnEval.Name = "btnEval";
            this.btnEval.Size = new System.Drawing.Size(98, 35);
            this.btnEval.TabIndex = 6;
            this.btnEval.Text = "Evaluate";
            this.btnEval.UseVisualStyleBackColor = true;
            this.btnEval.Click += new System.EventHandler(this.btnEval_Click);
            // 
            // cmbProgs
            // 
            this.cmbProgs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbProgs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProgs.DropDownWidth = 200;
            this.cmbProgs.FormattingEnabled = true;
            this.cmbProgs.Location = new System.Drawing.Point(791, 12);
            this.cmbProgs.Name = "cmbProgs";
            this.cmbProgs.Size = new System.Drawing.Size(199, 28);
            this.cmbProgs.TabIndex = 1;
            this.cmbProgs.SelectedIndexChanged += new System.EventHandler(this.cmbProgs_SelectedIndexChanged);
            // 
            // btnCompile
            // 
            this.btnCompile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCompile.Location = new System.Drawing.Point(16, 187);
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(98, 35);
            this.btnCompile.TabIndex = 3;
            this.btnCompile.Text = "Compile";
            this.btnCompile.UseVisualStyleBackColor = true;
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // txtCode
            // 
            this.txtCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCode.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCode.Location = new System.Drawing.Point(16, 46);
            this.txtCode.Multiline = true;
            this.txtCode.Name = "txtCode";
            this.txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCode.Size = new System.Drawing.Size(974, 131);
            this.txtCode.TabIndex = 2;
            this.txtCode.Text = "(Expression<Func<int>>)(() => 42)";
            this.txtCode.WordWrap = false;
            this.txtCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCode_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter an expression:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1002, 439);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.rtf);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(994, 406);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Code";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // rtf
            // 
            this.rtf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtf.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtf.Location = new System.Drawing.Point(0, 0);
            this.rtf.Name = "rtf";
            this.rtf.ReadOnly = true;
            this.rtf.Size = new System.Drawing.Size(994, 406);
            this.rtf.TabIndex = 8;
            this.rtf.Text = "";
            this.rtf.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rtf_MouseMove);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtResult);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(994, 406);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Debug View";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtResult.Location = new System.Drawing.Point(3, 3);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(988, 400);
            this.txtResult.TabIndex = 1;
            this.txtResult.WordWrap = false;
            this.txtResult.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCode_KeyDown);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pnlTree);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(994, 406);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Tree";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pnlTree
            // 
            this.pnlTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTree.Location = new System.Drawing.Point(3, 3);
            this.pnlTree.Name = "pnlTree";
            // 
            // pnlTree.Panel1
            // 
            this.pnlTree.Panel1.Controls.Add(this.trvExpr);
            // 
            // pnlTree.Panel2
            // 
            this.pnlTree.Panel2.Controls.Add(this.pnlDetail);
            this.pnlTree.Size = new System.Drawing.Size(988, 400);
            this.pnlTree.SplitterDistance = 512;
            this.pnlTree.TabIndex = 1;
            // 
            // trvExpr
            // 
            this.trvExpr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvExpr.Location = new System.Drawing.Point(0, 0);
            this.trvExpr.Name = "trvExpr";
            this.trvExpr.Size = new System.Drawing.Size(512, 400);
            this.trvExpr.TabIndex = 1;
            this.trvExpr.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvExpr_AfterSelect);
            // 
            // pnlDetail
            // 
            this.pnlDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetail.Location = new System.Drawing.Point(0, 0);
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // pnlDetail.Panel1
            // 
            this.pnlDetail.Panel1.Controls.Add(this.prgNode);
            // 
            // pnlDetail.Panel2
            // 
            this.pnlDetail.Panel2.Controls.Add(this.txtNode);
            this.pnlDetail.Size = new System.Drawing.Size(472, 400);
            this.pnlDetail.SplitterDistance = 199;
            this.pnlDetail.TabIndex = 1;
            // 
            // prgNode
            // 
            this.prgNode.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.prgNode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prgNode.HelpVisible = false;
            this.prgNode.Location = new System.Drawing.Point(0, 0);
            this.prgNode.Name = "prgNode";
            this.prgNode.Size = new System.Drawing.Size(472, 199);
            this.prgNode.TabIndex = 1;
            this.prgNode.ToolbarVisible = false;
            // 
            // txtNode
            // 
            this.txtNode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNode.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNode.Location = new System.Drawing.Point(0, 0);
            this.txtNode.Multiline = true;
            this.txtNode.Name = "txtNode";
            this.txtNode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtNode.Size = new System.Drawing.Size(472, 197);
            this.txtNode.TabIndex = 0;
            this.txtNode.WordWrap = false;
            // 
            // toolTip
            // 
            this.toolTip.ForeColor = System.Drawing.Color.Red;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1002, 33);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.optionsToolStripMenuItem.Text = "&Options...";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 712);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "RoslynPad";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.pnlTree.Panel1.ResumeLayout(false);
            this.pnlTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlTree)).EndInit();
            this.pnlTree.ResumeLayout(false);
            this.pnlDetail.Panel1.ResumeLayout(false);
            this.pnlDetail.Panel2.ResumeLayout(false);
            this.pnlDetail.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlDetail)).EndInit();
            this.pnlDetail.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCompile;
        private System.Windows.Forms.ComboBox cmbProgs;
        private System.Windows.Forms.Button btnEval;
        private System.Windows.Forms.Button btnReduce;
        private System.Windows.Forms.CheckBox chkModern;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer pnlTree;
        private System.Windows.Forms.TreeView trvExpr;
        private System.Windows.Forms.SplitContainer pnlDetail;
        private System.Windows.Forms.PropertyGrid prgNode;
        private System.Windows.Forms.TextBox txtNode;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox rtf;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    }
}

