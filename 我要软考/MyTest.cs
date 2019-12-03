﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;
using DAL;
using UI;

namespace 我要软考
{
    public partial class MyTest : Form
    {
        public MyTest()
        {
            InitializeComponent();
        }
        string[] questionArray;//储存一道的所有数据
        string[] numArray;
        int[] ruanjianshejishi_qBId = { 1, 2, 3, 4 };//标识有多少份题
        int[] chenxuyuan_qBId = { 5, 6 };//标识有多少份题
        string[] answerArray;//记录作答的记录
        string SuserEmail = "1316836373@qq.com";
        int qBId = 1;       //标识题库号
        int qId = 1;        //标识题号
        bool isPush = true; //标识是否点击提交
        bool Wrong;         //标识 是否是错题
        bool collection;    //标识 是否是收藏
        bool isRead;        //标识 是否是读取
        bool isExit;        //标识 是否是存在
        ArrayList stateList = new ArrayList();//初始化

        private void My_Load(object sender, EventArgs e)
        {
            isPanelFalse(false);
            panel5.Visible = false;
            //dataGridView1.DataSource = BLL.bll.getDataset();
            DataSet ds = new DataSet();
            using (SqlConnection CON = new SqlConnection(sqlLink.sqlcon()))
            {
                CON.Open();
                using (SqlCommand CMD = new SqlCommand("select * from Question", CON))
                {
                    SqlDataAdapter da = new SqlDataAdapter(CMD);
                    da.Fill(ds, "Question");
                    //da.Dispose();
                    CMD.Dispose();
                }
                CON.Close();
            }
        }

        //控制Pane的可视
        public void isPanelFalse(bool istrue)
        {
            panel1.Visible = istrue;
            panel2.Visible = istrue;
            panel3.Visible = istrue;
            panel4.Visible = istrue;
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "软件设计师":
                    isPanelFalse(true);
                    label2.Text = BLL.bll.qbdaytype(ruanjianshejishi_qBId[0]).ToString();
                    label3.Text = BLL.bll.qbdaytype(ruanjianshejishi_qBId[1]).ToString();
                    label4.Text = BLL.bll.qbdaytype(ruanjianshejishi_qBId[2]).ToString();
                    label5.Text = BLL.bll.qbdaytype(ruanjianshejishi_qBId[3]).ToString();
                    break;

                case "程序员":
                    isPanelFalse(true);
                    label2.Text = BLL.bll.qbdaytype(chenxuyuan_qBId[0]).ToString();
                    label3.Text = BLL.bll.qbdaytype(chenxuyuan_qBId[1]).ToString();
                    break;
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (label2.Text == BLL.bll.qbdaytype(ruanjianshejishi_qBId[0]).ToString())
            {
                isPanelFalse(false);
                panel5.Visible = true;
                var qBId = ruanjianshejishi_qBId[0];
                //var qId = 2;
                questionArray = BLL.bll.loadPaper(qBId, qId);//根据这个去查这个题库的试卷
                fill(questionArray, false);
            }

        }

        /// <summary>
        /// 读取数据填充到控件 
        /// </summary>
        /// <param name="thisquestionArray">问题的一系列的储存</param>
        /// <param name="isClear">是否点击了上下题，是则清空之前的数据</param>
        public void fill(string[] thisquestionArray, bool isClear)
        {
            if (isClear)
            {
                question.Text = string.Empty;
                parsing.Text = string.Empty;
                comments.Text = string.Empty;
            }
            lblqId.Text = "第" + thisquestionArray[0].ToString() + "题";
            rdoAnswerA.Text = "A. " + thisquestionArray[3].ToString();
            rdoAnswerB.Text = "B. " + thisquestionArray[4].ToString();
            rdoAnswerC.Text = "C. " + thisquestionArray[5].ToString();
            rdoAnswerD.Text = "D. " + thisquestionArray[6].ToString();
            string strQuestion = thisquestionArray[2];
            string strParsing = thisquestionArray[8];
            string strComments = thisquestionArray[9];
            string[] questionArray = strQuestion.Split(new char[] { '?' });
            string[] parsingArray = strParsing.Split(new char[] { '?' });
            string[] commentsArray = strComments.Split(new char[] { '?' });

            for (int i = 0; i < questionArray.Length; i++)
            {
                question.Text += questionArray[i].ToString() + "\n";
            }
            for (int i = 0; i < parsingArray.Length; i++)
            {
                parsing.Text += parsingArray[i].ToString() + "\n";
            }
            for (int i = 0; i < commentsArray.Length; i++)
            {
                comments.Text += commentsArray[i].ToString() + "\n";
            }

        }

        private void btnPush_Click(object sender, EventArgs e)
        {
            if (isPush)
            {
                if (rdoAnswerA.Checked == false && rdoAnswerB.Checked == false && rdoAnswerC.Checked == false && rdoAnswerD.Checked == false)
                {
                    MessageBox.Show("你还没有选择答案哦");
                    return;
                }
                //判断是否答对  对
                if (lblmyAnswer.Text == questionArray[7].ToString())
                {
                    stateList.Add(false.ToString());
                    if (stateList[qId - 1].ToString() == true.ToString())
                    {
                        btnPush.Enabled = false;
                    }
                    else
                    {
                        btnPush.Enabled = false;
                        qId = int.Parse(questionArray[0].ToString());
                        lblanswer.Text = questionArray[7].ToString();
                        picRight.Visible = true;
                        picwrong.Visible = false;
                        //numArray[qId] = true.ToString();
                        stateList[qId - 1] = true.ToString();
                    }
                }
                //判断是否答对  错
                else
                {
                    stateList.Add(false.ToString());
                    if (stateList[qId - 1].ToString() == true.ToString())
                    {
                        btnPush.Enabled = false;
                    }
                    else
                    {
                        Wrong = true;
                        collection = false;
                        isRead = true;
                        isExit = false; lblanswer.Text = questionArray[7].ToString();
                        //这要写入数据库，记录邮箱，题号，题库号，我的答案，是否错题，是否收藏。
                        //写入题号id，题库id，邮箱，错题
                        answerArray = BLL.bll.loadAnswer(int.Parse(questionArray[0].ToString()), int.Parse(questionArray[1].ToString()), SuserEmail, lblmyAnswer.Text, Wrong, collection, isRead, isExit);
                        picRight.Visible = false;
                        picwrong.Visible = true;
                        //numArray[qId] = true.ToString();
                        stateList[qId - 1] = true.ToString();
                    }
                }
                isPush = false;
                btnPush.Enabled = isPush;
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (isPush == true)//如果按钮没有被点击
            {
                if (stateList[qId - 2].ToString() != string.Empty)
                {
                    MessageBox.Show("Test");
                }
                //stateList.Add(false.ToString());//初始化按钮是否被点击
                else
                {
                    stateList[qId - 1] = false;
                }
            }
            if (stateList[qId - 2].ToString() == true.ToString())
            {
                btnPush.Enabled = false;
            }
            if (qId <= 1)
            {
                questionArray = BLL.bll.loadPaper(qBId, qId);
                fill(questionArray, true);
                isPush = true;
                return;
            }
            if (qId > 1)
            {
                qId--;
                questionArray = BLL.bll.loadPaper(qBId, qId);
                fill(questionArray, true);
                isPush = true;
                //btnPush.Enabled = isPush;
                lblanswer.Text = string.Empty;
                lblmyAnswer.Text = string.Empty;
                picRight.Visible = false;
                picwrong.Visible = false;
                rdoAnswerA.Checked = false;
                rdoAnswerB.Checked = false;
                rdoAnswerC.Checked = false;
                rdoAnswerD.Checked = false;
            }
            //else
            //{
            //    stateList[qId - 1] = true.ToString();
            //}
        }

        private void btnDowm_Click(object sender, EventArgs e)
        {
            //if (isPush==true)
            //{
            //    stateList.Add(false.ToString());//初始化按钮是否被点击 上下题，输入输出没有控制好
            //    if (stateList[qId-1].ToString()!=string.Empty)
            //    {
            //        stateList.Add(false.ToString());//初始化按钮是否被点击
            //    }
            //}
            //if (stateList[qId - 1].ToString() == true.ToString())
            //{
            btnPush.Enabled = false;
            //}
            if (qId >= 1)
            {
                qId++;
                questionArray = BLL.bll.loadPaper(qBId, qId);
                fill(questionArray, true);
                isPush = true;
                btnPush.Enabled = isPush;
                lblanswer.Text = string.Empty;
                lblmyAnswer.Text = string.Empty;
                picRight.Visible = false;
                picwrong.Visible = false;
                rdoAnswerA.Checked = false;
                rdoAnswerB.Checked = false;
                rdoAnswerC.Checked = false;
                rdoAnswerD.Checked = false;
            }
        }

        #region


        #endregion

        #region 移动窗体
        Point mouseOff;//鼠标移动位置变量
        bool leftFlag;//标记是否为左键
        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }
        private void Login_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);  //设置移动后的位置
                Location = mouseSet;
            }
        }
        private void Login_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }
        #endregion

        #region 控制选项，及选项文本
        /// <summary>
        /// 控制选项，及选项文本
        /// </summary>
        private void myAnswertxt()
        {

            if (isPush)
            {
                if (rdoAnswerA.Checked == true)
                {
                    lblmyAnswer.Text = "A";
                }
                if (rdoAnswerB.Checked == true)
                {
                    lblmyAnswer.Text = "B";
                }
                if (rdoAnswerC.Checked == true)
                {
                    lblmyAnswer.Text = "C";
                }
                if (rdoAnswerD.Checked == true)
                {
                    lblmyAnswer.Text = "D";
                }
            }
        }

        private void rdoAnswerA_CheckedChanged(object sender, EventArgs e)
        {
            myAnswertxt();
        }

        private void rdoAnswerB_CheckedChanged(object sender, EventArgs e)
        {
            myAnswertxt();
        }

        private void rdoAnswerC_CheckedChanged(object sender, EventArgs e)
        {
            myAnswertxt();
        }

        private void rdoAnswerD_CheckedChanged(object sender, EventArgs e)
        {
            myAnswertxt();
        }
        #endregion

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //String strConn = "Data Source=.;Initial Catalog=His;User ID=sa;Password=*****";
            //SqlConnection conn = new SqlConnection(strConn);
            //String sql = "select * from EMPLOYEE ";
            //conn.Open();
            //SqlCommand cmd = new SqlCommand(sqlId, conn);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataSet ds = new DataSet();
            //da.Fill(ds, "EMPLOYEE");
            //dataGridView1.DataSource = ds;
            //this.dataGridView1.AutoGenerateColumns = false;//是否自动生成列
            //dataGridView1.DataMember = "EMPLOYEE";
            //conn.Close(); 

        }

        private void miN_MAX_EXIT1_Load(object sender, EventArgs e)
        {
            miN_MAX_EXIT1.btnMax.Enabled = true;
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Hide();
            Form admin = new admin();
            admin.Show();
            //if (Form.ActiveForm.WindowState != FormWindowState.Maximized)
            //{
            //    Form.ActiveForm.WindowState = FormWindowState.Normal;
            //    dataGridView1.Width = this.Width;
            //}
            //else
            //{
            //    Form.ActiveForm.WindowState = FormWindowState.Maximized;
            //    dataGridView1.Width = this.Width;
            //    //751, 742
            //}
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
