using System.Drawing;
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

        public Font DetailsFont
        {
            get
            {
                return fntDetails.SelectedFont;
            }

            set
            {
                fntDetails.SelectedFont = value;
            }
        }
    }
}
