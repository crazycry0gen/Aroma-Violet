using System;
using System.IO;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        private string _path = @"C:\CorpSMS";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CorpSMSServiceAssembly.SMSService.SendSMS("0827648572", "StratCorp Single Test", 5, 1); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //CorpSMSServiceAssembly.SMSService.SendQueuedMessages(textBox1.Text);
            CorpSMSServiceAssembly.SMSService.SendAllQueuedMessages();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CorpSMSServiceAssembly.SMSService.RefreshDeliveryReport();
        }

    }
}
