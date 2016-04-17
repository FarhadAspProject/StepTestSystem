using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StepTestSystem
{
    public partial class formEnquire : Form
    {
        String connString = System.Configuration.ConfigurationManager.ConnectionStrings["StepTestConnString"].ToString();
        MySql.Data.MySqlClient.MySqlConnection conn;
        MySql.Data.MySqlClient.MySqlCommand cmd;
        MySql.Data.MySqlClient.MySqlDataReader reader;
        String queryStr;
        public formEnquire()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnSearchByFirstName_Click(object sender, EventArgs e)
        {
            using (conn = new MySql.Data.MySqlClient.MySqlConnection(connString))
            {
                conn.Open();
                queryStr = "";
                queryStr = "SELECT test_firstName, test_date, test_time FROM db_step_test_system.test_details" +
                    " WHERE test_firstName = '" + txtFirstName.Text.ToString()+ "'  ORDER BY test_date DESC, "+
                    " test_time DESC ";
                cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);

                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    lstPrevTests.Items.Clear();
                    while (reader.HasRows && reader.Read())
                    {
                        string curFirstName = "";
                        string curDate = "";
                        string curTime = "";
                        curFirstName = reader.GetString(reader.GetOrdinal("test_firstName"));
                        curDate = reader.GetString(reader.GetOrdinal("test_date"));
                        curTime = reader.GetString(reader.GetOrdinal("test_time"));

                        lstPrevTests.Items.Add("Name: " + curFirstName + ", test date: " +
                            curDate.Split(' ').ElementAt(0) + ", time: " + curTime);
                    }
                }
                else
                {
                    string message = "No data exists with that name!";
                    string caption = "Search result";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    // Displays the MessageBox.

                    result = MessageBox.Show(message, caption, buttons);

                }
                reader.Close();

                conn.Close();
            }
        }

        private void btnSearchByDate_Click(object sender, EventArgs e)
        {
            using (conn = new MySql.Data.MySqlClient.MySqlConnection(connString))
            {
                conn.Open();
                queryStr = "";
                queryStr = "SELECT test_firstName, test_date, test_time FROM db_step_test_system.test_details" +
                    " WHERE test_date = '" + datePicker.Value.ToString("yyyy-MM-dd") + "'  ORDER BY test_date DESC, " +
                    " test_time DESC ";
                cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);

                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    lstPrevTests.Items.Clear();
                    while (reader.HasRows && reader.Read())
                    {
                        string curFirstName = "";
                        string curDate = "";
                        string curTime = "";
                        curFirstName = reader.GetString(reader.GetOrdinal("test_firstName"));
                        curDate = reader.GetString(reader.GetOrdinal("test_date"));
                        curTime = reader.GetString(reader.GetOrdinal("test_time"));

                        lstPrevTests.Items.Add("Name: " + curFirstName + ", test date: " +
                            curDate.Split(' ').ElementAt(0) + ", time: " + curTime);
                    }
                }
                else
                {
                    string message = "No tests were taken in this date!";
                    string caption = "Search result";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    // Displays the MessageBox.

                    result = MessageBox.Show(message, caption, buttons);

                }
                reader.Close();

                conn.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLoadTestData_Click(object sender, EventArgs e)
        {

        }

        private void formEnquire_Load(object sender, EventArgs e)
        {

        }
    }
}
