using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XLog.Sample.Winforms
{
    public partial class Form1 : Form
    {
        class FinalizeableClass
        {
            ~FinalizeableClass()
            {
                throw new Exception("Exception in finalizer");
            }
        }

        private static readonly Logger Log = LogManager.Default.GetLogger("Form1");

        public Form1()
        {
            InitializeComponent();

            Log.Info("ctor");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            throw new Exception("Main thread exc");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem((o) => { throw new Exception("BG thread exc"); });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                throw new Exception("Unobserved task exc");
            });

            Thread.Sleep(16);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new FinalizeableClass();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await Task.Delay(100);

            throw new Exception("Async void exception");
        }
    }
}