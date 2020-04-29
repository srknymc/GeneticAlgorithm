using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeneticAlgorithm.Models;

namespace GeneticAlgorithm
{
    public partial class MainForm : Form
    {
        Maze defaultMaze;
        public static Panel mainPanel;
        byte Mode = 0; // 0->Draw Wall Mode, 1->Set Start Position, 2->Set Finish Position, 3->Start to Find Path
        int N = 20; // MapSize N*N
        Maze maze;
        Player player;
        public MainForm()
        {
            InitializeComponent();
            mainPanel = pnlMain;
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormSettings();
            AddHandlers();
        }
        private void createMap() //creating empty map N*N
        {
            Graphics g = pnlMain.CreateGraphics();
            float width = pnlMain.Size.Width;
            float height = pnlMain.Size.Height;
            int rec_width = Convert.ToInt32(pnlMain.Size.Width / N);
            int rec_height = Convert.ToInt32(pnlMain.Size.Height / N);
            for (int i = 0; i < N; i++)
            {

                for (int j = 0; j < N; j++)
                {
                    if (i == 0 || i == N - 1 || j == 0 || j == N - 1)//first,last row and first,last col set as wall(red)
                    {
                        CustomControl custom = new CustomControl { Type = ItemType.Wall, col = i, row = j, Size = new Size(rec_width, rec_height), Location = new Point(i * rec_width, j * rec_height) };
                        maze.mapData.Add($"{i},{j}", custom);
                    }
                    else //others set as path(white)
                    {
                        CustomControl custom = new CustomControl { Type = ItemType.Path, col = i, row = j, Size = new Size(rec_width, rec_height), Location = new Point(i * rec_width, j * rec_height) };
                        maze.mapData.Add($"{i},{j}", custom);
                    }
                }
            }
        }
        private void FormSettings()
        {
            this.DoubleBuffered = true; //for better performance
            player = new Player() { NextPosition = "0,0" }; //creating player
            maze = new Maze() { RowCount = N, ColumnCount = N, CurrentPlayer = player }; //creating maze with given values 
            createMap();
            MessageBox.Show("Program içerisinde F1 e bir kere bastıktan sonra istenilen yerlere duvar eklenebilir.\n F2 ye bir kere bastıktan sonra istenilen nokta seçilerek oyuncunun başlangıç noktası belirlenir.\n " +
                "F3 e bir kere bastıktan sonra haritanın bitiş noktası belirlenir!\n F4 e basınca ise algoritma çalışmaya başlar!(Freeze olabilir)\n F5 ise algoritma bittiği zaman basmanız halinde haritayı sıfırlar!");
        }
        private void AddHandlers()
        {
            pnlMain.Paint += MainForm_Paint;
            pnlMain.MouseDown += PnlMain_MouseDown;
            this.KeyDown += MainForm_KeyDown;
            this.Paint += MainForm_Paint;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                Mode = 0;
            }
            else if (e.KeyCode == Keys.F2)
            {
                Mode = 1;
            }
            else if (e.KeyCode == Keys.F3)
            {
                Mode = 2;
            }
            else if (e.KeyCode == Keys.F4)
            {
                Mode = 3;
                StartToFind();
            }
            else if (e.KeyCode == Keys.F5)
            {
                maze = defaultMaze.DeepCopy();
                pnlMain.Invalidate();
            }
            //For Debugging
            //else if (e.KeyCode == Keys.Down)
            //{
            //    maze.CurrentPlayer.MoveDown();
            //    maze.UpdateMaze();
            //}
            //else if (e.KeyCode == Keys.Right)
            //{
            //    maze.CurrentPlayer.MoveRight();
            //    maze.UpdateMaze();
            //}
            //else if (e.KeyCode == Keys.Up)
            //{
            //    maze.CurrentPlayer.MoveUp();
            //    maze.UpdateMaze();
            //}
            //else if (e.KeyCode == Keys.Left)
            //{
            //    maze.CurrentPlayer.MoveLeft();
            //    maze.UpdateMaze();
            //}
        }

        private void PnlMain_MouseDown(object sender, MouseEventArgs e)
        {
            CustomControl customControl = new CustomControl();
            //Find Clicked Cell
            foreach (CustomControl x in maze.mapData.Values)
            {
                if (e.Y > x.Location.Y && e.Y < (x.Location.Y + x.Size.Height) && e.X > x.Location.X && e.X < (x.Location.X + x.Size.Width))
                {
                    customControl = x;
                    break;
                }
            }
            if (Mode == 0)//Wall adding Mode
            {
                frmMessageBox form = new frmMessageBox();
                form.ShowDialog(); // asking for horizontal or vertical
                int result = form.DialogResult;
                int col = customControl.col;
                int row = customControl.row;
                if (result == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        maze.mapData[$"{col++},{row}"].Type = ItemType.Wall; //adding wall to map
                    }
                }
                else if (result == 1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (col < maze.ColumnCount && row < maze.RowCount)
                        {
                            maze.mapData[$"{col},{row++}"].Type = ItemType.Wall;

                        }
                    }
                }
                pnlMain.Invalidate();
            }
            else if (Mode == 1) //Set Player
            {
                maze.mapData[customControl.ToString()].Type = ItemType.Player;
                maze.CurrentPlayer.CurrCol = customControl.col;
                maze.CurrentPlayer.CurrRow = customControl.row;
                pnlMain.Invalidate();

            }
            else if (Mode == 2) //Set Finish
            {
                maze.mapData[customControl.ToString()].Type = ItemType.Finish;
                maze.FinishRow = customControl.row;
                maze.FinishColumn = customControl.col;
                pnlMain.Invalidate();

            }

        }
        //Main GA logic funtion
        private void StartToFind()
        {
            bool isFound = false;
            defaultMaze = maze.DeepCopy();
            GA ga = new GA() { MaxMove = N * 3, PopulationSize = 100, MutationRate = 0.6f };
            List<ChromosomeEntity> best = new List<ChromosomeEntity>();
            ga.CreatePopulation(); //Creating First Population
            int generationCount = 1;
            ChromosomeEntity tmp = new ChromosomeEntity();
            int bestscore = int.MaxValue;
            do
            {
                List<ChromosomeEntity> fitness = new List<ChromosomeEntity>();
                foreach (ChromosomeEntity ch in ga.Population)
                {
                    if (ga.Fitness(maze, ch) == -1) //fitness for generation untill arrive finish
                    {
                        isFound = true;
                        tmp = ch;
                        best.Add(ch.DeepCopy());
                        break;
                    }
                    fitness.Add(ch); //adding chromosome to list with scores
                    maze = defaultMaze.DeepCopy(); //settings for next generation
                }
                if (!isFound) //if is not found
                {
                    var sorted = fitness.OrderBy(x => x.Score).ToList(); //sort list by Score
                    var newpop = ga.Crossover(sorted.Take(sorted.Count / 2).ToList(), defaultMaze.DeepCopy()); //sending half fit ch for next generation
                    bestscore = sorted[0].Score;
                    tmp = sorted[0]; //to show bestfit chromosome
                    ga.Population = newpop;
                }
                //showing best fit in generation
                Thread t1 = new Thread(() =>
                {
                    ga.Fitness(maze, tmp);
                    pnlMain.Invalidate();
                    maze = defaultMaze.DeepCopy();
                });
                t1.Start();
                t1.Join();
                generationCount++;
            } while (!isFound);

            MessageBox.Show("Path found! Generation Count=" + generationCount.ToString());

        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (sender.GetType() == mainPanel.GetType())
                maze.DrawMap(((Panel)sender), e);

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            pnlMain.Invalidate();
        }

    }
}
