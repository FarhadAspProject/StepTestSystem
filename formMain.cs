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
using MySql.Data.MySqlClient;

namespace StepTestSystem
{
    public partial class formMain : Form
    {
        String connString = System.Configuration.ConfigurationManager.ConnectionStrings["StepTestConnString"].ToString();
        MySql.Data.MySqlClient.MySqlConnection conn;
        MySql.Data.MySqlClient.MySqlCommand cmd;
        MySql.Data.MySqlClient.MySqlDataReader reader;
        String queryStr;

        double slopMglobal = -50;
        double YcutBglobal = -50;

        // validatoin variables

        /*bool validateFirstName = false;
        bool validateAge = false;
        bool validateStep1 = false;
        bool validateStep2 = false;
        bool validateStep3 = false;
        bool validateStep4 = false;
        bool validateStep5 = false;*/


        public formMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalc_Click(object sender, EventArgs e)
        {
            string check = validateInputs();

            if (string.Compare(check, "OK") != 0)
            {
                string message =check;
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                return;
            }

            double sumXY = 0;
            double sumX = 0;
            double sumY = 0;
            double sumX2 = 0;
            double avgX = 0;
            double avgY = 0;
            double slopM = 0;
            double YcutB = 0;

            double age = Convert.ToDouble(ageTB.Text.ToString());
            double MaxHR = 220 - age;
            double HR80percent = MaxHR * 0.8;
            double HR50percent = MaxHR * 0.5;

            double AeroCap = 0;
            string fitnessRating = "";


            double[] XaeroCap15cm = { 11, 14, 18, 21, 25 };
            double[] XaeroCap20cm = { 12, 17, 21, 25, 29 };
            double[] XaeroCap25cm = { 14, 19, 24, 28, 33 };
            double[] XaeroCap30cm = { 16, 21, 27, 32, 37 };

            double[] XaeroCapCur = new double[5];
            double[] YHRperMin = new double[5];


            string StepSize = stepLB.SelectedItem.ToString();

            if (string.Compare(StepSize, "15 cm") == 0)
            {
                surnameTB.Text = "15 cm";
                Array.Copy(XaeroCap15cm, XaeroCapCur, 5);
            }
            else if (string.Compare(StepSize, "20 cm") == 0)
            {
                surnameTB.Text = "20 cm";
                Array.Copy(XaeroCap20cm, XaeroCapCur, 5);
            }
            else if (string.Compare(StepSize, "25 cm") == 0)
            {
                surnameTB.Text = "25 cm";
                Array.Copy(XaeroCap25cm, XaeroCapCur, 5);
            }
            else
            {
                surnameTB.Text = "30 cm";
                Array.Copy(XaeroCap30cm, XaeroCapCur, 5);
            }

            //////////////////////////////////////////////////
            double number;

            if (double.TryParse(step1TB.Text.ToString(), out number))
            {
                YHRperMin[0] = number;
            }
            else
            {
                YHRperMin[0] = -1;
            }

            if (double.TryParse(step2TB.Text.ToString(), out number))
            {
                YHRperMin[1] = number;
            }
            else
            {
                YHRperMin[1] = -1;
            }

            if (double.TryParse(step3TB.Text.ToString(), out number))
            {
                YHRperMin[2] = number;
            }
            else
            {
                YHRperMin[2] = -1;
            }

            if (double.TryParse(step4TB.Text.ToString(), out number))
            {
                YHRperMin[3] = number;
            }
            else
            {
                YHRperMin[3] = -1;
            }

            if (double.TryParse(step5TB.Text.ToString(), out number))
            {
                YHRperMin[4] = number;
            }
            else
            {
                YHRperMin[4] = -1;
            }
            //////////////////////////////////////////////////////////////////////////

            List<double> Xvalues = new List<double>();
            List<double> Yvalues = new List<double>();


            for (int i = 0; i < 5; i++)
            {
                if (YHRperMin[i] >= HR50percent && YHRperMin[i] <= HR80percent && YHRperMin[i] != -1)
                {
                    Xvalues.Add(XaeroCapCur[i]);
                    Yvalues.Add(YHRperMin[i]);
                }
            }

            if (Xvalues.Count < 2)
            {
                string message = "Not enough correct readings to calculate aerobic capacity!!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                return;
            }
            else
            {

                for (int i = 0; i < Xvalues.Count; i++)
                {

                    sumXY = sumXY + Yvalues[i] * Xvalues[i];
                    sumX = sumX + Xvalues[i];
                    sumY = sumY + Yvalues[i];
                    sumX2 = sumX2 + Xvalues[i] * Xvalues[i];


                }

                double nData = (double) Xvalues.Count;

                avgX = sumX / nData;
                avgY = sumY / nData;

                slopM = (sumXY - (sumX * sumY) / nData) / (sumX2 - (sumX * sumX) / nData);
                YcutB = avgY - slopM * avgX;
            }


            

            if (slopM < 0)
            {
                string message = "Inconsistency found in HR readings!! please enter the values again.";
                string caption = "Warning!!";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                return;
            }

            slopMglobal = slopM;

            YcutBglobal = YcutB;

            AeroCap = (MaxHR - YcutB) / slopM;

            bool sex = true;
            if (maleRB.Checked == true)
            {
                sex = false;
            }

            fitnessRating = getFitnessRating((int)age, sex, (int)AeroCap);
            fitnessRatingTB.Text = fitnessRating;


            aeroCapTB.Text = AeroCap.ToString();

            


        }
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void cmd_storeData_Click(object sender, EventArgs e)
        {
            String gender = "";
            if(femaleRB.Checked)
            { 
                gender = "Female";
            }
            else
            {
                gender="Male";
            }
            using (conn = new MySql.Data.MySqlClient.MySqlConnection(connString))
            {
                conn.Open();
                queryStr = "INSERT INTO db_step_test_system.participant_details (part_firstName, part_surname," +
                    "part_age, part_gender, testers_initials)" +
                "VALUES('" + firstNameTB.Text.ToString() + "','" + surnameTB.Text.ToString() +
                "','" + Int32.Parse(ageTB.Text.ToString()) + "','"+gender+"','"+remarksTB.Text.ToString() + "')";

                //'" + fitnessRatingTB.Text.ToString() + "','" + Int32.Parse(aeroCapTB.Text.ToString())+",

                cmd = new MySqlCommand(queryStr, conn);
                cmd.ExecuteReader();

                conn.Close();

            }
        }
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            slopMglobal = -50;
            YcutBglobal = -50;

            firstNameTB.Text = "";
            surnameTB.Text = "";
            femaleRB.Checked = false;
            maleRB.Checked = true;
            ageTB.Text = "";
            remarksTB.Text = "";
            step1TB.Text = "";
            step2TB.Text = "";
            step3TB.Text = "";
            step4TB.Text = "";
            step5TB.Text = "";
            fitnessRatingTB.Text = "";
            aeroCapTB.Text = "";
            pictureBox1.Image = Properties.Resources.graph2D_final0;
            //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            

        }
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnGraph_Click(object sender, EventArgs e)
        {

            string check = validateInputs();

            if (string.Compare(check, "OK") != 0)
            {
                string message = check;
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                return;
            }
            
            if(slopMglobal == -50)
            {
                string message = "Graph is available after successful calcualtion."+
                    " Please press the calculate button first."; 
                string caption = "Warning!";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                return;
            }

            int YvalPix = 0;
            int XoffsetPix = 30;
            int XoffsetGraph = 8;
            int YoffsetGraph = 50;
            int YtopPix = 185;

            double age = Convert.ToDouble(ageTB.Text.ToString());

            double MaxHR = 220 - age;
            double HR80percent = MaxHR * 0.8;
            double HR50percent = MaxHR * 0.5;

            int delXpix = 9;
            int delYpix = 1;
            int pointR = 3;

            int lowPointX = 0;
            int lowPointY = 0;
            int highPointX = 0;
            int highPointY = 0;

            int lowPointXpix = 0;
            int lowPointYpix = 0;
            int highPointXpix = 0;
            int highPointYpix = 0;

            int[] XaeroCap15cm = { 11, 14, 18, 21, 25 };
            int[] XaeroCap20cm = { 12, 17, 21, 25, 29 };
            int[] XaeroCap25cm = { 14, 19, 24, 28, 33 };
            int[] XaeroCap30cm = { 16, 21, 27, 32, 37 };

            int[] XaeroCapCur = new int[5];


            string StepSize = stepLB.SelectedItem.ToString();

            if (string.Compare(StepSize, "15 cm") == 0)
            {
                surnameTB.Text = "15 cm";
                Array.Copy(XaeroCap15cm, XaeroCapCur, 5);
            }
            else if (string.Compare(StepSize, "20 cm") == 0)
            {
                surnameTB.Text = "20 cm";
                Array.Copy(XaeroCap20cm, XaeroCapCur, 5);
            }
            else if (string.Compare(StepSize, "25 cm") == 0)
            {
                surnameTB.Text = "25 cm";
                Array.Copy(XaeroCap25cm, XaeroCapCur, 5);
            }
            else
            {
                surnameTB.Text = "30 cm";
                Array.Copy(XaeroCap30cm, XaeroCapCur, 5);
            }




            for (int i = 0; i < 5; i++)
            {
                XaeroCapCur[i] = XoffsetPix + (XaeroCapCur[i] - XoffsetGraph) * delXpix;
            }

            using (var g = Graphics.FromImage(pictureBox1.Image))
            {

                Pen redPen = new Pen(Color.Red, 4);
                Pen greenPen = new Pen(Color.Green, 3);
                Pen aquaPen = new Pen(Color.Aqua, 3);

                int stepValue;

                if (int.TryParse(step1TB.Text.ToString(), out stepValue))
                {
                    YvalPix = YtopPix - (stepValue - YoffsetGraph) * delYpix;
                    g.DrawEllipse(redPen, XaeroCapCur[0], YvalPix, pointR, pointR);
                    pictureBox1.Refresh();
                }

                if (int.TryParse(step2TB.Text.ToString(), out stepValue))
                {
                    YvalPix = YtopPix - (stepValue - YoffsetGraph) * delYpix;

                    g.DrawEllipse(redPen, XaeroCapCur[1], YvalPix, pointR, pointR);
                    pictureBox1.Refresh();
                }
                if (int.TryParse(step3TB.Text.ToString(), out stepValue))
                {
                    YvalPix = YtopPix - (stepValue - YoffsetGraph) * delYpix;

                    g.DrawEllipse(redPen, XaeroCapCur[2], YvalPix, pointR, pointR);
                    pictureBox1.Refresh();

                }
                if (int.TryParse(step4TB.Text.ToString(), out stepValue))
                {

                    YvalPix = YtopPix - (stepValue - YoffsetGraph) * delYpix;
                    g.DrawEllipse(redPen, XaeroCapCur[3], YvalPix, pointR, pointR);
                    pictureBox1.Refresh();
                }
                if (int.TryParse(step5TB.Text.ToString(), out stepValue))
                {

                    YvalPix = YtopPix - (stepValue - YoffsetGraph) * delYpix;
                    g.DrawEllipse(redPen, XaeroCapCur[4], YvalPix, pointR, pointR);
                    pictureBox1.Refresh();
                }



                lowPointY = (int)HR50percent ;

                highPointY = (int)MaxHR;


                lowPointX = (int)((lowPointY - YcutBglobal) / slopMglobal);
                highPointX = (int)((highPointY - YcutBglobal) / slopMglobal);

                lowPointYpix = YtopPix - (lowPointY - YoffsetGraph) * delYpix;
                highPointYpix = YtopPix - (highPointY - YoffsetGraph) * delYpix;




                lowPointXpix = XoffsetPix + (lowPointX - XoffsetGraph) * delXpix;
                highPointXpix = XoffsetPix + (highPointX - XoffsetGraph) * delXpix;


                

                g.DrawLine(greenPen, lowPointXpix, lowPointYpix,highPointXpix, highPointYpix);
                g.DrawLine(aquaPen, highPointXpix, highPointYpix, highPointXpix, YtopPix-10);
               

                pictureBox1.Refresh();


            }


            remarksTB.Text = "";
            remarksTB.Text = "(" + lowPointXpix.ToString() + ", " + lowPointYpix.ToString() + ") " +
                highPointXpix.ToString() + " " + highPointYpix.ToString();

           
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.graph2D_final0;
            stepLB.SelectedIndex = 0;
            maleRB.PerformClick();
            step1TB.Text = "100";
            step2TB.Text = "140";
            step3TB.Text = "160";
            step4TB.Text = "180";
            step5TB.Text = "195";
            //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            int xCoordinate = e.X;
            int yCoordinate = e.Y;

            firstNameTB.Text = xCoordinate.ToString() + ", " + yCoordinate.ToString();
        }

        private void surnameLbl_Click(object sender, EventArgs e)
        {
            

        }
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        public string getFitnessRating(int age, bool sex, int aeroCapCur)
        {
            if (sex)//female
            {
                if (15 <= age && age <= 19)
                {
                    if(aeroCapCur>54)
                    {
                        return "Excellent";
                    }
                    else if(44<= aeroCapCur && aeroCapCur<=54)
                    {
                        return "Good";
                    }
                    else if (35 <= aeroCapCur && aeroCapCur <= 43)
                    {
                        return "Average";
                    }
                    else if (29 <= aeroCapCur && aeroCapCur <= 35)
                    {
                        return "Below average";
                    }
                    else 
                    {
                        return "Poor";
                    }
                }
                else if (20 <= age && age <= 29)
                {
                    if (aeroCapCur > 49)
                    {
                        return "Excellent";
                    }
                    else if (40 <= aeroCapCur && aeroCapCur <= 49)
                    {
                        return "Good";
                    }
                    else if (32 <= aeroCapCur && aeroCapCur <= 39)
                    {
                        return "Average";
                    }
                    else if (27 <= aeroCapCur && aeroCapCur <= 31)
                    {
                        return "Below average";
                    }
                    else
                    {
                        return "Poor";
                    }

                }
                else if (30 <= age && age <= 39)
                {
                    if (aeroCapCur > 45)
                    {
                        return "Excellent";
                    }
                    else if (36 <= aeroCapCur && aeroCapCur <= 45)
                    {
                        return "Good";
                    }
                    else if (30 <= aeroCapCur && aeroCapCur <= 35)
                    {
                        return "Average";
                    }
                    else if (25 <= aeroCapCur && aeroCapCur <= 29)
                    {
                        return "Below average";
                    }
                    else
                    {
                        return "Poor";
                    }
                }
                else if (40 <= age && age <= 49)
                {
                    if (aeroCapCur > 42)
                    {
                        return "Excellent";
                    }
                    else if (34 <= aeroCapCur && aeroCapCur <= 42)
                    {
                        return "Good";
                    }
                    else if (28 <= aeroCapCur && aeroCapCur <= 33)
                    {
                        return "Average";
                    }
                    else if (22 <= aeroCapCur && aeroCapCur <= 27)
                    {
                        return "Below average";
                    }
                    else
                    {
                        return "Poor";
                    }
                }
                else if (50 <= age && age <= 59)
                {
                    if (aeroCapCur > 40)
                    {
                        return "Excellent";
                    }
                    else if (33 <= aeroCapCur && aeroCapCur <= 40)
                    {
                        return "Good";
                    }
                    else if (26 <= aeroCapCur && aeroCapCur <= 32)
                    {
                        return "Average";
                    }
                    else if (21 <= aeroCapCur && aeroCapCur <= 25)
                    {
                        return "Below average";
                    }
                    else
                    {
                        return "Poor";
                    }

                }
                else if (60 <= age && age <= 65)
                {
                    if (aeroCapCur > 38)
                    {
                        return "Excellent";
                    }
                    else if (31 <= aeroCapCur && aeroCapCur <= 38)
                    {
                        return "Good";
                    }
                    else if (24 <= aeroCapCur && aeroCapCur <= 30)
                    {
                        return "Average";
                    }
                    else if (19 <= aeroCapCur && aeroCapCur <= 23)
                    {
                        return "Below average";
                    }
                    else
                    {
                        return "Poor";
                    }
                }
                else
                {
                    return "Invalid age!";
                }
 
            }
            else // male
            {
                if (15 <= age && age <= 19)
                {
                    if (aeroCapCur > 59)
                    {
                        return "Excellent";
                    }
                    else if (48 <= aeroCapCur && aeroCapCur <= 59)
                    {
                        return "Good";
                    }
                    else if (39 <= aeroCapCur && aeroCapCur <= 47)
                    {
                        return "Average";
                    }
                    else if (30 <= aeroCapCur && aeroCapCur <= 38)
                    {
                        return "Below average";
                    }
                    else
                    {
                        return "Poor";
                    }
                }
                else if (20 <= age && age <= 29)
                {
                    if (aeroCapCur > 54)
                    {
                        return "Excellent";
                    }
                    else if (44 <= aeroCapCur && aeroCapCur <= 54)
                    {
                        return "Good";
                    }
                    else if (35 <= aeroCapCur && aeroCapCur <= 43)
                    {
                        return "Average";
                    }
                    else if (28 <= aeroCapCur && aeroCapCur <= 34)
                    {
                        return "Below average";
                    }
                    else
                    {
                        return "Poor";
                    }

                }
                else if (30 <= age && age <= 39)
                {
                    if (aeroCapCur > 49)
                    {
                        return "Excellent";
                    }
                    else if (40 <= aeroCapCur && aeroCapCur <= 49)
                    {
                        return "Good";
                    }
                    else if (34 <= aeroCapCur && aeroCapCur <= 39)
                    {
                        return "Average";
                    }
                    else if (26 <= aeroCapCur && aeroCapCur <= 33)
                    {
                        return "Below average";
                    }
                    else
                    {
                        return "Poor";
                    }
                }
                else if (40 <= age && age <= 49)
                {
                    if (aeroCapCur > 45)
                    {
                        return "Excellent";
                    }
                    else if (37 <= aeroCapCur && aeroCapCur <= 45)
                    {
                        return "Good";
                    }
                    else if (32 <= aeroCapCur && aeroCapCur <= 36)
                    {
                        return "Average";
                    }
                    else if (25 <= aeroCapCur && aeroCapCur <= 31)
                    {
                        return "Below average";
                    }
                    else
                    {
                        return "Poor";
                    }
                }
                else if (50 <= age && age <= 59)
                {
                    if (aeroCapCur > 43)
                    {
                        return "Excellent";
                    }
                    else if (35 <= aeroCapCur && aeroCapCur <= 43)
                    {
                        return "Good";
                    }
                    else if (29 <= aeroCapCur && aeroCapCur <= 34)
                    {
                        return "Average";
                    }
                    else if (23 <= aeroCapCur && aeroCapCur <= 28)
                    {
                        return "Below average";
                    }
                    else
                    {
                        return "Poor";
                    }

                }
                else if (60 <= age && age <= 65)
                {
                    if (aeroCapCur > 39)
                    {
                        return "Excellent";
                    }
                    else if (33 <= aeroCapCur && aeroCapCur <= 39)
                    {
                        return "Good";
                    }
                    else if (25 <= aeroCapCur && aeroCapCur <= 32)
                    {
                        return "Average";
                    }
                    else if (20 <= aeroCapCur && aeroCapCur <= 24)
                    {
                        return "Below average";
                    }
                    else
                    {
                        return "Poor";
                    }
                }
                else
                {
                    return "Invalid age!";
                }
 
            }

            return "";
        }

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        public string validateInputs()
        {
            int number;

            if (firstNameTB.Text.ToString().Length < 1)
            {
                return "Please insert the first name!";
            }
            else if(surnameTB.Text.ToString().Length<1)
            {
                surnameTB.Text = firstNameTB.Text.ToString();
            }
            else if (ageTB.Text.ToString().Length < 1 || !int.TryParse(ageTB.Text.ToString(), out number))
            {
                return "Invalid age!";
            }      
            else if (step1TB.Text.ToString().Length>0 && !int.TryParse(step1TB.Text.ToString(), out number))
            {
                return "HR reading for step 1 is not valid!";
            }

            else if (step2TB.Text.ToString().Length > 0 && !int.TryParse(step2TB.Text.ToString(), out number))
            {
                return "HR reading for step 2 is not valid!";
            }

            else if (step3TB.Text.ToString().Length > 0 && !int.TryParse(step3TB.Text.ToString(), out number))
            {
                return "Value for step 3 is not valid!";
            }

            else if (step4TB.Text.ToString().Length > 0 && !int.TryParse(step4TB.Text.ToString(), out number))
            {
                return "HR reading for step 4 is not valid!";
            }

            else if (step5TB.Text.ToString().Length > 0 && !int.TryParse(step5TB.Text.ToString(), out number))
            {
                return "HR reading for step 5 is not valid!";
            }
            else
            {
                return "OK";
            }
            return "OK";

        }
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ageTB_Leave(object sender, EventArgs e)
        {
            //validateAge = false;
            int number;
            bool type = int.TryParse(ageTB.Text.ToString(), out number);

            if(ageTB.Text.ToString().Length<1)
            {
                string message = "Please enter the age!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
            }

            else if (!type)
            {
                string message = "The entered age is not Valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                ageTB.Text = "";


            }
            else if(number<1 || number>100)
            {
                string message = "The entered age is not Valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                ageTB.Text = "";

            }
            else
            {
                //validateAge = true;
            }
        }
        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void step1TB_Leave(object sender, EventArgs e)
        {
            //validateStep1 = false;
            int number;


            bool type = int.TryParse(step1TB.Text.ToString(), out number);

            if (!type)
            {
                string message = "The entered HR for step 1 is not Valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                step1TB.Text = "";


            }
            else if (number < 60 || number > 220)
            {
                string message = "The entered HR for step 1 is not valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                step1TB.Text = "";

            }
            else
            {
                //validateStep1 = true;
            }
        }
        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void step2TB_Leave(object sender, EventArgs e)
        {
            //validateStep2 = false;
            int number;
            bool type = int.TryParse(step2TB.Text.ToString(), out number);
            
            if (!type)
            {
                string message = "The entered HR for step 2 is not Valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                step2TB.Text = "";


            }
            else if (number < 60 || number > 220)
            {
                string message = "The entered HR for step 2 is not Valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                step2TB.Text = "";

            }
            else
            {
               // validateStep2 = true;
            }
            

            
        }
        /// <summary>
        /// //////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void step3TB_Leave(object sender, EventArgs e)
        {
            //validateStep3 = false;
            int number;
            bool type = int.TryParse(step3TB.Text.ToString(), out number);

            if (!type)
            {
                string message = "The entered HR for step 3 is not Valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                step3TB.Text = "";


            }
            else if (number < 60 || number > 220)
            {
                string message = "The entered HR for step 3 is not Valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                step3TB.Text = "";

            }
            else
            {
               // validateStep3 = true;
            }
            

        }

        private void step4TB_Leave(object sender, EventArgs e)
        {
            //validateStep4 = false;
            int number;
            bool type = int.TryParse(step4TB.Text.ToString(), out number);
            
            if (!type)
            {
                string message = "The entered HR for step 4 is not Valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                step4TB.Text = "";


            }
            else if (number < 60 || number > 220)
            {
                string message = "The entered HR for step 4 is not Valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                step4TB.Text = "";

            }
            else
            {
               // validateStep4 = true;
            }           
        }

        private void step5TB_Leave(object sender, EventArgs e)
        {
           // validateStep5 = false;
            int number;
            bool type = int.TryParse(step5TB.Text.ToString(), out number);

            if (!type)
            {
                string message = "The entered HR for step 5 is not Valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                step5TB.Text = "";


            }
            else if (number < 60 || number > 220)
            {
                string message = "The entered HR for step 5 is not Valid!";
                string caption = "Error Detected in Input";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);
                step5TB.Text = "";

            }
            else
            {
               // validateStep5 = true;
            } 
        }
    }
}
