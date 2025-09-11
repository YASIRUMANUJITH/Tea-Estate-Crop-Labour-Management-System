using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using System.Text.RegularExpressions;
using System.IO;

namespace LoginRegistrationForm
{
    public partial class Form1 : Form
    {
        private readonly string connString;

        public Form1()
        {
            InitializeComponent();
            string dbPath = Path.Combine(Application.StartupPath, "Data", "main.db");
            connString = $"Data Source={dbPath}";
        }

        private void login_register_Click(object sender, EventArgs e)
        {
            Signup sForm = new Signup();
            sForm.Show();
            this.Hide();
        }

        private void login_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(login_username.Text) || string.IsNullOrWhiteSpace(login_password.Text))
            {
                MessageBox.Show("Please fill all blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var conn = new SqliteConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Check if username exists and verify password
                    string query = "SELECT password FROM userdata WHERE username = @username";
                    using (var cmd = new SqliteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", login_username.Text.Trim());
                        var result = cmd.ExecuteScalar();

                        if (result == null)
                        {
                            MessageBox.Show("Username does not exist", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string storedPassword = result.ToString();
                        if (storedPassword != login_password.Text)
                        {
                            MessageBox.Show("Incorrect password", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Login successful
                        MessageBox.Show("Logged in Successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AdminDashboard dashboard = new AdminDashboard();
                        dashboard.Show();
                        this.Hide();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void login_showPass_CheckedChanged(object sender, EventArgs e)
        {
            if (login_showPass.Checked)
            {
                login_password.PasswordChar = '\0';
            }
            else
            {
                login_password.PasswordChar = '*';
            }
        }
    }
}