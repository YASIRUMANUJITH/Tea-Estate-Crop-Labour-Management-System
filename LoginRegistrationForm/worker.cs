using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace LoginRegistrationForm
{
    public partial class worker : Form
    {
        private readonly string connString;

        public worker()
        {
            InitializeComponent();
            string dbPath = Path.Combine(Application.StartupPath, "Data", "main.db");
            connString = $"Data Source={dbPath}";
            CreateTableIfNotExists();
        }

        private void CreateTableIfNotExists()
        {
            using (var conn = new SqliteConnection(connString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Employee (
                            EmployeeId INTEGER PRIMARY KEY,
                            EmployeeName TEXT NOT NULL,
                            EmployeeEmail TEXT NOT NULL,
                            Salary DECIMAL(10,2) NOT NULL
                        )";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                    string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text) ||
                    string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    MessageBox.Show("Please fill in all fields before saving.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var conn = new SqliteConnection(connString))
                {
                    conn.Open();
                    string insertQuery = "INSERT INTO Employee (EmployeeId, EmployeeName, EmployeeEmail, Salary) VALUES (@EmployeeId, @EmployeeName, @EmployeeEmail, @Salary)";
                    using (var cmd = new SqliteCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@EmployeeName", textBox2.Text);
                        cmd.Parameters.AddWithValue("@EmployeeEmail", textBox3.Text);
                        cmd.Parameters.AddWithValue("@Salary", Convert.ToDecimal(textBox4.Text));
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Record saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData(); // Refresh the grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                using (var conn = new SqliteConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Employee";
                    using (var cmd = new SqliteCommand(query, conn))
                    {
                        var dt = new DataTable();
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                        dataGridView1.DataSource = dt;
                        label14.Text = dt.Rows.Count.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInsert_Click_1(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Please enter Employee ID to delete.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var conn = new SqliteConnection(connString))
                {
                    conn.Open();
                    string deleteQuery = "DELETE FROM Employee WHERE EmployeeId = @EmployeeId";
                    using (var cmd = new SqliteCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", int.Parse(textBox1.Text));
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData(); // Refresh the grid
                            btnClear_Click(null, null); // Clear the textboxes
                        }
                        else
                        {
                            MessageBox.Show("No record found with this Employee ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Please enter Employee ID to search.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var conn = new SqliteConnection(connString))
                {
                    conn.Open();
                    string searchQuery = "SELECT * FROM Employee WHERE EmployeeId = @EmployeeId";
                    using (var cmd = new SqliteCommand(searchQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", int.Parse(textBox1.Text));
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                textBox2.Text = reader["EmployeeName"].ToString();
                                textBox3.Text = reader["EmployeeEmail"].ToString();
                                textBox4.Text = reader["Salary"].ToString();

                                var dt = new DataTable();
                                dt.Load(reader);
                                dataGridView1.DataSource = dt;

                                MessageBox.Show("Record found.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No record found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                textBox2.Clear();
                                textBox3.Clear();
                                textBox4.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            LoadData(); // Refresh grid to show all records
            MessageBox.Show("Fields cleared.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    
}
