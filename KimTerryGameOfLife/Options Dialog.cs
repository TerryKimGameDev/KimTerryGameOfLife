using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KimTerryGameOfLife
{
    public partial class Options_Dialog : Form
    {
        public Options_Dialog()
        {
            InitializeComponent();
            //what values the numericup downs should display on opening //ps the max and mins were already set in properties
            TimeInterval.Value = Properties.Settings.Default.Interval;
            UniWidth.Value = Properties.Settings.Default.UniWidth;
            UniHeight.Value = Properties.Settings.Default.UniHeight;
        }

        //clicking ok button saves to defaults
        private void ButtonOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Interval = (int)TimeInterval.Value;
            Properties.Settings.Default.UniWidth = (uint)UniWidth.Value;
            Properties.Settings.Default.UniHeight = (uint)UniHeight.Value;
            Properties.Settings.Default.Save();
        }
    }
}
