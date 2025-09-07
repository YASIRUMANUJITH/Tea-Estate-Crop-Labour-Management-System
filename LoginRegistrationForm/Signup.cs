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
using System.Text.RegularExpressions;

namespace LoginRegistrationForm
{
    public partial class Signup : Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\senal\OneDrive\Documents\LoginData.mdf;Integrated Security=True;Connect Timeout=30");

        public Signup()
        {
            InitializeComponent();
        }

        private void signup_login_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void signup_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Email validation method
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Email regex pattern
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Username validation method with admin restriction
        private bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            // Username must be between 3 and 20 characters
            if (username.Length < 3 || username.Length > 20)
                return false;

            // Check if username contains "admin" in any form (except exact "Admin")
            if (!IsAllowedAdminUsage(username))
                return false;

            // Username must start with a letter and can only contain letters, numbers, and underscores
            string usernamePattern = @"^[a-zA-Z]+[a-zA-Z0-9_]*[0-9]*$";
            return Regex.IsMatch(username, usernamePattern);
        }

        // Method to check admin usage restrictions
        private bool IsAllowedAdminUsage(string username)
        {
            // Allow exact "Admin" only
            if (username == "Admin")
                return true;

            // Check if username contains "admin" in any case or with symbols/numbers
            string lowerUsername = username.ToLower();

            // Remove all non-alphabetic characters and check if it contains "admin"
            string cleanUsername = Regex.Replace(lowerUsername, @"[^a-z]", "");

            if (cleanUsername.Contains("admin"))
                return false;

            // Additional check for mixed case and symbol variations
            if (Regex.IsMatch(lowerUsername, @"a[^a-z]*d[^a-z]*m[^a-z]*i[^a-z]*n"))
                return false;

            return true;
        }

        // Password validation method
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Password must be at least 8 characters and include uppercase, lowercase, number, and special character
            string passwordPattern = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }

        // Check if email already exists in database
        private bool IsEmailExists(string email)
        {
            try
            {
                string checkEmail = "SELECT COUNT(*) FROM admin WHERE email = @email";
                using (SqlCommand checkEmailCmd = new SqlCommand(checkEmail, connect))
                {
                    checkEmailCmd.Parameters.AddWithValue("@email", email.Trim());
                    int count = (int)checkEmailCmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking email: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true; // Return true to prevent registration if there's an error
            }
        }

        // Check if username already exists in database
        private bool IsUsernameExists(string username)
        {
            try
            {
                string checkUsername = "SELECT COUNT(*) FROM admin WHERE username = @username";
                using (SqlCommand checkUsernameCmd = new SqlCommand(checkUsername, connect))
                {
                    checkUsernameCmd.Parameters.AddWithValue("@username", username.Trim());
                    int count = (int)checkUsernameCmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking username: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true; // Return true to prevent registration if there's an error
            }
        }

        // Validate all fields method
        private string ValidateFields()
        {
            List<string> errors = new List<string>();

            // Email validation
            if (string.IsNullOrWhiteSpace(signup_email.Text))
            {
                errors.Add("Email is required");
            }
            else if (!IsValidEmail(signup_email.Text.Trim()))
            {
                errors.Add("Please enter a valid email address");
            }

            // Username validation
            if (string.IsNullOrWhiteSpace(signup_username.Text))
            {
                errors.Add("Username is required");
            }
            else if (!IsValidUsername(signup_username.Text.Trim()))
            {
                if (signup_username.Text.Trim().Length < 3 || signup_username.Text.Trim().Length > 20)
                {
                    errors.Add("Username must be between 3 and 20 characters");
                }
                else if (!IsAllowedAdminUsage(signup_username.Text.Trim()))
                {
                    errors.Add("Username cannot contain 'admin' in any form.");
                }
                else
                {
                    errors.Add("Username must start with a letter and can only contain letters, numbers, and underscores");
                }
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(signup_password.Text))
            {
                errors.Add("Password is required");
            }
            else if (!IsValidPassword(signup_password.Text))
            {
                errors.Add("Password must be at least 8 characters and include uppercase, lowercase, number, and special character");
            }

            return string.Join("\n", errors);
        }

        private void signup_btn_Click(object sender, EventArgs e)
        {
            // Validate all fields first
            string validationErrors = ValidateFields();

            if (!string.IsNullOrEmpty(validationErrors))
            {
                MessageBox.Show(validationErrors, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Proceed with database operations if validation passes
            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    // Check if email already exists
                    if (IsEmailExists(signup_email.Text.Trim()))
                    {
                        MessageBox.Show("An account with this email already exists", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Check if username already exists
                    if (IsUsernameExists(signup_username.Text.Trim()))
                    {
                        MessageBox.Show(signup_username.Text + " already exists", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Insert new user if both email and username are unique
                    string insertData = "INSERT INTO admin (email, username, passowrd, date_created) VALUES (@email, @username, @pass, @date)";
                    DateTime date = DateTime.Today;
                    using (SqlCommand cmd = new SqlCommand(insertData, connect))
                    {
                        cmd.Parameters.AddWithValue("@email", signup_email.Text.Trim());
                        cmd.Parameters.AddWithValue("@username", signup_username.Text.Trim());
                        cmd.Parameters.AddWithValue("@pass", signup_password.Text.Trim());
                        cmd.Parameters.AddWithValue("@date", date);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Registered successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Form1 form1 = new Form1();
                        form1.Show();
                        this.Hide();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error connecting Database: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private void signup_showPass_CheckedChanged(object sender, EventArgs e)
        {
            if (signup_showPass.Checked)
            {
                signup_password.PasswordChar = '\0';
            }
            else
            {
                signup_password.PasswordChar = '*';
            }
        }
    }
}