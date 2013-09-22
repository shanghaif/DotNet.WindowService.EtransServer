using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace ET.WinService.Manager
{
    public delegate void SaveChange(ArrayList arr, bool isActivate);
    public partial class TaskEdit : Form
    {
        public event SaveChange SaveTimes;
        private string times;
        private bool isActivate;
        public TaskEdit()
        {
            InitializeComponent();
        }
        public TaskEdit(string times,bool isActivate)
        {
            InitializeComponent();
            this.times = times;
            this.isActivate = isActivate;
        }

        private void TaskEdit_Load(object sender, EventArgs e)
        {
            foreach (var item in times.Split(','))
            {
                lstTime.Items.Add(item);
            }
            chkActive.Checked = isActivate;
        }

        private void lstTime_DoubleClick(object sender, EventArgs e)
        {
            lstTime.Items.Remove(lstTime.SelectedItem);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string value=dtpTime.Text;
            if (!lstTime.Items.Contains(value))
            {
                lstTime.Items.Add(dtpTime.Text);
            }
            else
            {
                MessageBox.Show("已经存在相同值，不能再添加!");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ArrayList arr = new ArrayList();
            foreach (var item in lstTime.Items)
            {
                arr.Add(item);
            }
            SaveTimes(arr,chkActive.Checked);
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            lstTime.Items.Remove(lstTime.SelectedItem);
        }
    }
}
