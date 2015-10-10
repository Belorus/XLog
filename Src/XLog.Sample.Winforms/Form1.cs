using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XLog.Sample.Winforms
{
    public partial class Form1 : Form
    {
        private readonly Logger Log = LogManager.Default.GetLogger("Form1");

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
    }
}