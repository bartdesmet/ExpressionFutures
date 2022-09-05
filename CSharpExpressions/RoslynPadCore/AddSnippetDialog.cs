using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RoslynPad
{
    public partial class AddSnippetDialog : Form
    {
        private readonly IDictionary<string, string> _catalog;

        public AddSnippetDialog()
        {
            InitializeComponent();
        }

        public AddSnippetDialog(IDictionary<string, string> catalog)
            : this()
        {
            _catalog = catalog;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_catalog.ContainsKey(SnippetName))
            {
                var res = MessageBox.Show("An entry with this name already exists. Replace the existing item?", this.Text, MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.OK;
                }
                else if (res == DialogResult.No)
                {
                    this.DialogResult = DialogResult.Cancel;
                }
            }
        }

        public string SnippetName
        {
            get { return txtName.Text; }
        }
    }
}
