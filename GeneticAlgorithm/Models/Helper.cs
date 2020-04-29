using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeneticAlgorithm.Models
{
    public static class AppHelper
    {
        //public static CustomControl GetControl(TableLayoutPanel panel,int col,int row)
        //{
        //   return (CustomControl)panel.Controls.Find($"custom_{col},{row}", true).FirstOrDefault();
        //}
        public static int ManDistance(Maze maze)
        {
            return Math.Abs(maze.CurrentPlayer.CurrCol - maze.FinishColumn) + Math.Abs(maze.CurrentPlayer.CurrRow - maze.FinishRow);
        }
    }
}
