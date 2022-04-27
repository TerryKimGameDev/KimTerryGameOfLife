﻿using System;
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

        //for default grid color
        Color dgrid = Color.Black;
        Color dgrid10 = Color.Black;


        // The Timer class
        Timer timer = new Timer();

        Brush tBrush = new SolidBrush(Color.Red);

        // Generation count
        int generations = 0;
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
        #endregion

        //Set the next gen array size to the univere size 
        private void setNextGenSize()
        {
            NextGen = new CellState[universe.GetLength(0), universe.GetLength(1)];
        }


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



        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // Increment generation count
            generations++;
            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            swap();
        }

        private void swap()
        {

            // Swap them...
            CellState[,] temp = universe;
            universe = NextGen;
            NextGen = temp;
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }
        //graphics panel funtionality
        #region All Display/ graphics panel funcionality
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            //int for live cell count for the hud
            int Lcell = 0;

            //added an int for the count
            int count;
            //***float change above
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0) - 0.01f;
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1) - 0.01f;


            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            //default cell colors
            Brush Dbrush = new SolidBrush(BackgroundColor);

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
                        e.Graphics.FillRectangle(Dbrush, cellRect); ;
                    }

                    if (world == false)
                    {
                        count = CountNeighborsToroidal(x, y);
                    }
                    else
                    {
                        count = CountNeighborsFinite(x, y);
                    }
                    cellRules(count, x, y);

                    if (count > 0 && DisplayCount == true)
                    {
                        NeighborDisplay(e, x, y, count);
                    }
                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }
            // 10 by 10 grid display
            Grid10by10(e);
            if (HUD == true)
            {
                HUDelements(e, Lcell, generations, world);
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();

        }

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
        private void NeighborDisplay(PaintEventArgs e, int x, int y, int count)
        {
            Pen gridPen = new Pen(gridColor, 1);

            Brush cellBrush = new SolidBrush(cellColor);

            Brush DBrush = new SolidBrush(BackgroundColor);
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0) - 0.01f;
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1) - 0.01f;

            Font font = new Font("Arial", cellHeight * 0.7f);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;


            RectangleF rect = new RectangleF(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
            //int neighbors = 8;

            if (universe[x, y].GetCellState() == true)
            {
                e.Graphics.FillRectangle(cellBrush, rect);
            }
            else
            {
                e.Graphics.FillRectangle(DBrush, rect);
            }
            //Brush tBrush = new SolidBrush(cellColor);

            //e.Graphics.FillRectangle(cellBrush, rect);
            e.Graphics.DrawRectangle(gridPen, (x) * cellWidth, (y) * cellHeight, cellWidth, cellHeight);
            e.Graphics.DrawString((count).ToString(), font, tBrush, rect, stringFormat);

            gridPen.Dispose();
            cellBrush.Dispose();
            tBrush.Dispose();
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
            Font font = new Font("Arial", 12f, FontStyle.Bold);
            string s = (world == false) ? "Toroidal" : "Finite";
            string Hudtext = $"Generations: {generations}\nCell Count: {count}\nBoundary Type:{s}\nUniverse Size:(Width={universe.GetLength(0)}, Height={universe.GetLength(1)})";
            RectangleF rect = new RectangleF(0, 0, graphicsPanel1.ClientSize.Width, graphicsPanel1.ClientSize.Height);
            StringFormat stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Far;
            e.Graphics.DrawString(Hudtext, font, Brushes.Salmon, rect, stringFormat);
        } 
        #endregion

        //all Buttons
        #region All Button functionality
        //activate timer for the game
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Start();

            graphicsPanel1.Invalidate();
            Play.Enabled = false;
            pause.Enabled = true;
        }
        // click for the new gen
        private void next_Click(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }

        //clear grid/ new game
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            GridReset();
        }

        //pause game
        private void pause_Click(object sender, EventArgs e)
        {
            timer.Stop();
            pause.Enabled = false;
            Play.Enabled = true;
        }


        //finite view
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            world = true;
            finiteToolStripMenuItem.Checked = true;
            toriToolStripMenuItem.Checked = false;
            graphicsPanel1.Invalidate();
        }

        //toroidal view
        private void toriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            world = false;
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

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridControl();
        }


        //creates the random seed //will be updated later
        private void Randomize_Click(object sender, EventArgs e)
        {
            GridReset();
            Random Rint = new Random(DateTime.Now.Millisecond);
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
            graphicsPanel1.Invalidate();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridReset();
        }

        private void Options_Click(object sender, EventArgs e)
        {
            Options_Dialog opdlg = new Options_Dialog();
            if (DialogResult.OK == opdlg.ShowDialog())
            {
                universe = new CellState[Properties.Settings.Default.UniWidth, Properties.Settings.Default.UniHeight];
                setNextGenSize();
                arrayInit();

            }
            graphicsPanel1.Invalidate();
        }

        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hudControl();
        }


        private void hudToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            hudControl();
        }

        private void gridToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GridControl();
        }
        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundColor = CdialogControl(BackgroundColor);
            SaveChanges();
            graphicsPanel1.Invalidate();
        }
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cellColor = CdialogControl(cellColor);
            SaveChanges();
            graphicsPanel1.Invalidate();
        }
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = CdialogControl(gridColor);
            SaveChanges();
            graphicsPanel1.Invalidate();
        }

        private void gridx10ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grid10Color = CdialogControl(grid10Color);
            SaveChanges();
            graphicsPanel1.Invalidate();
        } 
        #endregion

        //control Groups// All created functions for different uses
        #region Control
        //grid control
        private void GridControl()
        {
            grid = !grid;
            gridToolStripMenuItem.Checked = grid;
            gridToolStripMenuItem1.Checked = grid;
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
            HUD = !HUD;
            hUDToolStripMenuItem.Checked = HUD;
            hudToolStripMenuItem1.Checked = HUD;
            graphicsPanel1.Invalidate();
        }
        //Grid reset control // for newing the grid
        private void GridReset()
        {
            timer.Stop();
            generations = 0;

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
            if (DialogResult.OK == Cdlg.ShowDialog() )
            {
                clr = Cdlg.Color;
            }
            return clr;
        }


        //Load in user settings on open
        private void Form1_Load(object sender, EventArgs e)
        {
            cellColor = KimTerryGameOfLife.Properties.Settings.Default.CellColor;
            gridColor = KimTerryGameOfLife.Properties.Settings.Default.GridColor;
            grid10Color = KimTerryGameOfLife.Properties.Settings.Default.Grid10Color;
            BackgroundColor = KimTerryGameOfLife.Properties.Settings.Default.BackgroundColor;
            Interval.Text = "Interval = " + Properties.Settings.Default.Interval.ToString();

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
        #endregion
    } 
}
