using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoslynPad
{
    public partial class FontPicker : UserControl
    {
        public FontPicker()
        {
            InitializeComponent();
        }

        public string Caption
        {
            get
            {
                return lblCaption.Text;
            }

            set
            {
                lblCaption.Text = value;
            }
        }

        public Font SelectedFont
        {
            get
            {
                return rtfSample.Font;
            }

            set
            {
                rtfSample.Text = $"{value.Name}, {value.SizeInPoints}pt";
                rtfSample.Font = value;
            }
        }

        private void btnPick_Click(object sender, EventArgs e)
        {
            dlgFont.Font = SelectedFont;
            if (dlgFont.ShowDialog() == DialogResult.OK)
            {
                SelectedFont = dlgFont.Font;
            }
        }
    }
}
