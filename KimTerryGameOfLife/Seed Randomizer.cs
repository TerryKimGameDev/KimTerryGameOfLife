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
        public Seed_Randomizer()
        {
            InitializeComponent();
            SeedUpDown.Maximum = int.MaxValue;
            SeedUpDown.Minimum = int.MinValue;
            SeedUpDown.Value = Properties.Settings.Default.Seed;
        }

        private void Seed_Randomizer_Load(object sender, EventArgs e)
        {

        }

        private void Randomize_Click(object sender, EventArgs e)
        {
            Random gen = new Random(DateTime.Now.Millisecond);
            int seed = gen.Next(int.MinValue, int.MaxValue);
            SeedUpDown.Value = seed;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Seed = (int)SeedUpDown.Value;
            Properties.Settings.Default.Save();
        }
    }
}
