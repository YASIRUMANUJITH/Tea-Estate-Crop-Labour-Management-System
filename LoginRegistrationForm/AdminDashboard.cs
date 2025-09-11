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
    public partial class AdminDashboard : Form
    {
      
        public AdminDashboard()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
           worker dashboard = new worker();
           dashboard.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Entry dashboard = new Entry();
            dashboard.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Salary dashboard = new Salary();
            dashboard.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Feature dashboard = new Feature();
            dashboard.Show();
            this.Hide();
        }

        
    }
}

