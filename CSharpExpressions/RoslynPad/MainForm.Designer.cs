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
            this.btnOptimize = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.chkModern = new System.Windows.Forms.CheckBox();
            this.btnReduce = new System.Windows.Forms.Button();
            this.btnEval = new System.Windows.Forms.Button();
            this.cmbProgs = new System.Windows.Forms.ComboBox();
            this.btnCompile = new System.Windows.Forms.Button();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabCode = new System.Windows.Forms.TabPage();
            this.rtf = new System.Windows.Forms.RichTextBox();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.tabTree = new System.Windows.Forms.TabPage();
            this.pnlTree = new System.Windows.Forms.SplitContainer();
            this.trvExpr = new System.Windows.Forms.TreeView();
            this.pnlDetail = new System.Windows.Forms.SplitContainer();
            this.prgNode = new System.Windows.Forms.PropertyGrid();
            this.txtNode = new System.Windows.Forms.TextBox();
            this.tabCSharp = new System.Windows.Forms.TabPage();
            this.txtCSharp = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRun = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCompile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptimize = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReduce = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEvaluate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.tabIL = new System.Windows.Forms.TabPage();
            this.txtIL = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabCode.SuspendLayout();
            this.tabDebug.SuspendLayout();
            this.tabTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlTree)).BeginInit();
            this.pnlTree.Panel1.SuspendLayout();
            this.pnlTree.Panel2.SuspendLayout();
            this.pnlTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlDetail)).BeginInit();
            this.pnlDetail.Panel1.SuspendLayout();
            this.pnlDetail.Panel2.SuspendLayout();
            this.pnlDetail.SuspendLayout();
            this.tabCSharp.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabIL.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.btnOptimize);
            this.splitContainer1.Panel1.Controls.Add(this.btnAdd);
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
            this.splitContainer1.Panel2.Controls.Add(this.tabMain);
            this.splitContainer1.Size = new System.Drawing.Size(1002, 679);
            this.splitContainer1.SplitterDistance = 236;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnOptimize
            // 
            this.btnOptimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptimize.Enabled = false;
            this.btnOptimize.Location = new System.Drawing.Point(684, 187);
            this.btnOptimize.Name = "btnOptimize";
            this.btnOptimize.Size = new System.Drawing.Size(98, 35);
            this.btnOptimize.TabIndex = 8;
            this.btnOptimize.Text = "Optimize";
            this.btnOptimize.UseVisualStyleBackColor = true;
            this.btnOptimize.Click += new System.EventHandler(this.btnOptimize_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.AutoSize = true;
            this.btnAdd.Image = global::RoslynPad.Properties.Resources.AddItemstoFolder_13217_32;
            this.btnAdd.Location = new System.Drawing.Point(607, 12);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(34, 29);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
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
            this.cmbProgs.Location = new System.Drawing.Point(647, 12);
            this.cmbProgs.Name = "cmbProgs";
            this.cmbProgs.Size = new System.Drawing.Size(343, 28);
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
            this.txtCode.TextChanged += new System.EventHandler(this.txtCode_TextChanged);
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
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabCode);
            this.tabMain.Controls.Add(this.tabDebug);
            this.tabMain.Controls.Add(this.tabTree);
            this.tabMain.Controls.Add(this.tabCSharp);
            this.tabMain.Controls.Add(this.tabIL);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1002, 439);
            this.tabMain.TabIndex = 1;
            // 
            // tabCode
            // 
            this.tabCode.Controls.Add(this.rtf);
            this.tabCode.Location = new System.Drawing.Point(4, 29);
            this.tabCode.Name = "tabCode";
            this.tabCode.Size = new System.Drawing.Size(994, 406);
            this.tabCode.TabIndex = 2;
            this.tabCode.Text = "Code";
            this.tabCode.UseVisualStyleBackColor = true;
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
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.txtResult);
            this.tabDebug.Location = new System.Drawing.Point(4, 29);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tabDebug.Size = new System.Drawing.Size(994, 406);
            this.tabDebug.TabIndex = 0;
            this.tabDebug.Text = "Debug View";
            this.tabDebug.UseVisualStyleBackColor = true;
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
            // tabTree
            // 
            this.tabTree.Controls.Add(this.pnlTree);
            this.tabTree.Location = new System.Drawing.Point(4, 29);
            this.tabTree.Name = "tabTree";
            this.tabTree.Padding = new System.Windows.Forms.Padding(3);
            this.tabTree.Size = new System.Drawing.Size(994, 406);
            this.tabTree.TabIndex = 1;
            this.tabTree.Text = "Tree";
            this.tabTree.UseVisualStyleBackColor = true;
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
            // tabCSharp
            // 
            this.tabCSharp.Controls.Add(this.txtCSharp);
            this.tabCSharp.Location = new System.Drawing.Point(4, 29);
            this.tabCSharp.Name = "tabCSharp";
            this.tabCSharp.Padding = new System.Windows.Forms.Padding(3);
            this.tabCSharp.Size = new System.Drawing.Size(994, 406);
            this.tabCSharp.TabIndex = 3;
            this.tabCSharp.Text = "C# Decompiled";
            this.tabCSharp.UseVisualStyleBackColor = true;
            // 
            // txtCSharp
            // 
            this.txtCSharp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCSharp.Font = new System.Drawing.Font("Consolas", 8F);
            this.txtCSharp.Location = new System.Drawing.Point(3, 3);
            this.txtCSharp.Multiline = true;
            this.txtCSharp.Name = "txtCSharp";
            this.txtCSharp.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCSharp.Size = new System.Drawing.Size(988, 400);
            this.txtCSharp.TabIndex = 0;
            this.txtCSharp.WordWrap = false;
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
            this.actionsToolStripMenuItem,
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
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveasToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(218, 30);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(218, 30);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(218, 30);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveasToolStripMenuItem
            // 
            this.saveasToolStripMenuItem.Name = "saveasToolStripMenuItem";
            this.saveasToolStripMenuItem.Size = new System.Drawing.Size(218, 30);
            this.saveasToolStripMenuItem.Text = "Save &as...";
            this.saveasToolStripMenuItem.Click += new System.EventHandler(this.saveasToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(215, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(218, 30);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRun,
            this.toolStripSeparator2,
            this.mnuCompile,
            this.mnuOptimize,
            this.mnuReduce,
            this.mnuEvaluate});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(83, 29);
            this.actionsToolStripMenuItem.Text = "&Actions";
            // 
            // mnuRun
            // 
            this.mnuRun.Name = "mnuRun";
            this.mnuRun.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuRun.Size = new System.Drawing.Size(215, 30);
            this.mnuRun.Text = "&Run...";
            this.mnuRun.Click += new System.EventHandler(this.mnuRun_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(212, 6);
            // 
            // mnuCompile
            // 
            this.mnuCompile.Name = "mnuCompile";
            this.mnuCompile.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.mnuCompile.Size = new System.Drawing.Size(215, 30);
            this.mnuCompile.Text = "&Compile";
            this.mnuCompile.Click += new System.EventHandler(this.mnuCompile_Click);
            // 
            // mnuOptimize
            // 
            this.mnuOptimize.Enabled = false;
            this.mnuOptimize.Name = "mnuOptimize";
            this.mnuOptimize.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.mnuOptimize.Size = new System.Drawing.Size(215, 30);
            this.mnuOptimize.Text = "&Optimize";
            this.mnuOptimize.Click += new System.EventHandler(this.mnuOptimize_Click);
            // 
            // mnuReduce
            // 
            this.mnuReduce.Enabled = false;
            this.mnuReduce.Name = "mnuReduce";
            this.mnuReduce.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.mnuReduce.Size = new System.Drawing.Size(215, 30);
            this.mnuReduce.Text = "Re&duce";
            this.mnuReduce.Click += new System.EventHandler(this.mnuReduce_Click);
            // 
            // mnuEvaluate
            // 
            this.mnuEvaluate.Enabled = false;
            this.mnuEvaluate.Name = "mnuEvaluate";
            this.mnuEvaluate.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.mnuEvaluate.Size = new System.Drawing.Size(215, 30);
            this.mnuEvaluate.Text = "&Evaluate...";
            this.mnuEvaluate.Click += new System.EventHandler(this.mnuEvaluate_Click);
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
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(173, 30);
            this.optionsToolStripMenuItem.Text = "&Options...";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // openFile
            // 
            this.openFile.DefaultExt = "xml";
            this.openFile.FileName = "DefaultCatalog.xml";
            this.openFile.Filter = "Catalogs|*.xml";
            this.openFile.Title = "Select a catalog";
            // 
            // saveFile
            // 
            this.saveFile.DefaultExt = "xml";
            this.saveFile.FileName = "MyCatalog.xml";
            this.saveFile.Filter = "Catalogs|*.xml";
            this.saveFile.Title = "Save catalog";
            // 
            // tabIL
            // 
            this.tabIL.Controls.Add(this.txtIL);
            this.tabIL.Location = new System.Drawing.Point(4, 29);
            this.tabIL.Name = "tabIL";
            this.tabIL.Size = new System.Drawing.Size(994, 406);
            this.tabIL.TabIndex = 4;
            this.tabIL.Text = "IL";
            this.tabIL.UseVisualStyleBackColor = true;
            // 
            // txtIL
            // 
            this.txtIL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIL.Font = new System.Drawing.Font("Consolas", 8F);
            this.txtIL.Location = new System.Drawing.Point(0, 0);
            this.txtIL.Multiline = true;
            this.txtIL.Name = "txtIL";
            this.txtIL.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtIL.Size = new System.Drawing.Size(994, 406);
            this.txtIL.TabIndex = 0;
            this.txtIL.WordWrap = false;
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabCode.ResumeLayout(false);
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            this.tabTree.ResumeLayout(false);
            this.pnlTree.Panel1.ResumeLayout(false);
            this.pnlTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlTree)).EndInit();
            this.pnlTree.ResumeLayout(false);
            this.pnlDetail.Panel1.ResumeLayout(false);
            this.pnlDetail.Panel2.ResumeLayout(false);
            this.pnlDetail.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlDetail)).EndInit();
            this.pnlDetail.ResumeLayout(false);
            this.tabCSharp.ResumeLayout(false);
            this.tabCSharp.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabIL.ResumeLayout(false);
            this.tabIL.PerformLayout();
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
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabDebug;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TabPage tabTree;
        private System.Windows.Forms.SplitContainer pnlTree;
        private System.Windows.Forms.TreeView trvExpr;
        private System.Windows.Forms.SplitContainer pnlDetail;
        private System.Windows.Forms.PropertyGrid prgNode;
        private System.Windows.Forms.TextBox txtNode;
        private System.Windows.Forms.TabPage tabCode;
        private System.Windows.Forms.RichTextBox rtf;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuRun;
        private System.Windows.Forms.ToolStripMenuItem mnuCompile;
        private System.Windows.Forms.ToolStripMenuItem mnuEvaluate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuReduce;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveasToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFile;
        private System.Windows.Forms.TabPage tabCSharp;
        private System.Windows.Forms.TextBox txtCSharp;
        private System.Windows.Forms.Button btnOptimize;
        private System.Windows.Forms.ToolStripMenuItem mnuOptimize;
        private System.Windows.Forms.TabPage tabIL;
        private System.Windows.Forms.TextBox txtIL;
    }
}

