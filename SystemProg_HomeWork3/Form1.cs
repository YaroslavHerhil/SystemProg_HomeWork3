using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemProg_HomeWork3
{
    public partial class Form1 : Form
    {
        Thread primeNumberGeneratorThread = null;
        Thread fibonacciNumberGeneratorThread = null;
        ManualResetEvent mre1;
        ManualResetEvent mre2;


        public Form1()
        {
            mre1 = new ManualResetEvent(true);
            mre2 = new ManualResetEvent(true);
            InitializeComponent();
        }


        private void GeneratePrimeNumbers(object parameters)
        {
            int[] boundaries = (int[])parameters;
            int head = boundaries[0];
            int tail = boundaries[1];

            int numberBuffer = head;
            while (numberBuffer <= tail || tail < 0) 
            {
                bool isPrime = true;
                int divisorCounter = 0;

                mre1.WaitOne();
                for (int i = 1; i <= numberBuffer; i++) {
                    if (numberBuffer % i == 0)
                        divisorCounter++;
                    if(divisorCounter > 2)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    BeginInvoke(new Action(() => { richTextBox1.AppendText(numberBuffer + " ");}));
                    
                }
                Thread.Sleep(1);

                numberBuffer++;
            }

            BeginInvoke(new Action(() =>
            {
                buttonStop1.Enabled = false;
                buttonPause1.Enabled = false;
                buttonStart1.Enabled = true;
                primeNumberGeneratorThread.Abort();
            }));
        }
        private void GenerateFibonacciNumbers(object parameters)
        {
            int[] boundaries = (int[])parameters;
            int head = boundaries[0];
            int tail = boundaries[1];

            int twoStepsBehind = 0;
            int oneStepBehind = head;

            int numberBuffer = 0;
            while ((numberBuffer <= tail && numberBuffer >= 0 || tail < 0 ) && numberBuffer >= 0)
            {
                mre2.WaitOne();
                numberBuffer = oneStepBehind + twoStepsBehind;

                BeginInvoke(new Action(() => { richTextBox2.AppendText(numberBuffer + " "); }));

                
                Thread.Sleep(1);
                twoStepsBehind = oneStepBehind;
                oneStepBehind = numberBuffer;
                
            }

            BeginInvoke(new Action(() =>
            {
                buttonStop2.Enabled = false;
                buttonPause2.Enabled = false;
                buttonStart2.Enabled = true;
                fibonacciNumberGeneratorThread.Abort();
            }));
        }



        private void button1_Click(object sender, EventArgs e)
        {
            buttonStop1.Enabled = true;
            buttonPause1.Enabled = true;
            buttonStart1.Enabled = false;

            richTextBox1.Clear();

            primeNumberGeneratorThread = new Thread(new ParameterizedThreadStart(GeneratePrimeNumbers));

            if (!int.TryParse(textBox1.Text, out int lowerB) || lowerB <= 0)
                lowerB = 2;
            if (!int.TryParse(textBox2.Text, out int upperB))
                upperB = -1;



            int[] parameters = { lowerB, upperB };
            primeNumberGeneratorThread.Start(parameters);
        }


        private void button2_Click(object sender, EventArgs e)
        {

            buttonPause1.Text = "Pause";
            buttonStop1.Enabled = false;
            buttonStart1.Enabled = true;
            buttonPause1.Enabled = false;

            primeNumberGeneratorThread.Abort();
            mre1.Set();
        }

        private void buttonStart2_Click(object sender, EventArgs e)
        {
            buttonStop2.Enabled = true;
            buttonStart2.Enabled = false;
            buttonPause2.Enabled = true;


            fibonacciNumberGeneratorThread = new Thread(new ParameterizedThreadStart(GenerateFibonacciNumbers));

            richTextBox2.Clear();

            if (!int.TryParse(textBox4.Text, out int lowerB) || lowerB <= 0)
                lowerB = 2;
            if (!int.TryParse(textBox3.Text, out int upperB))
                upperB = -1;

            int[] parameters = { lowerB, upperB };
            fibonacciNumberGeneratorThread.Start(parameters);

        }

        private void buttonStop2_Click(object sender, EventArgs e)
        {
            buttonPause2.Text = "Pause";
            buttonStop2.Enabled = false;
            buttonStart2.Enabled = true;
            buttonPause2.Enabled = false;

            fibonacciNumberGeneratorThread.Abort();
            mre2.Set();
        }

        private void buttonPause1_Click(object sender, EventArgs e)
        {
            if (mre1.WaitOne(0))
            {
                mre1.Reset();
                buttonPause1.Text = "Resume";
            }
            else
            {
                mre1.Set();
                buttonPause1.Text = "Pause";
            }
        }

        private void buttonPause2_Click(object sender, EventArgs e)
        {
            if (!mre2.WaitOne(0))
            {
                mre2.Reset();
                buttonPause2.Text = "Resume";
            }
            else
            {
                mre2.Set();
                buttonPause2.Text = "Pause";
            }
        }
    }
}
