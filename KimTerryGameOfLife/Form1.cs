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

namespace KimTerryGameOfLife
{
    public partial class Form1 : Form
    {

        // The universe array
        CellState[,] universe = new CellState[5,5];
        

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        //next generation array
        CellState[,] NextGen; //this is the scratchpad

        //Set the next gen array size to the univer size 
        private void setNextGenSize()
        {
            NextGen = new CellState[universe.GetLength(0), universe.GetLength(1)];
        }


        public Form1()
        {
            InitializeComponent();
            setNextGenSize();
            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    universe[i, j] = new CellState();
                    NextGen[i, j] = new CellState();
                }
            }
            //***made change so timer does not automatically run***
            //timer.Enabled = true; // start timer running

        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {


            // Increment generation count
            generations++;
            swap();
            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
        }

        private void swap()
        {
            //bool[,] universe = new bool[5, 5];
            //bool[,] scratchPad = new bool[5, 5];

            // Swap them...
            CellState[,] temp = universe;
            universe = NextGen;
            NextGen = temp;
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
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
                    }
                    else
                    {
                        e.Graphics.FillRectangle(Brushes.White, cellRect);
                    }
                    int count = CountNeighborsFinite(x, y, e);
                    //Rectext(CountNeighborsFinite(x, y, e), e, x, y, gridPen, cellBrush);

                    if (count > 0)
                    {
                        test(e, x, y, count);
                    }
                    cellRules(count, x, y);
                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
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


        //rules for the Cell generation
        private void cellRules(int count, int x, int y)
        {
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
            if (universe[x,y].GetCellState() == true && count == 2 || count == 3)
            {
                //NextGen[x, y] = true;
                NextGen[x, y].SetLcells(true);
            }
            if (universe[x,y].GetCellState() == false && count == 3)
            {
                //NextGen[x, y] = true;
                NextGen[x, y].SetLcells(true);
            }


        }
        private void test(PaintEventArgs e, int x, int y, int count)
        {

            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0) - 0.01f;
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1) - 0.01f;

            Font font = new Font("Arial", 20f);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            RectangleF rect = new RectangleF(x*cellWidth, y*cellHeight, cellWidth, cellHeight);
            //int neighbors = 8;

            if (universe[x, y].GetCellState() == true)
            {
                e.Graphics.FillRectangle(cellBrush, rect);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, rect);
            }
            //e.Graphics.FillRectangle(cellBrush, rect);
            e.Graphics.DrawRectangle(gridPen, (x) * cellWidth, (y) * cellHeight, cellWidth, cellHeight);
            e.Graphics.DrawString((count).ToString(), font, Brushes.Black, rect, stringFormat);


            gridPen.Dispose();
            cellBrush.Dispose();
            graphicsPanel1.Invalidate();
        }
        //neighbor count display
        private void Rectext(int count, PaintEventArgs e, int x, int y, Pen gridpen, Brush cellBrush)
        {
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0) - 0.01f;
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1) - 0.01f;
            Font font = new Font("Arial", 20f);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            RectangleF rect = RectangleF.Empty;


            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    rect = new RectangleF((x + xOffset) * cellWidth, (y + yOffset) * cellHeight, cellWidth, cellHeight);
                    if (xOffset != 0 || yOffset != 0)
                    {
                        //if (universe[x, y] == true)
                        {
                            e.Graphics.FillRectangle(cellBrush, rect);
                            e.Graphics.DrawRectangle(gridpen, (x + xOffset) * cellWidth, (y + yOffset) * cellHeight, cellWidth, cellHeight);
                        }
                        //else
                        //{
                        //    e.Graphics.FillRectangle(cellBrush, rect);
                        //    e.Graphics.DrawRectangle(gridpen, (x + xOffset) * cellWidth, (y + yOffset) * cellHeight, cellWidth, cellHeight);
                        //}
                        e.Graphics.DrawString((count).ToString(), font, Brushes.Black, rect, stringFormat);
                    }
                }
            }
            //graphicsPanel1.Invalidate();
        }

        private void IniRandUniverse()
        {

        }


        private int CountNeighborsFinite(int x, int y, PaintEventArgs e)
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }
    }
}
