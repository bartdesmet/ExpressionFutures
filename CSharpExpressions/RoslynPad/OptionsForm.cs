using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoslynPad
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
        }

        public Font EditorFont
        {
            get
            {
                return fntEditor.SelectedFont;
            }

            set
            {
                fntEditor.SelectedFont = value;
            }
        }

        public Font DebugViewFont
        {
            get
            {
                return fntDebug.SelectedFont;
            }

            set
            {
                fntDebug.SelectedFont = value;
            }
        }

        public Font SyntaxFont
        {
            get
            {
                return fntSyntax.SelectedFont;
            }

            set
            {
                fntSyntax.SelectedFont = value;
            }
        }

        public Font TreeFont
        {
            get
            {
                return fntTree.SelectedFont;
            }

            set
            {
                fntTree.SelectedFont = value;
            }
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
        }
    }
}
