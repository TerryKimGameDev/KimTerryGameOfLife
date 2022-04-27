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
    public partial class RunToDialogue : Form
    {
        //fields or properties

        //generation for runto form
        public int generations { get; set; } = 0;
        //stopat for runto form
        public int StopAt { get; private set; }
        public RunToDialogue()
        {
            InitializeComponent();

        }
        //when first shown will get initial values for numeric updown
        private void RunToDialogue_Shown(object sender, EventArgs e)
        {
            RunGen.Value = generations + 1;
            RunGen.Minimum = generations;
        }

        //clicking ok stores the numeric value to property
        private void OK_Click(object sender, EventArgs e)
        {
            StopAt = (int)RunGen.Value;
        }
    }
}
