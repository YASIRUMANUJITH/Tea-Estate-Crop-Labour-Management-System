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
using Microsoft.Data.Sqlite;
using System.IO;

namespace LoginRegistrationForm
{
    public partial class Signup : Form
    {
        string connString = $"Data Source={Path.Combine(Application.StartupPath, "Data", "main.db")}";

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
        private bool IsEmailExists(string email, SqliteConnection conn)
        {
            try
            {
                string checkEmail = "SELECT COUNT(*) FROM userdata WHERE email = @email";
                using (SqliteCommand checkEmailCmd = new SqliteCommand(checkEmail, conn))
                {
                    checkEmailCmd.Parameters.AddWithValue("@email", email.Trim());
                    var result = checkEmailCmd.ExecuteScalar();
                    // Convert the result to Int64 first, then to int
                    return Convert.ToInt32(result) > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking email: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

        }

        // Check if username already exists in database
        private bool IsUsernameExists(string username, SqliteConnection conn)
        {
            try
            {
                string checkUsername = "SELECT COUNT(*) FROM userdata WHERE username = @username";
                using (SqliteCommand checkUsernameCmd = new SqliteCommand(checkUsername, conn))
                {
                    checkUsernameCmd.Parameters.AddWithValue("@username", username.Trim());
                    var result = checkUsernameCmd.ExecuteScalar();
                    // Convert the result to Int64 first, then to int
                    return Convert.ToInt32(result) > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking username: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
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
            using (var conn = new SqliteConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Create table with correct schema
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS userdata (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    email VARCHAR(255) NOT NULL,
                    username TEXT NOT NULL,
                    password TEXT NOT NULL,
                    date_created TEXT NOT NULL
                );";
                        cmd.ExecuteNonQuery();
                    }

                    // Validate all fields first
                    string validationErrors = ValidateFields();
                    if (!string.IsNullOrEmpty(validationErrors))
                    {
                        MessageBox.Show(validationErrors, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Check if email already exists
                    if (IsEmailExists(signup_email.Text.Trim(), conn))
                    {
                        MessageBox.Show("An account with this email already exists", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Check if username already exists
                    if (IsUsernameExists(signup_username.Text.Trim(), conn))
                    {
                        MessageBox.Show(signup_username.Text + " already exists", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Insert new user
                    string insertData = "INSERT INTO userdata (email, username, password, date_created) VALUES (@email, @username, @pass, @date)";
                    DateTime date = DateTime.Today;
                    using (SqliteCommand cmd = new SqliteCommand(insertData, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", signup_email.Text.Trim());
                        cmd.Parameters.AddWithValue("@username", signup_username.Text.Trim());
                        cmd.Parameters.AddWithValue("@pass", signup_password.Text.Trim());
                        cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Registered successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Form1 form1 = new Form1();
                        form1.Show();
                        this.Hide();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error connecting Database: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void signup_password_TextChanged(object sender, EventArgs e)
        {

        }
    }
}