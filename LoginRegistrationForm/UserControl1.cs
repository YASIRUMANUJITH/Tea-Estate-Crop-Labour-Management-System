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
        private Form currentForm = null;
        public UserControl1()
        {
            InitializeComponent();
            // Wire up click events for panels
            panel12.Click += new EventHandler(panel12_Click);
            panel6.Click += new EventHandler(panel6_Click);
            panel7.Click += new EventHandler(panel7_Click);
            panel8.Click += new EventHandler(panel8_Click);
            panel9.Click += new EventHandler(panel9_Click);
            panel10.Click += new EventHandler(panel10_Click);
        }
        private void OpenForm(Form newForm)
        {
            if (currentForm != null)
            {
                currentForm.Close();
            }
            currentForm = newForm;
            newForm.TopLevel = false;
            newForm.FormBorderStyle = FormBorderStyle.None;
            newForm.Dock = DockStyle.Fill;
            this.Parent.Controls.Add(newForm);
            newForm.BringToFront();
            newForm.Show();
        }
        private void panel12_Click(object sender, EventArgs e)
        {
            OpenForm(new AdminDashboard());
        }
        private void panel6_Click(object sender, EventArgs e)
        {
            OpenForm(new worker());
        }
        private void panel7_Click(object sender, EventArgs e)
        {
            OpenForm(new Entry());
        }
        private void panel8_Click(object sender, EventArgs e)
        {
            OpenForm(new Salary());
        }
        private void panel9_Click(object sender, EventArgs e)
        {
            OpenForm(new Feature());
        }
        private void panel10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Exiting the application", "Information Message",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Exit();
        }
        private void panel12_Paint(object sender, PaintEventArgs e)
        {
        }
        private void panel6_Paint(object sender, PaintEventArgs e)
        {
        }
        private void panel7_Paint(object sender, PaintEventArgs e)
        {
        }
        private void panel8_Paint(object sender, PaintEventArgs e)
        {
        }
        private void panel9_Paint(object sender, PaintEventArgs e)
        {
        }
        private void panel10_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}