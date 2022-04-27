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
    public partial class Seed_Randomizer : Form
    {

        //constructor
        public Seed_Randomizer()
        {
            InitializeComponent();
            //setup for seed numeric
            SeedUpDown.Maximum = int.MaxValue;
            SeedUpDown.Minimum = int.MinValue;
            SeedUpDown.Value = Properties.Settings.Default.Seed;
        }
        //button funtion for randomizer
        private void Randomize_Click(object sender, EventArgs e)
        {
            // get a seed base on time and show it in numeric;
            Random gen = new Random(DateTime.Now.Millisecond);
            int seed = gen.Next(int.MinValue, int.MaxValue);
            SeedUpDown.Value = seed;
        }

        //ok button saves seed value to property
        private void OK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Seed = (int)SeedUpDown.Value;
            Properties.Settings.Default.Save();
        }
    }
}
