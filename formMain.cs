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

        double slopMglobal = 0;
        double YcutBglobal = 0;

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

        private void btnCalc_Click(object sender, EventArgs e)
        {
            double sumXY = 0;
            double sumX = 0;
            double sumY = 0;
            double sumX2 = 0;
            double avgX = 0;
            double avgY = 0;
            double slopM = 0;
            double YcutB = 0;

            double MaxBR = 200;

            double AeroCap = 0;
            double fitnessRating = 0;



            double XoffsetGraph = 8;
            double YoffsetGraph = 50;


            double[] XaeroCap15cm = { 11, 14, 18, 21, 25 };
            double[] XaeroCap20cm = { 12, 17, 21, 25, 29 };
            double[] XaeroCap25cm = { 14, 19, 24, 28, 33 };
            double[] XaeroCap30cm = { 16, 21, 27, 32, 37 };

            double[] XaeroCapCur = new double[5];
            double[] YBRperMin = new double[5];


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



            YBRperMin[0] = Convert.ToDouble(step1TB.Text.ToString());
            YBRperMin[1] = Convert.ToDouble(step2TB.Text.ToString());
            YBRperMin[2] = Convert.ToDouble(step3TB.Text.ToString());
            YBRperMin[3] = Convert.ToDouble(step4TB.Text.ToString());
            YBRperMin[4] = Convert.ToDouble(step5TB.Text.ToString());

            /*for (int i = 0; i < 5; i++)
            {
                if (YBRperMin[i] < 100 || YBRperMin[i] > 200)
                {
                    YBRperMin[i] = -1;
                }
            }*/

            for (int i = 0; i < 5; i++)
            {

                sumXY = sumXY + YBRperMin[i] * XaeroCapCur[i];
                sumX = sumX + XaeroCapCur[i];
                sumY = sumY + YBRperMin[i];
                sumX2 = sumX2 + XaeroCapCur[i] * XaeroCapCur[i];


            }

            double nData = 5;

            avgX = sumX / nData;
            avgY = sumY / nData;

            slopM = (sumXY - (sumX * sumY) / nData) / (sumX2 - (sumX * sumX) / nData);
            YcutB = avgY - slopM * avgX;

            slopMglobal = slopM;
            YcutBglobal = YcutB;

            AeroCap = (MaxBR - YcutB) / slopM;

            aeroCapTB.Text = AeroCap.ToString();

            


        }

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

        private void btnReset_Click(object sender, EventArgs e)
        {
            slopMglobal = 0;
            YcutBglobal = 0;

            firstNameTB.Text = "";
            surnameTB.Text = "";
            femaleRB.Checked = false;
            maleRB.Checked = false;
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

        private void btnGraph_Click(object sender, EventArgs e)
        {
            //firstNameTB.Text = stepLB.GetItemText(stepLB.SelectedItem);
            int YvalPix = 0;
            //int Xval = 0;
            //int Xtop = 550;
            int XoffsetPix = 30;
            int XoffsetGraph = 8;
            int YoffsetGraph = 50;
            int YtopPix = 185;

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

                if (step1TB.Text.ToString().Length > 0)
                {
                    YvalPix = YtopPix - (Int32.Parse(step1TB.Text.ToString()) - YoffsetGraph) * delYpix;
                    //Xval = 50;
                    g.DrawEllipse(redPen, XaeroCapCur[0], YvalPix, pointR, pointR);
                    pictureBox1.Refresh();
                }

                if (step2TB.Text.ToString().Length > 0)
                {
                    //Yval = Ytop - Int32.Parse(step2TB.Text.ToString()) * delY;
                    YvalPix = YtopPix - (Int32.Parse(step2TB.Text.ToString()) - YoffsetGraph) * delYpix;
                    //Xval = 150;
                    g.DrawEllipse(redPen, XaeroCapCur[1], YvalPix, pointR, pointR);
                    pictureBox1.Refresh();
                }
                if (step3TB.Text.ToString().Length > 0)
                {
                    //Yval = Ytop - Int32.Parse(step3TB.Text.ToString()) * delY;
                    YvalPix = YtopPix - (Int32.Parse(step3TB.Text.ToString()) - YoffsetGraph) * delYpix;
                    //Xval = 250;
                    g.DrawEllipse(redPen, XaeroCapCur[2], YvalPix, pointR, pointR);
                    pictureBox1.Refresh();

                }
                if (step4TB.Text.ToString().Length > 0)
                {
                    //Yval = Ytop - Int32.Parse(step4TB.Text.ToString()) * delY;
                    YvalPix = YtopPix - (Int32.Parse(step4TB.Text.ToString()) - YoffsetGraph) * delYpix;
                    //Xval = 360;
                    g.DrawEllipse(redPen, XaeroCapCur[3], YvalPix, pointR, pointR);
                    pictureBox1.Refresh();
                }
                if (step5TB.Text.ToString().Length > 0)
                {
                    //Yval = Ytop - Int32.Parse(step5TB.Text.ToString())*delY;
                    YvalPix = YtopPix - (Int32.Parse(step5TB.Text.ToString()) - YoffsetGraph) * delYpix;
                    //Xval = 450;
                    g.DrawEllipse(redPen, XaeroCapCur[4], YvalPix, pointR, pointR);
                    pictureBox1.Refresh();
                }

                lowPointY = 100;

                highPointY = 200;


                lowPointX = (int)((lowPointY - YcutBglobal) / slopMglobal);
                highPointX = (int)((highPointY - YcutBglobal) / slopMglobal);

                lowPointYpix = YtopPix - (lowPointY - YoffsetGraph) * delYpix;
                highPointYpix = YtopPix - (highPointY - YoffsetGraph) * delYpix;




                lowPointXpix = XoffsetPix + (lowPointX - XoffsetGraph) * delXpix;
                highPointXpix = XoffsetPix + (highPointX - XoffsetGraph) * delXpix;


                

                g.DrawLine(greenPen, lowPointXpix, lowPointYpix,highPointXpix, highPointYpix);
                g.DrawLine(aquaPen, highPointXpix, highPointYpix, highPointXpix, YtopPix-10);

                //g.DrawLine(greenPen, 100, 100, 200, 200);

                pictureBox1.Refresh();


            }


            remarksTB.Text = "";
            remarksTB.Text = "(" + lowPointXpix.ToString() + ", " + lowPointYpix.ToString() + ") " +
                highPointXpix.ToString() + " " + highPointYpix.ToString();// +" " +
                //XaeroCapCur[4].ToString();

            /*remarksTB.Text = "";
            remarksTB.Text = XaeroCapCur[0].ToString() + " " + XaeroCapCur[1].ToString() + " " +
                XaeroCapCur[2].ToString() + " " + XaeroCapCur[3].ToString() + " " +
                XaeroCapCur[4].ToString();*/

            /*using (conn = new MySql.Data.MySqlClient.MySqlConnection(connString))
            {
                conn.Open();
                queryStr = "";
                queryStr = "SELECT * FROM db_step_test_system.participant_details";

                cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);

                reader = cmd.ExecuteReader();


                reader.Close();

                conn.Close();
            }*/
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.graph2D_final0;
            stepLB.SelectedIndex = 0;
            step1TB.Text = "100";
            step2TB.Text = "140";
            step3TB.Text = "160";
            step4TB.Text = "180";
            step5TB.Text = "195";
            //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

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
    }
}
