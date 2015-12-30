using System;
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

        private void ChangeFonts(Func<Font, Font> change)
        {
            EditorFont = change(EditorFont);
            DebugViewFont = change(DebugViewFont);
            SyntaxFont = change(SyntaxFont);
            TreeFont = change(TreeFont);
            DetailsFont = change(DetailsFont);
        }

        private Font IncreaseFont(Font font)
        {
            var size = Math.Min(32, font.Size + 2);
            return new Font(font.FontFamily, size, font.Style, font.Unit);
        }

        private Font DecreaseFont(Font font)
        {
            var size = Math.Max(8, font.Size - 2);
            return new Font(font.FontFamily, size, font.Style, font.Unit);
        }

        private void btnGrowAll_Click(object sender, EventArgs e)
        {
            ChangeFonts(IncreaseFont);
        }

        private void btnShrinkAll_Click(object sender, EventArgs e)
        {
            ChangeFonts(DecreaseFont);
        }
    }
}
