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

namespace LoginRegistrationForm
{
    public partial class Form1 : Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\senal\OneDrive\Documents\LoginData.mdf;Integrated Security=True;Connect Timeout=30");

        public Form1()
        {
            InitializeComponent();
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
            if (login_username.Text == "" || login_password.Text == "")
            {
                MessageBox.Show("Please fill all the blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();

                        // First check if username exists
                        String checkUsername = "SELECT COUNT(*) FROM admin WHERE username = @username";
                        using (SqlCommand cmdUsername = new SqlCommand(checkUsername, connect))
                        {
                            cmdUsername.Parameters.AddWithValue("@username", login_username.Text);
                            int usernameCount = (int)cmdUsername.ExecuteScalar();

                            if (usernameCount == 0)
                            {
                                MessageBox.Show("Username does not exist", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        // If username exists, get the actual password from database and compare case-sensitively
                        String getPassword = "SELECT passowrd FROM admin WHERE username = @username";
                        using (SqlCommand cmdGetPassword = new SqlCommand(getPassword, connect))
                        {
                            cmdGetPassword.Parameters.AddWithValue("@username", login_username.Text);
                            object result = cmdGetPassword.ExecuteScalar();

                            if (result != null)
                            {
                                string storedPassword = result.ToString();

                                // Case-sensitive password comparison
                                if (storedPassword != login_password.Text)
                                {
                                    MessageBox.Show("Incorrect password", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Error retrieving password", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        // If both username and password are correct
                        MessageBox.Show("Logged in Successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AdminDashboard dashboard = new AdminDashboard();
                        dashboard.Show();
                        this.Hide();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error Connecting: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
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