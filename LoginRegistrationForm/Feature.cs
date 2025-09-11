using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.Json;




namespace LoginRegistrationForm
{
    public partial class Feature : Form
    {
        public Feature()
        {
            InitializeComponent();
        }

        async static Task<string> Test(string prompt)
        {
            using (var client = new HttpClient())
            {

                var apiKey = "";
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    messages = new[]
                    {
                new
                {
                    role = "user",
                    content = prompt
                }
            },
                    model = "openai/gpt-oss-120b"
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
                var responseString = await response.Content.ReadAsStringAsync();


                using (var document = JsonDocument.Parse(responseString))
                {
                    var messageContent = document.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    //Console.WriteLine(messageContent);
                    return messageContent;
                }

            }
        }
        private void Feature_Load(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string prompt = textBox1.Text;
            var message = await Test(prompt);
            MessageBox.Show(message, "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
