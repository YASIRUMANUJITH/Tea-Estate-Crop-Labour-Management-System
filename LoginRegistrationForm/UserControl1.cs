using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginRegistrationForm
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            panel12.Click += Panel12_Click;
            panel6.Click += Panel6_Click;
            panel7.Click += Panel7_Click;
            panel8.Click += Panel8_Click;
            panel9.Click += Panel9_Click;
            panel10.Click += Panel10_Click;


        }

        private void Panel12_Click(object sender, EventArgs e)
        {
            AdminDashboard dashboard = new AdminDashboard();
            dashboard.Show();
            this.Hide();
        }

        private void Panel6_Click(object sender, EventArgs e)
        {
            worker dashboard = new worker();
            dashboard.Show();
            this.Hide();
        }
        private void Panel7_Click(object sender, EventArgs e)
        {
            Entry dashboard = new Entry();
            dashboard.Show();
            this.Hide();
        }

        private void Panel8_Click(object sender, EventArgs e)
        {
            Salary dashboard = new Salary();
            dashboard.Show();
            this.Hide();
        }

        private void Panel9_Click(object sender, EventArgs e)
        {
            Feature dashboard = new Feature();
            dashboard.Show();
            this.Hide();
        }

        private void Panel10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Logged off successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Exit();
        }




        private void panel12_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
