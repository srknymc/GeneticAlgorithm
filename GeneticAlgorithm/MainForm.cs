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
        bool flag = true;
        public static Panel mainPanel;
        byte Mode = 0; // 0->Draw Wall Mode, 1->Set Start Position, 2->Set Finish Position, 3->Start to Find Path
        int N = 10;
        //Dictionary<string, CustomControl> cList = new Dictionary<string, CustomControl>();
        //List<CustomControl> list = new List<CustomControl>();
        Maze maze;
        Player player;
        //GeneticAlgorithmSettings GA;
        List<int[]> gList = new List<int[]>();
        //TableLayoutPanel tableLayout = new TableLayoutPanel();
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
        private void createMap()
        {
            //e.Graphics.Clear(Color.Transparent);
            //pnlMain.Controls.Clear();
            Graphics g = pnlMain.CreateGraphics();
            //float width = g.VisibleClipBounds.Width;
            //float height = g.VisibleClipBounds.Height;
            float width = pnlMain.Size.Width;
            float height = pnlMain.Size.Height;
            int rec_width = Convert.ToInt32(pnlMain.Size.Width / N);
            int rec_height = Convert.ToInt32(pnlMain.Size.Height / N);
            for (int i = 0; i < N; i++)
            {

                for (int j = 0; j < N; j++)
                {


                    if (i == 0 || i == N - 1 || j == 0 || j == N - 1)
                    {

                        //tableLayout.Controls.Add(new CustomControl { col = i, row = j, Name = $"custom_{i},{j}", Dock = DockStyle.Fill, BackColor = Color.Red, Margin = new Padding(0) }, i, j);
                        //e.Graphics.FillRectangle(Brushes.Red, i * rec_width, j * rec_height, rec_width, rec_height);
                        //e.Graphics.DrawRectangle(Pens.Black, i * rec_width, j * rec_height, rec_width, rec_height);
                        CustomControl custom = new CustomControl { Type = ItemType.Wall, col = i, row = j, Size = new Size(rec_width, rec_height), Location = new Point(i * rec_width, j * rec_height) };
                        //custom.MouseDown += C_MouseClick;
                        //pnlMain.Controls.Add(custom);
                        maze.mapData.Add($"{i},{j}", custom);
                        //list.Add(custom);
                    }
                    else
                    {
                        //e.Graphics.FillRectangle(Brushes.White, i * rec_width, j * rec_height, rec_width, rec_height);
                        //e.Graphics.DrawRectangle(Pens.Black, i * rec_width, j * rec_height, rec_width, rec_height);
                        //pnlMain.Controls.Add(new CustomControl { col = i, row = j, Name = $"custom_{i},{j}",Location=new Point(i,j)});
                        CustomControl custom = new CustomControl { Type = ItemType.Path, col = i, row = j, Size = new Size(rec_width, rec_height), Location = new Point(i * rec_width, j * rec_height) };
                        maze.mapData.Add($"{i},{j}", custom);
                        //list.Add(custom);
                    }
                    //tableLayout.Controls.Add(new CustomControl { col = i, row = j, Name = $"custom_{i},{j}", Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0) }, i, j);

                }
            }
        }
        private void FormSettings()
        {
            //CheckForIllegalCrossThreadCalls = false;
            this.DoubleBuffered = true;
            player = new Player() { NextPosition = "0,0" };
            maze = new Maze() { RowCount = N, ColumnCount = N, CurrentPlayer = player };
            player.CurrRow = player.CurrCol = 1;

            //pnlMain.VerticalScroll.Enabled = true;
            //pnlMain.VerticalScroll.Visible = true;

            //pnlMain.AutoScroll = true;
            createMap();
            //tableLayout.SuspendLayout();
            //tableLayout.RowStyles.Clear();
            //tableLayout.ColumnStyles.Clear();
            //tableLayout.RowCount = N;
            //tableLayout.ColumnCount = N;
            ////tableLayout.AutoSize = true;
            //tableLayout.Dock = DockStyle.Fill;

            //for (int i = 0; i < N; i++)
            //{
            //    tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100/N));
            //}

            //for (int i = 0; i < N; i++)
            //{
            //    tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent,100/N));
            //}
            //for (int i = 0; i < N; i++)
            //{
            //    for (int j = 0; j < N; j++)
            //    {
            //        if (i == 0 || i == N - 1 || j == 0 || j == N - 1)
            //            tableLayout.Controls.Add(new CustomControl { col=i,row=j,Name=$"custom_{i},{j}" ,Dock = DockStyle.Fill, BackColor = Color.Red, Margin = new Padding(0) }, i, j);
            //        else
            //            tableLayout.Controls.Add(new CustomControl { col=i,row=j,Name = $"custom_{i},{j}", Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0) }, i, j);

            //    }
            //}
            //mainPanel.Controls.Add(tableLayout);
            //tableLayout.ResumeLayout();
        }

        private void TableLayout_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            //if (e.Column == 0 || e.Column==N-1 || e.Row==0 || e.Row==N-1)
            //{
            //    e.Graphics.FillRectangle(Brushes.Red, e.CellBounds);
            //}
            var panel = sender as TableLayoutPanel;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            var rectangle = e.CellBounds;
            using (var pen = new Pen(Color.Black, 1))
            {
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                if (e.Row == (panel.RowCount - 1))
                {
                    rectangle.Height -= 1;
                }

                if (e.Column == (panel.ColumnCount - 1))
                {
                    rectangle.Width -= 1;
                }

                e.Graphics.DrawRectangle(pen, rectangle);
            }
        }

        private void AddHandlers()
        {
            //this.SizeChanged += Form1_SizeChanged;
            //tableLayout.CellPaint += TableLayout_CellPaint;
            //tableLayout.MouseClick += TableLayout_MouseClick;
            pnlMain.Paint += MainForm_Paint;
            pnlMain.MouseDown += PnlMain_MouseDown;
            this.KeyDown += MainForm_KeyDown;
            //this.Paint += MainForm_Paint;
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
            }
            else if (e.KeyCode == Keys.Down)
            {
                maze.CurrentPlayer.MoveDown();
                maze.UpdateMaze();
            }
            else if (e.KeyCode == Keys.Right)
            {
                maze.CurrentPlayer.MoveRight();
                maze.UpdateMaze();
            }
            else if (e.KeyCode == Keys.Up)
            {
                maze.CurrentPlayer.MoveUp();
                maze.UpdateMaze();
            }
            else if (e.KeyCode == Keys.Left)
            {
                maze.CurrentPlayer.MoveLeft();
                maze.UpdateMaze();
            }
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
            if (Mode == 0)
            {

                //MessageBox.Show(customControl.ToString());
                frmMessageBox form = new frmMessageBox();
                form.ShowDialog();
                int result = form.DialogResult;
                int col = customControl.col;
                int row = customControl.row;
                if (result == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        maze.mapData[$"{col++},{row}"].Type = ItemType.Wall;

                        //pnlMain.CreateGraphics().FillRectangle(Brushes.Red, tmp.Location.X, tmp.Location.Y, tmp.Size.Width, tmp.Size.Height);
                        //pnlMain.CreateGraphics().DrawRectangle(Pens.Black, tmp.Location.X, tmp.Location.Y, tmp.Size.Width, tmp.Size.Height);
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
                        //tmp = (CustomControl)pnlMain.Controls[$"custom_{col},{row++}"];
                        //tmp.BackColor = Color.Red;
                        //pnlMain.CreateGraphics().FillRectangle(Brushes.Red, tmp.Location.X, tmp.Location.Y, tmp.Size.Width, tmp.Size.Height);
                        //pnlMain.CreateGraphics().DrawRectangle(Pens.Black, tmp.Location.X, tmp.Location.Y, tmp.Size.Width, tmp.Size.Height);
                    }

                }

                pnlMain.Invalidate();
            }
            else if (Mode == 1)
            {
                maze.mapData[customControl.ToString()].Type = ItemType.Player;
                maze.CurrentPlayer.CurrCol = customControl.col;
                maze.CurrentPlayer.CurrRow = customControl.row;
                pnlMain.Invalidate();

            }
            else if (Mode == 2)
            {
                maze.mapData[customControl.ToString()].Type = ItemType.Finish;
                maze.FinishRow = customControl.row;
                maze.FinishColumn = customControl.col;
                pnlMain.Invalidate();

            }
            else if (Mode == 3)
            {
                bool isFinished = false;
                Maze defaultMaze = maze.DeepCopy();
                GA ga = new GA() { MaxMove = AppHelper.ManDistance(maze), PopulationSize = 100, MutationRate = 0.7f };
                ga.CreatePopulation();
                ChromosomeEntity tmp = new ChromosomeEntity(); ;
                int bestscore = int.MaxValue;
                int prevscore = bestscore;
                List<ChromosomeEntity> prevList = new List<ChromosomeEntity>();
                do
                {
                    List<ChromosomeEntity> fitness = new List<ChromosomeEntity>();
                    foreach (ChromosomeEntity ch in ga.Population)
                    {
                        isFinished = ga.Fitness(maze, ch);
                        fitness.Add(ch);
                        maze = defaultMaze.DeepCopy();
                    }
                    //var sorted = fitness.OrderBy(x => x.Score).ToList();
                    //var newpop = ga.Crossover(sorted.Take(Convert.ToInt32(sorted.Count / 2)).ToList());
                    
                    if (prevscore < ga.Population.OrderBy(x=>x.Score).First().Score)
                    {
                        var sorted = prevList;
                        var newpop = ga.Crossover(sorted.Take(sorted.Count/2).ToList());
                        bestscore = sorted[0].Score;
                        tmp = sorted[0];
                        prevscore = bestscore;
                        ga.Population = newpop;

                    }
                    else
                    {
                        prevList = fitness.OrderBy(x => x.Score).ToList();

                        var sorted = fitness.OrderBy(x => x.Score).ToList();
                        var newpop = ga.Crossover(sorted.Take(sorted.Count/2).ToList());
                        bestscore = sorted[0].Score;
                        tmp = sorted[0];
                        prevscore = bestscore;
                        ga.Population = newpop;

                    }

                    Thread t1 = new Thread(() =>
                      {
                          ga.Fitness(maze, tmp);
                          pnlMain.Invalidate();
                          //Thread.Sleep(100);
                          maze = defaultMaze.DeepCopy();

                      });
                    t1.Start();
                    t1.Join();
                } while (bestscore >= 1);
                //ga.Fitness(maze, tmp);
                //pnlMain.Invalidate();
            }

        }
        private void StartGame(int BestScore, GA ga, Maze defaultMaze)
        {
            //if (BestScore <= 1)
            //{
            //    MessageBox.Show("sonuç bulundu");
            //}
            //else
            //{

            //    StartGame(best, ga, maze);
            //}

            //Thread t1 = new Thread(()=>{
            //    maze.CurrentPlayer.MoveDown();
            //    maze.UpdateMaze();
            //    Thread.Sleep(2000);
            //});
            //t1.Start();
            //t1.Join();
            //Thread t2 = new Thread(() => {
            //    maze.CurrentPlayer.MoveDown();
            //    maze.UpdateMaze();

            //});
            //t2.Start();
            //t2.Join();


            //foreach (ChromosomeEntity ch in ga.Population)
            //{
            //    ga.Fitness(maze, ch);
            //    maze = maze2.DeepCopy();
            //}
            //ga.Population.OrderBy(x => x.Score);
            //ga.Fitness(maze,ga.Population[0]);
            //pnlMain.Invalidate();


        }
        private int Fitness(int[] chromosome)
        {
            int score = 0;
            foreach (int move in chromosome)
            {
                switch (move)
                {
                    case 0:
                        break;
                    case 1:
                        maze.CurrentPlayer.MoveUp();
                        break;
                    case 2:
                        maze.CurrentPlayer.MoveDown();
                        break;
                    case 3:
                        maze.CurrentPlayer.MoveLeft();
                        break;
                    case 4:
                        maze.CurrentPlayer.MoveRight();
                        break;
                    default:
                        break;
                }
            }
            score = Math.Abs(maze.CurrentPlayer.CurrCol - maze.FinishColumn) + Math.Abs(maze.CurrentPlayer.CurrRow - maze.FinishRow) + maze.CurrentPlayer.Penalties;
            return score;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            maze.DrawMap(((Panel)sender), e);

            //e.Graphics.Clear(Color.Transparent);
            //pnlMain.Controls.Clear();
            //foreach (CustomControl c in cList.Values)
            //{
            //    if (c.Type == ItemType.Wall)
            //    {
            //        e.Graphics.FillRectangle(Brushes.Red, new RectangleF(c.Location, c.Size));
            //        e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));

            //    }
            //    else if (c.Type == ItemType.Path)
            //    {
            //        e.Graphics.FillRectangle(Brushes.White, new RectangleF(c.Location, c.Size));
            //        e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));
            //    }
            //    else if (c.Type == ItemType.Player)
            //    {
            //        e.Graphics.FillRectangle(Brushes.Green, new RectangleF(c.Location, c.Size));
            //        e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));
            //    }
            //}
            //int width = e.ClipRectangle.Width;
            //int height = e.ClipRectangle.Height;
            //int rec_width = width / N;
            //int rec_height = height / N;
            //pnlMain.SuspendLayout();
            //for (int i = 0; i < N; i++)
            //{

            //    for (int j = 0; j < N; j++)
            //    {


            //        if (i == 0 || i == N - 1 || j == 0 || j == N - 1)
            //        {

            //            //tableLayout.Controls.Add(new CustomControl { col = i, row = j, Name = $"custom_{i},{j}", Dock = DockStyle.Fill, BackColor = Color.Red, Margin = new Padding(0) }, i, j);
            //            e.Graphics.FillRectangle(Brushes.Red, i * rec_width, j * rec_height, rec_width, rec_height);
            //            e.Graphics.DrawRectangle(Pens.Black, i * rec_width, j * rec_height, rec_width, rec_height);
            //            CustomControl custom = new CustomControl { Type = ItemType.Wall, col = i, row = j, Name = $"custom_{i},{j}", Size = new Size(rec_width, rec_height), Location = new Point(i * rec_width, j * rec_height) };
            //            custom.MouseDown += C_MouseClick;
            //            //pnlMain.Controls.Add(custom);
            //            list.Add(custom);
            //        }
            //        else
            //        {
            //            e.Graphics.FillRectangle(Brushes.White, i * rec_width, j * rec_height, rec_width, rec_height);
            //            e.Graphics.DrawRectangle(Pens.Black, i * rec_width, j * rec_height, rec_width, rec_height);
            //            //pnlMain.Controls.Add(new CustomControl { col = i, row = j, Name = $"custom_{i},{j}",Location=new Point(i,j)});
            //            CustomControl custom = new CustomControl { Type = ItemType.Path, col = i, row = j, Name = $"custom_{i},{j}", Size = new Size(rec_width, rec_height), Location = new Point(i * rec_width, j * rec_height) };
            //            custom.MouseDown += C_MouseClick;
            //            //pnlMain.Controls.Add(custom);
            //            list.Add(custom);
            //        }
            //        //tableLayout.Controls.Add(new CustomControl { col = i, row = j, Name = $"custom_{i},{j}", Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0) }, i, j);

            //    }
            //}
            //pnlMain.ResumeLayout();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            pnlMain.Invalidate();
        }
        private void C_MouseClick(object sender, MouseEventArgs e)
        {
            //CustomControl customControl = sender as CustomControl;
            ////MessageBox.Show(customControl.ToString());
            //CustomControl tmp = new CustomControl();
            //frmMessageBox form = new frmMessageBox();
            //form.ShowDialog();
            //int result = form.DialogResult;
            //int col = customControl.col;
            //int row = customControl.row;
            //if (result == 0)
            //{
            //    for (int i = 0; i < 4; i++)
            //    {
            //        ((CustomControl)pnlMain.Controls[$"custom_{col++},{row}"]).Type = ItemType.Wall;

            //        //pnlMain.CreateGraphics().FillRectangle(Brushes.Red, tmp.Location.X, tmp.Location.Y, tmp.Size.Width, tmp.Size.Height);
            //        //pnlMain.CreateGraphics().DrawRectangle(Pens.Black, tmp.Location.X, tmp.Location.Y, tmp.Size.Width, tmp.Size.Height);
            //    }
            //}
            //else if (result == 1)
            //{
            //    for (int i = 0; i < 4; i++)
            //    {
            //        ((CustomControl)pnlMain.Controls[$"custom_{col},{row++}"]).Type = ItemType.Wall;
            //        //tmp = (CustomControl)pnlMain.Controls[$"custom_{col},{row++}"];
            //        //tmp.BackColor = Color.Red;
            //        //pnlMain.CreateGraphics().FillRectangle(Brushes.Red, tmp.Location.X, tmp.Location.Y, tmp.Size.Width, tmp.Size.Height);
            //        //pnlMain.CreateGraphics().DrawRectangle(Pens.Black, tmp.Location.X, tmp.Location.Y, tmp.Size.Width, tmp.Size.Height);
            //    }

            //}
            //pnlMain.Invalidate();
        }
        //Point? GetRowColIndex(Point point)
        //{
        //    if (point.X > tableLayout.Width || point.Y > tableLayout.Height)
        //        return null;

        //    int w = tableLayout.Width;
        //    int h = tableLayout.Height;
        //    int[] widths = tableLayout.GetColumnWidths();

        //    int i;
        //    for (i = widths.Length - 1; i >= 0 && point.X < w; i--)
        //        w -= widths[i];
        //    int col = i + 1;

        //    int[] heights = tableLayout.GetRowHeights();
        //    for (i = heights.Length - 1; i >= 0 && point.Y < h; i--)
        //        h -= heights[i];

        //    int row = i + 1;

        //    return new Point(col, row);
        //}
    }
}
