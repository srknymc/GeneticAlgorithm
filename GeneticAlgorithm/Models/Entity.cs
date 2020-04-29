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
        public int MaxMove { get; set; } //Chromosome Lenght
        public int PopulationSize { get; set; } // Population Size
        public float MutationRate { get; set; } // Stands for Mutation Chance
        public List<ChromosomeEntity> Population { get; set; } // List of Population
        public GA()
        {
            Population = new List<ChromosomeEntity>();
        }
        public void CreatePopulation()
        {
            Random rnd = new Random();
            //Generation population
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
        public int Fitness(Maze maze, ChromosomeEntity ch)
        {
            bool isFound = false;
            //iterate over chromosome's movement array
            foreach (int move in ch.Chromosome)
            {
                switch (move)
                {
                    case 0:
                        break;
                    case 1:
                        maze.CurrentPlayer.MoveUp();
                        isFound=maze.UpdateMaze();
                        break;
                    case 2:
                        maze.CurrentPlayer.MoveDown();
                        isFound = maze.UpdateMaze();

                        break;
                    case 3:
                        maze.CurrentPlayer.MoveLeft();
                        isFound = maze.UpdateMaze();

                        break;
                    case 4:
                        maze.CurrentPlayer.MoveRight();
                        isFound = maze.UpdateMaze();
                        break;
                    default:
                        break;
                }
                if (isFound)
                    break;
            }
            if (!isFound)
                ch.Score = AppHelper.ManDistance(maze) + maze.CurrentPlayer.Penalties * 100;
            else
                ch.Score = -1;
            return ch.Score;
        }

        public ChromosomeEntity GetBestScore(List<ChromosomeEntity> Population)
        {
            return Population.OrderByDescending(x => x.Score).FirstOrDefault();
        }
        public List<ChromosomeEntity> Crossover(List<ChromosomeEntity> FitPopulation,Maze defaultMaze)
        {
            Random rnd = new Random();
            List<ChromosomeEntity> newPopulation = new List<ChromosomeEntity>(); //for next generation
            for (int i = 0; i < PopulationSize; i++)
            {
                ChromosomeEntity parent = FitPopulation[rnd.Next(FitPopulation.Count)]; //find a parent from prev generation
                ChromosomeEntity parent2 = FitPopulation[rnd.Next(FitPopulation.Count)];//find other parent
                int crossoverIndex = rnd.Next(MaxMove);
                ChromosomeEntity child = new ChromosomeEntity();
                var newCh = new List<int>();
                newCh.AddRange(parent.Chromosome.Take(crossoverIndex));//taking genes from parent1 
                newCh.AddRange(parent2.Chromosome.Skip(crossoverIndex).Take(MaxMove - crossoverIndex));//genes from parent2
                child.Chromosome = newCh.ToArray();
                child = Mutation(child); //Mutation func for new child

                //to make better new generation if child has worst score keep adding parents to new generation
                int p1 = Fitness(defaultMaze.DeepCopy(), parent);
                int p2 = Fitness(defaultMaze.DeepCopy(), parent2);
                int c1 = Fitness(defaultMaze.DeepCopy(), child);
                if (c1 != -1 || p1 != -1 || p2 != -1)
                {
                    if (p1 <= c1 && p1 <= p2)
                    {
                        newPopulation.Add(parent);
                    }
                    else if (c1 < p2)
                    {
                        newPopulation.Add(child);
                    }
                    else
                    {
                        newPopulation.Add(parent2);
                    }
                }
                
            }
            return newPopulation;
        }
        public ChromosomeEntity Mutation(ChromosomeEntity ch)
        {
            Random rnd = new Random();
            if (MutationRate > rnd.NextDouble()) 
            {
                //ch.Chromosome[rnd.Next(MaxMove)] = rnd.Next(1, 5); //single gene mutation
                int genescount = new Random().Next(5); //to mutate genes count
                for (int i = 0; i < genescount; i++)
                {
                    ch.Chromosome[rnd.Next(MaxMove)] = rnd.Next(1, 5); //new genes value
                }
            }
            return ch;
        }
    }
    [Serializable]
    public class ChromosomeEntity
    {
        public int[] Chromosome { get; set; } //movement array
        public int Score { get; set; } // score
        public ChromosomeEntity()
        {
            Chromosome = new int[30];
        }
        public ChromosomeEntity DeepCopy()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (ChromosomeEntity)formatter.Deserialize(ms);
            }
        }
    }
    [Serializable]
    public class Maze
    {
        public int RowCount { get; set; } //Maze row count
        public int ColumnCount { get; set; } // Maze column count
        public Dictionary<string, CustomControl> mapData { get; set; } // mapdata includes walls,start,finish,player's prev moves
        public Maze()
        {
            mapData = new Dictionary<string, CustomControl>();
        }
        public int FinishRow { get; set; } //Finish point
        public int FinishColumn { get; set; }//Finish point
        public Player CurrentPlayer { get; set; } //Current Chromosome on map
        public bool UpdateMaze()
        {
            if (mapData[CurrentPlayer.NextPosition].Type == ItemType.Wall || mapData[CurrentPlayer.NextPosition].Type == ItemType.FakeWall) //checking player's next movement. if its wall adding it penalty score
            {
                CurrentPlayer.Penalties++;
                return false;
            }
            else if (mapData[CurrentPlayer.NextPosition].Type == ItemType.Path) //if next movement is free to go, prev location sets as fakewall and updating player location
            {
                mapData[CurrentPlayer.CurrentPosition].Type = ItemType.FakeWall;
                mapData[CurrentPlayer.NextPosition].Type = ItemType.Player;
                CurrentPlayer.CurrentPosition = CurrentPlayer.NextPosition;
                CurrentPlayer.setNextToCurrent();
                return false;
            }
            else //if it arrived finished
            {
                //CurrentPlayer.CurrentPosition = CurrentPlayer.NextPosition;
                //CurrentPlayer.setNextToCurrent();
                return true;
            }
        }
        public Maze DeepCopy() //For saving initial data
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (Maze)formatter.Deserialize(ms);
            }
        }
        public void DrawMap(Panel pnlMain, PaintEventArgs e) //drawing map function
        {
            e.Graphics.Clear(Color.Transparent);
            pnlMain.Controls.Clear();
            foreach (CustomControl c in mapData.Values) //iterate over mapdata
            {
                if (c.Type == ItemType.Wall) //if wall set as red
                {
                    e.Graphics.FillRectangle(Brushes.Red, new RectangleF(c.Location, c.Size));
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));

                }
                else if (c.Type == ItemType.Path)//if path set as white
                {
                    e.Graphics.FillRectangle(Brushes.White, new RectangleF(c.Location, c.Size));
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));
                }
                else if (c.Type == ItemType.Player) //if player set as green
                {
                    e.Graphics.FillRectangle(Brushes.Green, new RectangleF(c.Location, c.Size));
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));
                }
                else if (c.Type == ItemType.Finish) //if finish set as yellow
                {
                    e.Graphics.FillRectangle(Brushes.Yellow, new RectangleF(c.Location, c.Size));
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(c.Location, c.Size));
                }
                else if (c.Type == ItemType.FakeWall) //if fakewalls(prev movements) set as purple
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
        public int CurrRow { get; set; } //player location row id
        public int CurrCol { get; set; }//player location col id
        public int Penalties { get; set; } //player's penalty score
        public int Step { get; set; } //player's steps count
        public string NextPosition { get; set; } //player's nextmovement(checking in Maze.UpdateMaze() func)
        private string _currentPos;
        public string CurrentPosition { get { return PositionToString(CurrCol, CurrRow); } set { _currentPos = value; } } //player's current location for dictionary key 
        
        //player's movement logic
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
    public class CustomControl //Control object for map entities
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
