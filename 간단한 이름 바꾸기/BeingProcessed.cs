using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace 간단한_이름_바꾸기
{
    public partial class BeingProcessed : Form
    {
        public BeingProcessed()
        {
            InitializeComponent();
        }

        private void BeingProcessed_Load(object sender, EventArgs e)
        {
            lblCurrentFileName.Text = "";
            lblCurrentFileCount.Text = "0/0";
            pcbBeingProcessed.Value = 0;
            pcbBeingProcessed.Maximum = 1;
            pcbBeingProcessed.Minimum = 0;
        }

        public void SetMaxCount(int max)
        {
            pcbBeingProcessed.Maximum = max;
        }

        public void SetValue(int count)
        {
            pcbBeingProcessed.Value = count;
        }

        public void SetCurrentFileName(string name)
        {
            lblCurrentFileName.Text = name;
            lblCurrentFileName.Update();
        }

        public void SetCurrentFileCount(string count)
        {
            lblCurrentFileCount.Text = count;
            lblCurrentFileCount.Update();
        }

        private void btnRestoration_Click(object sender, EventArgs e)
        {
            ChangeName.instance.ReChangeFileName();

        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
