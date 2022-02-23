using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {
        int seed = 10;
        // The universe array
        bool[,] universe = new bool[10, 10];
        bool[,] scratchPad = new bool[10, 10];
        bool drawRec = true;
        bool drawNum = true;

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
        int allLiveCells = 0;


        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = true; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
           
            //Apply rules
            for (int y = 0; y < scratchPad.GetLength(1); y++)
            {
                for (int x = 0; x < scratchPad.GetLength(0); x++)
                {
                    int liveCells = CountNeighbors(x, y);

                    if (liveCells < 2)
                    {
                        scratchPad[x, y] = false;
                    }
                    //Dies
                    if (liveCells > 3)
                    {
                        scratchPad[x, y] = false;
                    }
                    //Lives
                    if (liveCells == 2 && (universe[x, y] == true))
                    {
                        scratchPad[x, y] = true;
                    }
                    if (liveCells == 3 && (universe[x, y] == true))
                    {
                        scratchPad[x, y] = true;
                    }
                    //Born
                    if (liveCells == 3)
                    {
                        scratchPad[x, y] = true;
                    }

                }
            }

            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "Living Cells: " + allLiveCells.ToString();
            //intervalStripStatusLabel.Text = "Interval: " + timer.Interval.ToString();
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }
      
        private void GraphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
           // Calculate the width and height of each cell in pixels
            // Currently fits to window - I subtracted 1 from clientsize.width/heigth
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = ((float)graphicsPanel1.ClientSize.Width - 1) / universe.GetLength(0); //returns length in deminsion 2D array so 0 1 for getlength

            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = ((float)graphicsPanel1.ClientSize.Height - 1) / universe.GetLength(1);

            int penWidth = 1;

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, penWidth);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    int neighbors = CountNeighbors(x, y);
                    string insideNum = "";
                    //show neighbors
                    Font font = new Font("Arial", 14f);

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    if(neighbors == 0)
                    {
                        insideNum = "";
                    }
                    if ( neighbors > 0 || (neighbors == 0 && universe[x,y]))
                    {
                        insideNum = neighbors.ToString();  
                    }
                    if (drawNum == true)
                    {
                        e.Graphics.DrawString(insideNum.ToString(), font, Brushes.Black, cellRect, stringFormat);

                    }

                    if (drawRec == true)
                    {
                        // Outline the cell with a pen
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                }
            }
            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();

            }

           
        

        private void GraphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                
                
                    // Toggle the cell's state
                    // error if clicked in the gap from resizing ..  fix or handle error
                    universe[x, y] = !universe[x, y];
                   

                    // Tell Windows you need to repaint ... tell windows you need refresh what you see in the window
                    toolStripStatusLabel1.Text = "Living Cells: " + allLiveCells.ToString();

                    graphicsPanel1.Invalidate();
                
            }
        } 
        public int CountNeighbors(int x, int y)
        {
            int neighborCount = 0;
            int xLength = universe.GetLength(0);
            int yLength = universe.GetLength(1);
            for (int yOff = -1; yOff <= 1; yOff++)
            {
                for (int xOff = -1; xOff <= 1; xOff++)
                {
                    int xC = x + xOff;
                    int yC = y + yOff;
                    if (xOff == 0 && yOff == 0)
                    {
                        continue;
                    }
                    if (yC < 0)
                    {
                        continue;
                    }
                    if(xC < 0)
                    {
                        continue;
                    }
                    if(xC >= xLength)
                    {
                        continue;
                    }
                    if(yC >= yLength)
                    {
                        continue;
                    }
                    if (universe[xC, yC] == true) neighborCount++;





                }
            }
            return neighborCount;

        }
        public void RandomUniverse()
        {
            generations = 0;
            allLiveCells = 0;

            Random randomNumber = new Random(seed);
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (randomNumber.Next(0, 3) == 0)
                    {
                        universe[x, y] = true;
                        ++allLiveCells;
                    }
                    else
                    {
                        universe[x, y] = false;
                    }
                }
            }
            toolStripStatusLabel1.Text = "Living Cells: " + allLiveCells.ToString();

            graphicsPanel1.Invalidate();

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration(); 
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void randomizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomUniverse();
        }
    }
}
