using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace chess
{

    public partial class Form6 : Form
    {


        public Form6()
        {
            InitializeComponent();
        }

       

        private void Form6_Load(object sender, EventArgs e)
        {

        }
        
        
        private void button1_Click(object sender, EventArgs e)
        {
            Form f6 = new Form6();
            Form f4 = new Form4();
            f4.Show();
            this.Close();
        }
    }

}
