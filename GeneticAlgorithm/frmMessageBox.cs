using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeneticAlgorithm
{
    public partial class frmMessageBox : Form
    {
        public new int DialogResult;
        public frmMessageBox()
        {
            InitializeComponent();
        }

        private void btnYatay_Click(object sender, EventArgs e)
        {
            DialogResult = 0;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = 1;
            this.Close();
        }
    }
}
