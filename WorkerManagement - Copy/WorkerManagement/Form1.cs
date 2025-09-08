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

namespace WorkerManagement
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void btnSave_Click(object sender, EventArgs e)
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



                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\CompFix\Desktop\vstudio\WorkerManagement\WorkerManagement\Database1.mdf;Integrated Security=True"))
                {
                    con.Open();
                    SqlCommand cnn = new SqlCommand("insert into Employee values(@employeeid,@employeename,@employeeemail,@salary)", con);
                    cnn.Parameters.AddWithValue("@Employeeid", int.Parse(textBox1.Text));
                    cnn.Parameters.AddWithValue("@EmployeeName", textBox2.Text);
                    cnn.Parameters.AddWithValue("@EmployeeEmail", textBox3.Text);
                    cnn.Parameters.AddWithValue("@Salary", Convert.ToDecimal(textBox4.Text));
                    cnn.ExecuteNonQuery();
                    MessageBox.Show("Record saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                
                
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid numbers for Employee ID and Salary.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Could not save record. Database error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\CompFix\Desktop\vstudio\WorkerManagement\WorkerManagement\Database1.mdf;Integrated Security=True"))
                {
                    SqlCommand cnn = new SqlCommand("select * from Employee", con);
                    SqlDataAdapter da = new SqlDataAdapter(cnn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                MessageBox.Show("Records loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Could not load records. Database error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Please enter Employee ID to delete a record.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\CompFix\Desktop\vstudio\WorkerManagement\WorkerManagement\Database1.mdf;Integrated Security=True"))
                {
                    con.Open();
                    SqlCommand cnn = new SqlCommand("delete Employee where employeeid=@employeeid", con);
                    cnn.Parameters.AddWithValue("@EmployeeId", int.Parse(textBox1.Text));

                    int rowsAffected = cnn.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No record found with the given Employee ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Employee ID must be a number."+ ex.Message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Could not delete record. Database error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            MessageBox.Show("Fields cleared.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            try
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\CompFix\Desktop\vstudio\WorkerManagement\WorkerManagement\Database1.mdf;Integrated Security=True"))
                {
                    SqlCommand cnn = new SqlCommand("select * from Employee", con);
                    SqlDataAdapter da = new SqlDataAdapter(cnn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    label8.Text = dt.Rows.Count.ToString();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Could not load employee data. Database error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Please enter Employee ID to search.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\CompFix\Desktop\vstudio\WorkerManagement\WorkerManagement\Database1.mdf;Integrated Security=True"))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT EmployeeId, EmployeeName, EmployeeEmail, Salary FROM Employee WHERE EmployeeId=@EmployeeId", con);
                    cmd.Parameters.AddWithValue("@EmployeeId", int.Parse(textBox1.Text));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        textBox1.Text = dt.Rows[0]["Employeeid"].ToString();
                        textBox2.Text = dt.Rows[0]["EmployeeName"].ToString();
                        textBox3.Text = dt.Rows[0]["EmployeeEmail"].ToString();
                        textBox4.Text = string.Format("{0:0.##}", dt.Rows[0]["Salary"]);

                        dataGridView1.DataSource = dt;
                        dataGridView1.Columns["Salary"].DefaultCellStyle.Format = "N0";

                        MessageBox.Show("Employee record found.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No record found with this Employee ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox2.Clear();
                        textBox3.Clear();
                        textBox4.Clear();
                        dataGridView1.DataSource = null;
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Employee ID must be a number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
    }
}

