using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace chess
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form f4 = new Form4();
            Form f1 = new Form1();
            f4.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form f5 = new Form5();
            Form f6 = new Form6();
            f6.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form f7 = new Form7();
            Form f5 = new Form5();
            f7.Show();
            this.Close();
        }

        

      
    }
}
