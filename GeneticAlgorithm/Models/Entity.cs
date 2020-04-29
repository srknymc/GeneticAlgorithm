using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeneticAlgorithm.Models
{
    public enum ItemType
    {
        Wall = 0,
        Path = 1,
        Player = 2,
        Finish = 3,
        FakeWall = 4, //previous moves
    }
    public class GA
    {
        public int MaxMove { get; set; }
        public int PopulationSize { get; set; }
        public float MutationRate { get; set; }
        public List<ChromosomeEntity> Population { get; set; }
        public GA()
        {
            Population = new List<ChromosomeEntity>();
        }
        public void CreatePopulation()
        {
            Random rnd = new Random();
            for (int pop = 0; pop < PopulationSize; pop++)
            {
                ChromosomeEntity ch = new ChromosomeEntity();
                ch.Chromosome = new int[MaxMove];
                for (int i = 0; i < MaxMove; i++)
                {
                    ch.Chromosome[i] = rnd.Next(1,5);
                }
                Population.Add(ch);
            }
        }
        public bool Fitness(Maze maze, ChromosomeEntity ch)
        {
            bool isFinished = false;
            foreach (int move in ch.Chromosome)
            {
                switch (move)
                {
                    case 0:
                        break;
                    case 1:
                        maze.CurrentPlayer.MoveUp();
                        isFinished=maze.UpdateMaze();
                        break;
                    case 2:
                        maze.CurrentPlayer.MoveDown();
                        isFinished = maze.UpdateMaze();

                        break;
                    case 3:
                        maze.CurrentPlayer.MoveLeft();
                        isFinished = maze.UpdateMaze();

                        break;
                    case 4:
                        maze.CurrentPlayer.MoveRight();
                        isFinished = maze.UpdateMaze();

                        break;
                    default:
                        break;
                }
                //if (isFinished)
                //    break;

            }
            ch.Score = AppHelper.ManDistance(maze) + maze.CurrentPlayer.Penalties * 100;
            return isFinished;
        }

        public ChromosomeEntity GetBestScore(List<ChromosomeEntity> Population)
        {
            return Population.OrderByDescending(x => x.Score).FirstOrDefault();
        }
        public List<ChromosomeEntity> Crossover(List<ChromosomeEntity> FitPopulation)
        {
            Random rnd = new Random();
            List<ChromosomeEntity> newPopulation = new List<ChromosomeEntity>();
            for (int i = 0; i < PopulationSize; i++)
            {
                ChromosomeEntity parent = FitPopulation[rnd.Next(FitPopulation.Count)];
                ChromosomeEntity parent2 = FitPopulation[rnd.Next(FitPopulation.Count)];
                int crossoverIndex = rnd.Next(MaxMove);
                ChromosomeEntity child = new ChromosomeEntity();
                var newCh = new List<int>();
                newCh.AddRange(parent.Chromosome.Take(crossoverIndex));
                newCh.AddRange(parent2.Chromosome.Skip(crossoverIndex).Take(MaxMove - crossoverIndex));
                child.Chromosome = newCh.ToArray();
                child = Mutation(child);
                newPopulation.Add(child);
            }
            return newPopulation;
        }
        public ChromosomeEntity Mutation(ChromosomeEntity ch)
        {
            Random rnd = new Random();
            if (MutationRate > rnd.NextDouble()) 
            {
                //ch.Chromosome[rnd.Next(MaxMove)] = rnd.Next(1, 5);
                int genescount = new Random().Next(5);
                for (int i = 0; i < genescount; i++)
                {
                    ch.Chromosome[new Random().Next(MaxMove)] = new Random().Next(1, 5);
                }
            }
            return ch;
        }
    }
    public class ChromosomeEntity
    {
        public int[] Chromosome { get; set; }
        public int Score { get; set; }
        public ChromosomeEntity()
        {
            Chromosome = new int[30];
        }
    }
    [Serializable]
    public class Maze
    {
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public Dictionary<string, CustomControl> mapData { get; set; }
        public Maze()
        {
            mapData = new Dictionary<string, CustomControl>();
        }
        public int FinishRow { get; set; }
        public int FinishColumn { get; set; }
        public Player CurrentPlayer { get; set; }
        public bool UpdateMaze()
        {
            if (mapData[CurrentPlayer.NextPosition].Type == ItemType.Wall || mapData[CurrentPlayer.NextPosition].Type == ItemType.FakeWall)
            {
                CurrentPlayer.Penalties++;
                return false;
            }
            else if (mapData[CurrentPlayer.NextPosition].Type == ItemType.Path)
            {
                mapData[CurrentPlayer.CurrentPosition].Type = ItemType.FakeWall;
                mapData[CurrentPlayer.NextPosition].Type = ItemType.Player;
                CurrentPlayer.CurrentPosition = CurrentPlayer.NextPosition;
                CurrentPlayer.Step++;
                CurrentPlayer.setNextToCurrent();
                //MainForm.mainPanel.Invalidate();
                //Thread t1 = new Thread(() =>
                //  {
                //      MainForm.mainPanel.Invalidate();
                //      Thread.Sleep(300);
                //  });
                //t1.Start();
                //t1.Join();
                return false;
            }
            else
            {
                //mapData[CurrentPlayer.CurrentPosition].Type = ItemType.FakeWall;
                //mapData[CurrentPlayer.NextPosition].Type = ItemType.Player;
                CurrentPlayer.CurrentPosition = CurrentPlayer.NextPosition;
                CurrentPlayer.Step++;
                CurrentPlayer.setNextToCurrent();
                return true;
            }
        }
        public Maze DeepCopy()
        {

            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (Maze)formatter.Deserialize(ms);
            }

        }
        public void DrawMap(Panel pnlMain, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Transparent);
            pnlMain.Controls.Clear();
            foreach (CustomControl c in mapData.Values)
            {
                if (c.Type == ItemType.Wall)
                {
                    e.Graphics.FillRectangle(Brushes.Red, new RectangleF(c.Location, c.Size));
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));

                }
                else if (c.Type == ItemType.Path)
                {
                    e.Graphics.FillRectangle(Brushes.White, new RectangleF(c.Location, c.Size));
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));
                }
                else if (c.Type == ItemType.Player)
                {
                    e.Graphics.FillRectangle(Brushes.Green, new RectangleF(c.Location, c.Size));
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));
                }
                else if (c.Type == ItemType.Finish)
                {
                    e.Graphics.FillRectangle(Brushes.Yellow, new RectangleF(c.Location, c.Size));
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));
                }
                else if (c.Type == ItemType.FakeWall)
                {
                    e.Graphics.FillRectangle(Brushes.Purple, new RectangleF(c.Location, c.Size));
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));
                }
            }
        }
    }

    [Serializable]
    public class Player
    {
        public int CurrRow { get; set; }
        public int CurrCol { get; set; }
        public int Penalties { get; set; }
        public int Step { get; set; }
        public string NextPosition { get; set; }
        private string _currentPos;
        public string CurrentPosition { get { return PositionToString(CurrCol, CurrRow); } set { _currentPos = value; } }
        public void MoveUp()
        {
            NextPosition = PositionToString(CurrCol, CurrRow - 1);
        }
        public void MoveDown()
        {
            NextPosition = PositionToString(CurrCol, CurrRow + 1);

        }
        public void MoveRight()
        {
            NextPosition = PositionToString(CurrCol + 1, CurrRow);
        }
        public void MoveLeft()
        {
            NextPosition = PositionToString(CurrCol - 1, CurrRow);
        }
        public string PositionToString(int cCol, int cRow)
        {
            return $"{cCol},{cRow}";
        }
        public void setNextToCurrent()
        {
            string[] tmp = NextPosition.Split(',');
            CurrCol = Convert.ToInt32(tmp[0]);
            CurrRow = Convert.ToInt32(tmp[1]);
        }
    }
    [Serializable]
    public class CustomControl
    {

        public int col { get; set; }
        public int row { get; set; }
        public ItemType Type { get; set; }
        public Point Location { get; set; }
        public Size Size { get; set; }
        public override string ToString()
        {
            return $"{col},{row}";
        }
    }
}
