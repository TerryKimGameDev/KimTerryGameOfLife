using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

//github
//https://github.com/TerryKimGameDev/KimTerryGameOfLife
//Most bits of code are organized in regions open them to look at code
namespace KimTerryGameOfLife
{
    public partial class Form1 : Form
    {
        //all member fields
        #region All member Fields
        // The universe array
        CellState[,] universe = new CellState[20, 20];


        // Drawing colors
        Color gridColor = Form1.DefaultForeColor;
        Color grid10Color = Color.Black;
        Color cellColor = Color.Gray;
        //for background
        Color BackgroundColor = Form1.DefaultBackColor;

        //textbrush
        Brush tBrush = new SolidBrush(Color.Red);

        //stores the seed on load for the reload logic
        int Lseed = Properties.Settings.Default.Seed;


        // The Timer class
        Timer timer = new Timer();

        //stop at specific generations
        int stopat = -1;

        // Generation count
        public int generations = 0;
        //the interval for the timer
        int interval = Properties.Settings.Default.Interval;
        //set world state for toroidal or finite
        bool world = false;
        //bool for the neighbor count
        bool DisplayCount = true;
        //bool for grid
        bool grid = true;
        //bool for hud
        bool HUD = true;
        //next generation array
        CellState[,] NextGen; //this is the scratchpad 
        #endregion All MemberFields

        //ctor/nextgen/timertick
        #region Constructor/nextgeneration/timertick
        //constructor for form1
        public Form1()
        {
            InitializeComponent();

            //setup for the universe size and initialization of the scratchpad and universe
            universe = new CellState[Properties.Settings.Default.UniWidth, Properties.Settings.Default.UniHeight];
            setNextGenSize();
            arrayInit();

            // Setup the timer
            timer.Interval = interval; // milliseconds
            timer.Tick += Timer_Tick;

            //***made change so timer does not automatically run***
            //timer.Enabled = true; // start timer running

        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // Increment generation count
            generations++;
            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            swap();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();

            if (generations == stopat)
            {
                Play.Enabled = true;
                startToolStripMenuItem.Enabled = true;
                pause.Enabled = false;
                pauseToolStripMenuItem.Enabled = false;
                timer.Stop();
            }

        }

        #endregion Constructor/nextgeneration/timertick

        //graphics panel funtionality
        #region All Display/ graphics panel funcionality

        //painter
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            //int for live cell count for the hud

            //added an int for the count
            int count;

            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0) - 0.01f;
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1) - 0.01f;


            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            //default cell colors or when dead cause I did not like using panel back color
            Brush Dbrush = new SolidBrush(BackgroundColor);

            int Lcell = 0;
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    //***changed to rectF
                    RectangleF cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y].GetCellState() == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        Lcell++;
                    }
                    else
                    {
                        e.Graphics.FillRectangle(Dbrush, cellRect); //should not have been need but did it this way anyways
                    }

                    //change value in status bar
                    AliveCells.Text = "Alive = " + Lcell.ToString();
                    //Change to toroidal or finite base on a bool
                    if (world == false)
                    {
                        count = CountNeighborsToroidal(x, y);
                    }
                    else
                    {
                        count = CountNeighborsFinite(x, y);
                    }

                    //call to the cell rules
                    cellRules(count, x, y);

                    //the display for the neighbors based on count and bool
                    if (count > 0 && DisplayCount == true)
                    {
                        NeighborDisplay(e, cellRect, count);
                    }
                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }
            // 10 by 10 grid display
            Grid10by10(e);

            //hud display based on bool
            if (HUD == true)
            {
                HUDelements(e, Lcell, generations, world);
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();

        }

        //mouse click on panel
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                //***float changes to clickevents
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = (int)(e.X / cellWidth);
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = (int)(e.Y / cellHeight);

                // Toggle the cell's state
                //universe[x, y] = !universe[x, y];

                universe[x, y].toggle();
                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        //the 10 by 10 grid display
        private void Grid10by10(PaintEventArgs e)
        {
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0) - 0.01f;
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1) - 0.01f;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;

                    if (x % 10 == 0 && y % 10 == 0)
                    {
                        //draw the 10 by 10 grid based on modulo of 10
                        e.Graphics.DrawRectangle(new Pen(grid10Color, 2), cellRect.X, cellRect.Y, cellWidth * 10, cellHeight * 10);
                    }
                }
            }
        }


        //rules for the Cell generation
        private void cellRules(int count, int x, int y)
        {
            tBrush = new SolidBrush(Color.Red);
            if (count < 2)
            {
                //NextGen[x, y] = false; //scratchpad death
                NextGen[x, y].SetLcells(false);
            }
            if (count > 3)
            {
                //NextGen[x, y] = false;
                NextGen[x, y].SetLcells(false);
            }
            if (universe[x, y].GetCellState() == true && count == 2 || count == 3)
            {
                //NextGen[x, y] = true;
                NextGen[x, y].SetLcells(true);
                tBrush = new SolidBrush(Color.Green);
            }
            else NextGen[x, y].SetLcells(false);

            if (universe[x, y].GetCellState() == false && count == 3)
            {
                //NextGen[x, y] = true;
                NextGen[x, y].SetLcells(true);
                tBrush = new SolidBrush(Color.Green);
            }
        }

        //display neighbor count
        private void NeighborDisplay(PaintEventArgs e, RectangleF rect, int count)
        {
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1) - 0.01f;

            //string format settings
            Font font = new Font("Arial", cellHeight * 0.7f, FontStyle.Bold);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            //draws neighbor count to display
            e.Graphics.DrawString((count).ToString(), font, tBrush, rect, stringFormat);
        }

        //finite universe
        private int CountNeighborsFinite(int x, int y)
        {

            int count = 0;

            int xLen = universe.GetLength(0);

            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }
                    //test(e, xCheck, yCheck, count);
                    if (universe[xCheck, yCheck].GetCellState() == true) count++;
                }
            }
            return count;
        }
        //toroidal universe
        private int CountNeighborsToroidal(int x, int y)

        {

            int count = 0;

            int xLen = universe.GetLength(0);

            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)

            {

                for (int xOffset = -1; xOffset <= 1; xOffset++)

                {

                    int xCheck = x + xOffset;

                    int yCheck = y + yOffset;

                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }

                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }

                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }


                    if (universe[xCheck, yCheck].GetCellState() == true) count++;

                }

            }
            return count;

        }
        //The Hud display
        private void HUDelements(PaintEventArgs e, int count, int generations, bool World)
        {
            //hud format string
            Font font = new Font("Arial", 12f, FontStyle.Bold);

            //should a string display as toroidal or finite
            string s = (world == false) ? "Toroidal" : "Finite";
            //the string to display
            string Hudtext = $"Generations: {generations}\nCell Count: {count}\nBoundary Type:{s}\nUniverse Size:(Width={universe.GetLength(0)}, Height={universe.GetLength(1)})";
            //a rectangle the size of the panel to adjust shape automatically
            RectangleF rect = new RectangleF(0, 0, graphicsPanel1.ClientSize.Width, graphicsPanel1.ClientSize.Height);

            //align text to bottom left
            StringFormat stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Far;

            //draw the hud
            e.Graphics.DrawString(Hudtext, font, Brushes.Salmon, rect, stringFormat);
        }
        #endregion All Displays

        //all Buttons
        #region All Button functionality
        //activate timer for the game
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            stopat = -1;
            timer.Start(); //starts timer.... duh to me

            graphicsPanel1.Invalidate();

            //enable and disable buttons
            Play.Enabled = false;
            startToolStripMenuItem.Enabled = false;
            pause.Enabled = true;
            pauseToolStripMenuItem.Enabled = true;
        }

        // click for the next gen
        private void next_Click(object sender, EventArgs e)
        {
            stopat = -1;
            NextGeneration();
            graphicsPanel1.Invalidate();
        }

        //pause game
        private void pause_Click(object sender, EventArgs e)
        {
            timer.Stop(); //pause timer

            //enable and disable button
            pause.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            Play.Enabled = true;
            startToolStripMenuItem.Enabled = true;
        }

        //RunTo functionality
        private void runToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //stops timer
            timer.Stop();

            //activate and deactivate associated buttons
            Play.Enabled = true;
            startToolStripMenuItem.Enabled = true;
            pause.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;

            //initialize runto dialogue
            RunToDialogue RunThrough = new RunToDialogue();
            RunThrough.generations = generations;

            //open run through
            if (DialogResult.OK == RunThrough.ShowDialog())
            {
                //activate and deactivate associated buttons
                Play.Enabled = false;
                startToolStripMenuItem.Enabled = false;
                pause.Enabled = true;
                pauseToolStripMenuItem.Enabled = true;

                //store stopat
                stopat = RunThrough.StopAt;
                //start timer
                timer.Start();
            }
        }

        //finite view
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //bool for the finite or toroidal view of universe
            world = true;

            //checkmark logic
            finiteToolStripMenuItem.Checked = true;
            toriToolStripMenuItem.Checked = false;

            graphicsPanel1.Invalidate();
        }

        //toroidal view
        private void toriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //bool for world view
            world = false;
            //checkmark logic
            toriToolStripMenuItem.Checked = true;
            finiteToolStripMenuItem.Checked = false;
            graphicsPanel1.Invalidate();
        }

        //toggle the neighbor count
        private void NeighborCount_Click(object sender, EventArgs e)
        {
            DisplayCount = !DisplayCount;
            graphicsPanel1.Invalidate();
        }

        //turn off or on the grid
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridControl();
        }

        //seed from time
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clear universe
            GridReset();

            //rand based on time
            Random time = new Random(DateTime.Now.Millisecond);

            //get seed value
            int seed = time.Next(int.MinValue, int.MaxValue);

            //change prop seed
            Properties.Settings.Default.Seed = seed;

            //seed the random
            Random Rint = new Random(seed);

            //fill values for live/ dead cells
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    int num = Rint.Next(0, 3);
                    if (num == 0)
                    {
                        universe[i, j].SetLcells(true);
                    }
                    else universe[i, j].SetLcells(false);
                }
            }
            SeedStatus.Text = "Seed = " + seed.ToString();
            graphicsPanel1.Invalidate();
        }

        //open up the random dialogue when clicked
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //randomizer dialog box
            Seed_Randomizer RanDLg = new Seed_Randomizer();

            if (DialogResult.OK == RanDLg.ShowDialog())
            {
                //clear grid
                GridReset();
                //seed random
                Random Rint = new Random(Properties.Settings.Default.Seed);

                //fill universe based on seed
                for (int i = 0; i < universe.GetLength(0); i++)
                {
                    for (int j = 0; j < universe.GetLength(1); j++)
                    {
                        int num = Rint.Next(0, 3);
                        if (num == 0)
                        {
                            universe[i, j].SetLcells(true);
                        }
                        else universe[i, j].SetLcells(false);
                    }
                }
                SeedStatus.Text = "Seed = " + Properties.Settings.Default.Seed.ToString();
            }
            graphicsPanel1.Invalidate();
        }

        //from current seed
        private void currentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clear the universe
            GridReset();

            //store seed
            int seed = Properties.Settings.Default.Seed;

            //seed random
            Random Rint = new Random(seed);

            //fill values for live/ dead cells
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    int num = Rint.Next(0, 3);
                    if (num == 0)
                    {
                        universe[i, j].SetLcells(true);
                    }
                    else universe[i, j].SetLcells(false);
                }
            }
            SeedStatus.Text = "Seed = " + seed.ToString();
            graphicsPanel1.Invalidate();
        }
        //Hud activate/deactivate
        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hudControl();
        }



        //context menu items
        #region Context menu items
        //context sensitive hud activation
        private void hudToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            hudControl();
        }

        //context sensitive grid activation
        private void gridToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GridControl();
        }

        //context background color
        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundColor = CdialogControl(BackgroundColor);
            SaveChanges();
            graphicsPanel1.Invalidate();
        }

        //context cellcolor
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cellColor = CdialogControl(cellColor);
            SaveChanges();
            graphicsPanel1.Invalidate();
        }

        //context grid color
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = CdialogControl(gridColor);
            SaveChanges();
            graphicsPanel1.Invalidate();
        }

        //context grid10 color
        private void gridx10ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grid10Color = CdialogControl(grid10Color);
            SaveChanges();
            graphicsPanel1.Invalidate();
        }
        #endregion Context menu items

        //settings menu strip buttons
        #region Settings menu strip items
        //back color
        private void backColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            BackgroundColor = CdialogControl(BackgroundColor);
            SaveChanges();
            graphicsPanel1.Invalidate();
        }

        //cell color
        private void cellColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = CdialogControl(cellColor);
            SaveChanges();
            graphicsPanel1.Invalidate();
        }

        //grid color
        private void gridColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            gridColor = CdialogControl(gridColor);
            SaveChanges();
            graphicsPanel1.Invalidate();
        }

        //gridx10 color
        private void gridx10ColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            grid10Color = CdialogControl(grid10Color);
            SaveChanges();
            graphicsPanel1.Invalidate();
        }

        //open us the options dialogue when clicked
        private void Options_Click(object sender, EventArgs e)
        {
            //options dialog box
            Options_Dialog opdlg = new Options_Dialog();

            if (DialogResult.OK == opdlg.ShowDialog())
            {
                //set interval display for the status bar
                Interval.Text = "Interval = " + Properties.Settings.Default.Interval.ToString();

                //change what interval value is used for timer
                timer.Interval = Properties.Settings.Default.Interval;

                //if check so to not new univer should nothing have been changed
                if (universe.GetLength(0) != Properties.Settings.Default.UniWidth || universe.GetLength(1) != Properties.Settings.Default.UniHeight)
                {
                    //set universe and scratchpad size
                    universe = new CellState[Properties.Settings.Default.UniWidth, Properties.Settings.Default.UniHeight];
                    setNextGenSize();
                    arrayInit();
                }

            }
            graphicsPanel1.Invalidate();
        }

        //reset the universe and colors to true default settings
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            universe = new CellState[Properties.Settings.Default.UniWidth, Properties.Settings.Default.UniHeight];
            cellColor = KimTerryGameOfLife.Properties.Settings.Default.CellColor;
            gridColor = KimTerryGameOfLife.Properties.Settings.Default.GridColor;
            grid10Color = KimTerryGameOfLife.Properties.Settings.Default.Grid10Color;
            BackgroundColor = KimTerryGameOfLife.Properties.Settings.Default.BackgroundColor;
            timer.Interval = Properties.Settings.Default.Interval;
            Interval.Text = "Interval = " + timer.Interval.ToString();
            SeedStatus.Text = Properties.Settings.Default.Seed.ToString();
            GridReset();

        }

        //reload the user settings
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            universe = new CellState[Properties.Settings.Default.UniWidth, Properties.Settings.Default.UniHeight];
            cellColor = KimTerryGameOfLife.Properties.Settings.Default.CellColor;
            gridColor = KimTerryGameOfLife.Properties.Settings.Default.GridColor;
            grid10Color = KimTerryGameOfLife.Properties.Settings.Default.Grid10Color;
            BackgroundColor = KimTerryGameOfLife.Properties.Settings.Default.BackgroundColor;
            timer.Interval = Properties.Settings.Default.Interval;
            Interval.Text = "Interval = " + timer.Interval.ToString();
            Properties.Settings.Default.Seed = Lseed;
            SeedStatus.Text = Lseed.ToString();
            GridReset();
        }
        #endregion Settings menu strip items

        //run tool strip function buttons
        #region RunToolStripFunctions

        //start timer
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Start(); //starts timer....

            graphicsPanel1.Invalidate();

            //enable and disable buttons
            Play.Enabled = false;
            startToolStripMenuItem.Enabled = false;
            pause.Enabled = true;
            pauseToolStripMenuItem.Enabled = true;
        }

        //pause timer
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop(); //pause timer

            //enable and disable button
            pause.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            Play.Enabled = true;
            startToolStripMenuItem.Enabled = true;
        }

        //next gen
        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }
        #endregion RunToolStripFunctions

        // New, save and load buttons
        #region New, Save and load buttons
        //clear grid/ new game
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            //call to function so new grid
            GridReset();
        }
        //save as
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        //open txt
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFile();
        }
        #endregion New, Save and load buttons

        //tool strip file items
        #region Tool strip Files
        //news the panal
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridReset();
        }
        //open file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }
        //save file

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }
        //exit form

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #endregion All Buttons

        //control Groups// All created functions for different uses
        #region Control
        //grid control
        private void GridControl()
        {
            //toggles
            grid = !grid;
            gridToolStripMenuItem.Checked = grid;
            gridToolStripMenuItem1.Checked = grid;

            //logic for the grid to show or not
            if (grid == true)
            {
                gridColor = KimTerryGameOfLife.Properties.Settings.Default.GridColor;
                grid10Color = KimTerryGameOfLife.Properties.Settings.Default.Grid10Color;
            }
            else
            {
                gridColor = Color.Empty;
                grid10Color = Color.Empty;
            }
            graphicsPanel1.Invalidate();
        }
        //hud control
        private void hudControl()
        {
            //all toggles
            HUD = !HUD; //bool for hud display
            hUDToolStripMenuItem.Checked = HUD;
            hudToolStripMenuItem1.Checked = HUD;
            graphicsPanel1.Invalidate();
        }
        //Set the next gen array size to the univere size 
        private void setNextGenSize()
        {
            NextGen = new CellState[universe.GetLength(0), universe.GetLength(1)];
        }
        //initialize the arrays
        private void arrayInit()
        {

            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    universe[i, j] = new CellState();
                    NextGen[i, j] = new CellState();
                }
            }
        }
        //a swap function
        private void swap()
        {
            // Swap them...
            CellState[,] temp = universe;
            universe = NextGen;
            NextGen = temp;
        }
        //Grid reset control // for newing the grid
        private void GridReset()
        {
            timer.Stop();
            generations = 0;
            Play.Enabled = true;
            pause.Enabled = false;

            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            setNextGenSize();
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    universe[i, j] = new CellState();
                    NextGen[i, j] = new CellState();
                }
            }

            graphicsPanel1.Invalidate();
        }


        //color dialog control for changing whatever needs to have a color change
        private Color CdialogControl(Color clr)
        {
            ColorDialog Cdlg = new ColorDialog();
            Cdlg.Color = clr;
            if (DialogResult.OK == Cdlg.ShowDialog())
            {
                clr = Cdlg.Color;
            }
            return clr;
        }

        //save file
        private void SaveFile()
        {
            //save dialogue and setup
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";

            //show the save dialogue
            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!" + DateTime.Now + "\n");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x, y].GetCellState() == true)
                        {
                            currentRow += 'O';
                        }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else
                        {
                            currentRow += '.';
                        }
                    }

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        //open file
        private void OpenFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    if (!row.StartsWith("!") && !string.IsNullOrEmpty(row))
                    {
                        maxHeight++;
                    }
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    maxWidth = row.Length;
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = new CellState[maxWidth, maxHeight];
                setNextGenSize();
                arrayInit();


                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);


                //yposition of universe
                int y = 0;

                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();


                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    if (!row.StartsWith("!") && !string.IsNullOrEmpty(row))
                    {
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // If row[xPos] is a 'O' (capital O) then
                            // set the corresponding cell in the universe to alive.
                            if (row[xPos] == 'O')
                            {
                                universe[xPos, y].SetLcells(true);
                            }
                            // If row[xPos] is a '.' (period) then
                            // set the corresponding cell in the universe to dead.
                            else
                            {
                                universe[xPos, y].SetLcells(false);
                            }
                        }
                        y++;
                    }
                }
                graphicsPanel1.Invalidate();
                // Close the file.
                reader.Close();
            }
        }

        #endregion All Controls

        //User settings change functions
        #region UserSettings
        //Load in user settings on open
        private void Form1_Load(object sender, EventArgs e)
        {
            cellColor = KimTerryGameOfLife.Properties.Settings.Default.CellColor;
            gridColor = KimTerryGameOfLife.Properties.Settings.Default.GridColor;
            grid10Color = KimTerryGameOfLife.Properties.Settings.Default.Grid10Color;
            BackgroundColor = KimTerryGameOfLife.Properties.Settings.Default.BackgroundColor;
            Interval.Text = "Interval = " + Properties.Settings.Default.Interval.ToString();
            SeedStatus.Text = "Seed = " + Properties.Settings.Default.Seed.ToString();

        }
        //Save user settings on close //this is kinda redundant with the code below this
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            KimTerryGameOfLife.Properties.Settings.Default.CellColor = cellColor;
            KimTerryGameOfLife.Properties.Settings.Default.GridColor = gridColor;
            KimTerryGameOfLife.Properties.Settings.Default.Grid10Color = grid10Color;
            KimTerryGameOfLife.Properties.Settings.Default.BackgroundColor = BackgroundColor;
            KimTerryGameOfLife.Properties.Settings.Default.Save();
        }

        //save changes when they are made
        private void SaveChanges()
        {
            KimTerryGameOfLife.Properties.Settings.Default.CellColor = cellColor;
            KimTerryGameOfLife.Properties.Settings.Default.GridColor = gridColor;
            KimTerryGameOfLife.Properties.Settings.Default.Grid10Color = grid10Color;
            KimTerryGameOfLife.Properties.Settings.Default.BackgroundColor = BackgroundColor;
            KimTerryGameOfLife.Properties.Settings.Default.Save();
        }




        #endregion User Settings

        
    }
}
