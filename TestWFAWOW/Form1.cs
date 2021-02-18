using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWFAWOW
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("GetDomainID : " + Thread.GetDomainID().ToString());
            MessageBox.Show("Normal ManagedThreadId : " + Thread.CurrentThread.ManagedThreadId);
            new Thread(() =>
            {
                MessageBox.Show("new Thread ManagedThreadId : " + Thread.CurrentThread.ManagedThreadId);
            }).Start();
            Task.Factory.StartNew(() =>
            {
                MessageBox.Show("Task.Factory.StartNew ManagedThreadId : " + Thread.CurrentThread.ManagedThreadId);
            });
        }
    }
}
