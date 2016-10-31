using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StratcolIntegration
{
    public partial class Form1 : Form
    {
        StratcolInterface.StratcolMain scInterface = new StratcolInterface.StratcolMain();
        public Form1()
        {
            InitializeComponent();
            scInterface.StratcolMainInit();
        }

        private void buttonCodes_Click(object sender, EventArgs e)
        {           
           scInterface.UpdateCodes();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            scInterface.UpdateOutputs(false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            scInterface.UpdateInputs();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            scInterface.UpdateAhv();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            scInterface.UpdateRejected();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            scInterface.UpdateAuths();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
            scInterface.ProcessResults();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            scInterface.SearchCancelled();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            scInterface.RecallBatch();
        }
    }
}
