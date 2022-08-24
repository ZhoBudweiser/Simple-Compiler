using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace 编译翻译器
{

    public partial class Display : Form
    {
        #region 内部符号
        private String inString;//输入符号串
        private Queue<string> outsys = new Queue<string>();//常数和符号
        private Stack<int> statusStack=new Stack<int>();//状态栈
        private Stack<char> symbolStack = new Stack<char>();//符号栈
        //private List<char> Vt = new List<char>
        //{ 'f', '(', ')', 'a', 'b',';', '{', '}', '#' };//终结符集合
        //private List<char> Vn = new List<char>
        //{ 'S', 'W', 'M', 'X'};//非终结符集合
        //private List<string> production = new List<string>
        //{
        //    "", "S->f(W)X","W->a;b;a","X->a;","X->{M}","M->Ma;","M->a;"};//为保证序号和下标一致，第一个产生式为空，不使用
        private List<char> Vt = new List<char> 
        { 'f', '(', ')', 'i', '=','<',';', '+', '-', '{', '}', '#'};//终结符集合
        private List<char> Vn = new List<char> 
        { 'S', 'W', 'M','X','E','N','A', 'D', 'B'};//非终结符集合
        private List<string> production = new List<string> 
        {
            "", "S->f(NW)NX","W->A;NB;ND","A->i=E","E->i","B->i<0","D->i++","D->i--","X->A;","X->{M}","M->MNA;","M->A;","N->","E->E+E"};//为保证序号和下标一致，第一个产生式为空，不使用
        //private List<List<int>> GOTO = new List<List<int>> {//GOTO表
        //    new List<int> {1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,4,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,19 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,13,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
        //    new List<int> {-1,-1,-1,-1 },
                                                                         //};

        //private List<List<string>> ACTION = new List<List<string>> {//ACTION表
        //    new List<string> {"s2",null,null,null,null,null,null,null,null},
        //    new List<string> {null,null,null,null,null,null,null,null,"acc"},
        //    new List<string> {null,"s3",null,null,null,null,null,null,null},
        //    new List<string> {null,null,null,"s5",null,null,null,null,null},
        //    new List<string> {null,null,"s10",null,null,null,null,null,null},
        //    new List<string> {null,null,null,null,null,"s6",null,null,null},
        //    new List<string> {null,null,null,null,"s7",null,null,null,null},
        //    new List<string> {null,null,null,null,null,"s8",null,null,null},
        //    new List<string> {null,null,null,"s9",null,null,null,null,null},
        //    new List<string> {null,null,"r2",null,null,null,null,null,null},
        //    new List<string> {null,null,null,"s11",null,null,"s12",null,null},
        //    new List<string> {null,null,null,null,null,"s20",null,null,null},
        //    new List<string> {null,null,null,"s14",null,null,null,null,null},
        //    new List<string> {null,null,null,"s17",null,null,null,"s16",null},
        //    new List<string> {null,null,null,null,null,"s15",null,null,null},
        //    new List<string> {null,null,null,"r6",null,null,null,"r6",null},
        //    new List<string> {null,null,null,null,null,null,null,null,"r4"},
        //    new List<string> {null,null,null,null,null,"s18",null,null,null},
        //    new List<string> {null,null,null,"r5",null,null,null,"r5",null},
        //    new List<string> {null,null,null,"r5",null,null,null,null,"r1"},
        //    new List<string> {null,null,null,null,null,null,null,null,"r3"},
        //};

        private List<List<int>> GOTO = new List<List<int>> {//GOTO表
            new List<int> {1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,4,-1,-1,-1},
            new List<int> {-1,5,-1,-1,-1,-1,6,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,11,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,12,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,13,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,16,-1,-1,15,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,18},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,23,-1,-1,-1,22,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,26,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,28,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,30,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,32,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,33,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
            new List<int> {-1,-1,-1,-1,-1,-1,-1,-1,-1},
        };

        private List<List<string>> ACTION = new List<List<string>> {//ACTION表
            new List<string> {"s2",null,null,null,null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,null,null,null,null,null,null,null,null,"acc"},
            new List<string> {null,"s3",null,null,null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,"r12",null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,"s7",null,null,null,null,null,null,null,null},
            new List<string> {null,null,"s8",null,null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,null,null,null,"s9",null,null,null,null,null},
            new List<string> {null,null,null,null,"s10",null,null,null,null,null,null,null},
            new List<string> {null,null,null,"r12",null,null,null,null,null,"r12",null,null},
            new List<string> {null,null,null,"r12",null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,"s14",null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,"s7",null,null,null,null,null,"s17",null,null},
            new List<string> {null,null,null,"s19",null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,null,null,null,"r3","s20",null,null,null,null},
            new List<string> {null,null,null,null,null,null,"r4","r4",null,null,null,null},
            new List<string> {null,null,null,null,null,null,"s21",null,null,null,null,null},
            new List<string> {null,null,null,null,null,null,null,null,null,null,null,"r1"},
            new List<string> {null,null,null,"s7",null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,null,null,null,"s24",null,null,null,null,null},
            new List<string> {null,null,null,null,null,"s25",null,null,null,null,null,null},
            new List<string> {null,null,null,"s14",null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,null,null,null,null,null,null,null,null,"r8"},
            new List<string> {null,null,null,null,null,null,"s27",null,null,null,null,null},
            new List<string> {null,null,null,"r12",null,null,null,null,null,null,"s29",null},
            new List<string> {null,null,null,"r12",null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,"s31",null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,null,null,null,"r13","s20",null,null,null,null},
            new List<string> {null,null,null,"r11",null,null,null,null,null,null,"r11",null},
            new List<string> {null,null,null,"s7",null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,null,null,null,null,null,null,null,null,"r9"},
            new List<string> {null,null,null,"s34",null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,null,null,null,"r5",null,null,null,null,null},
            new List<string> {null,null,null,null,null,null,"s35",null,null,null,null,null},
            new List<string> {null,null,"r2",null,null,null,null,null,null,null,null,null},
            new List<string> {null,null,null,null,null,null,null,"s36","s37",null,null,null},
            new List<string> {null,null,null,"r10",null,null,null,null,null,null,"r10",null},
            new List<string> {null,null,null,null,null,null,null,"s38",null,null,null,null},
            new List<string> {null,null,null,null,null,null,null,null,"s39",null,null,null},
            new List<string> {null,null,"r6",null,null,null,null,null,null,null,null,null},
            new List<string> {null,null,"r7",null,null,null,null,null,null,null,null,null},
        };
        private int ip=0;//输入指针
        private class Log
        {
            public string strStatus;
            public string strSymbol;
            public string strString;
            public string strAction;
        }
        private List<Log> logList = new List<Log>();//记录分析过程

        private bool isGoOn = false;

        private string strReadFilePath = "";

        private List<string> WordAnalyzeRes = new List<string>();
        private List<string> GrammarAnalyzeRes = new List<string>();
        private List<string> TransformerRes = new List<string>();

        public Display()
        {
            InitializeComponent();
        }
        #endregion

        #region LR总控程序
        //总控初始化
        public void initial()
        {
            ip = 0;
            statusStack.Push(0);
            symbolStack.Push('#');
        }

        public void LR1()
        {
            initial();
            int s;//状态栈栈顶
            char a; //ip所指向的符号
            int sp = 0;//几号状态
            string right = "";//产生式右部
            Transformer ts = new Transformer();
            ts.setSys(outsys);
            while (true)
            {
                s = statusStack.First();
                //System.Console.WriteLine(inString + ip.ToString());
                a = inString[ip];
                int index = getIndex(a);

                //tbRes.Text = index.ToString();
                System.Console.WriteLine(s.ToString()+"  "+index.ToString());
                if (index < 0 || ACTION[s][index] == null)
                {
                    //a不是终结符或者action表中没有对应信息
                    Error();
                    break;
                }
                else
                {
                    if (ACTION[s][index] == "acc")
                    {
                        //成功接收
                        tbRes.Text = tbRes.Text.Insert(tbRes.Text.Length, "语法分析正确\n");
                        WriteLog(1);
                        break;
                    }
                    else if (ACTION[s][index][0] == 's')
                    {
                        //移进动作
                        int next = Convert.ToInt32(ACTION[s][index].Remove(0, 1));//查找动作表，移进后转到第几个状态
                        WriteLog(2, next, s, a);

                        symbolStack.Push(a);
                        statusStack.Push(next);
                        ++ip;

                    }
                    else if (ACTION[s][index][0] == 'r')
                    {
                        //归约动作
                        int n = Convert.ToInt32(ACTION[s][index].Remove(0, 1));//查找动作表，用第几个产生式归约
                        right = GetRight(n);//获取第n个产生式的右部
                        sp = statusStack.ElementAt(right.Length);//几号状态
                        int k = GOTO[sp][getNIndex(production[n][0])];//查找goto表，转到第几号状态
                        WriteLog(3, n, sp, production[n][0], k);

                        for (int i = 0; i < right.Length; i++)
                        {
                            symbolStack.Pop();
                            statusStack.Pop();
                        }

                        statusStack.Push(k);
                        symbolStack.Push(production[n][0]);

                        ts.Action(n);
                    }
                }
            }
            //foreach (var item in ts.QuarList)
            {
                //System.Console.WriteLine(item.quad.ToString() + "\t" + item.op + "\t" + item.arg1 + "\t" + item.arg2 + "\t" + item.res);
            }
            if (isGoOn)
            {
               ShowTransProcedure(ts);
            }

        }
        #endregion

        #region 中间函数
        //获得非终结符在Vn中的索引
        public int getNIndex(char target)
        {
            for (int i = 0; i < Vn.Count; i++)
            {
                if (Vn[i]==target)
                {
                    return i;
                }
            }
            return -1;
        }
        //获得终结符在Vt中的索引
        public int getIndex(char target)
        {
            for (int i = 0; i < Vt.Count; i++)
            {
                if (Vt[i] == target)
                {
                    return i;
                }
            }
            return -1;
        }
        //根据索引获得产生式的右部
        public string GetRight(int index)
        {
            try
            {
                return production[index].Remove(0, 3);
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
                throw;
            }
            
        }
        public void Error()
        {
            tbRes.Text = tbRes.Text.Insert(tbRes.Text.Length, "语法分析出错\n");
            tbRes.BackColor = Color.Red;
            isGoOn = false;
            WriteLog(0);
        }
        public void WriteLog(int code,int n=-1,int firstParameter=-1, char secondParameter = ' ',int goWhere=-1)
        {
            //类型，状态/产生式，ACTION行、列，
            Log l = new Log();
            StringBuilder sStatus = new StringBuilder();//遍历状态栈，将栈中所有元素拼接成字符串
            Stack<string> s = new Stack<string>();//原栈的逆序遍历
            foreach (var item in statusStack)
            {
                s.Push(item.ToString());
            }
            foreach (var item in s)
            {
                sStatus.AppendFormat("{0} ",item);
            }
            l.strStatus = sStatus.ToString();
            l.strSymbol = new string(symbolStack.Reverse().ToArray());
            l.strString = inString.Remove(0, ip);
            
            switch (code)
            {
                case 0:
                    l.strAction = "出错";
                    break;
                case 1:
                    l.strAction = "Acc：分析成功";
                    break;
                case 2:
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("ACTION[{0},{1}]=S{2},状态{2}入栈", firstParameter, secondParameter,n);
                    System.Console.WriteLine(sb.ToString());
                    l.strAction = sb.ToString();
                    break;
                case 3:
                    StringBuilder sb1 = new StringBuilder();
                    sb1.AppendFormat("r{0}:{1}归约,GOTO({2},{3})={4}入栈", n, production[n], firstParameter, secondParameter, goWhere);
                    System.Console.WriteLine(sb1.ToString());
                    l.strAction = sb1.ToString();
                    break;
                default:
                    break;
            }
            logList.Add(l);
        }

        #endregion

        #region 开始
        private bool Init()
        {
            dgvProgress.Rows.Clear();
            dgvWords.Rows.Clear();
            dgvTrans.Rows.Clear();
            logList.Clear();
            statusStack.Clear();
            symbolStack.Clear();
            tbInstring.Clear();
            tbRes.Clear();
            tbRes.BackColor = Color.LightGray;
            isGoOn = true;

            return true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Init();

            //词法分析
            WordAnalyze wa = new WordAnalyze();
            
            if (File.Exists(strReadFilePath))
            {
                StreamReader sr = new StreamReader(strReadFilePath);
                wa.Analyze(sr);
                ShowWordAnalysisProcedure(wa);
                //获取用户的输入
                if (isGoOn)
                {
                    tbRes.Text = tbRes.Text.Insert(tbRes.Text.Length, "词法分析正确\n");
                    inString = wa.getOutstring();
                    tbInstring.Text = tbInstring.Text.Insert(tbInstring.Text.Length, inString + "\n");
                    inString += "#";
                }
                sr.Close();
            }
            else
            {
                System.Console.WriteLine("文件打开失败！");
                tbRes.Text = tbRes.Text.Insert(tbRes.Text.Length, "文件打开失败\n");
                isGoOn = false;
            }

            if (isGoOn)
            {
                outsys = wa.sys;

                ShowAnalysisTable();

                LR1();

                ShowAnalysisProcedure();
            }


        }
        #endregion

        #region 外观设置
        private void setdgvLayout()
        {
            dgvPrediction.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPrediction.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrediction.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPrediction.DefaultCellStyle.Font = new Font("微软雅黑", 10);
            dgvPrediction.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 10);
            dgvPrediction.TopLeftHeaderCell.Value = "状态";
        }
        //显示分析表
        public void ShowAnalysisTable()
        {
            dgvPrediction.Columns.Clear();

            int k;
            for (k = 0; k < Vt.Count; k++)
            {
                DataGridViewTextBoxColumn actionColumn = new DataGridViewTextBoxColumn();
                actionColumn.DefaultCellStyle.BackColor = Color.AliceBlue;
                actionColumn.HeaderCell.Value = Vt[k].ToString();
                dgvPrediction.Columns.Add(actionColumn);
            }
            for (int j = 0; k < Vn.Count + Vt.Count; k++, j++)
            {
                DataGridViewTextBoxColumn gotoColumn = new DataGridViewTextBoxColumn();
                gotoColumn.DefaultCellStyle.BackColor = Color.LightSkyBlue;
                gotoColumn.HeaderCell.Value = Vn[j].ToString();
                dgvPrediction.Columns.Add(gotoColumn);
            }
            for (int i = 0; i < ACTION.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = i.ToString();
                dgvPrediction.Rows.Add(row);
                int j;
                for (j = 0; j < ACTION[i].Count; j++)
                {
                    dgvPrediction.Rows[i].Cells[j].Value = ACTION[i][j];
                }
                for (int m = 0; j < ACTION[i].Count + GOTO[i].Count; j++, m++)
                {
                    if (GOTO[i][m] == -1)
                    {
                        continue;
                    }
                    else
                    {
                        dgvPrediction.Rows[i].Cells[j].Value = GOTO[i][m];
                    }
                }
            }
            setdgvLayout();
        }
        //显示分析过程
        public void ShowAnalysisProcedure()
        {
            GrammarAnalyzeRes.Clear();
            for (int i = 0; i < logList.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = i.ToString();
             
                dgvProgress.Rows.Add(row);

                dgvProgress.Rows[i].Cells[0].Value = i.ToString();
                dgvProgress.Rows[i].Cells[1].Value = logList[i].strStatus;
                dgvProgress.Rows[i].Cells[2].Value = logList[i].strSymbol;
                dgvProgress.Rows[i].Cells[3].Value = logList[i].strString;
                dgvProgress.Rows[i].Cells[4].Value = logList[i].strAction;

                string log = new string(i.ToString()+"\t"+ logList[i].strStatus + "\t" + logList[i].strSymbol + "\t" + logList[i].strString + "\t" + logList[i].strAction);
                GrammarAnalyzeRes.Add(log);

                if (i % 2 == 0)
                {
                    dgvProgress.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else
                {
                    dgvProgress.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                }
                if (logList[i].strAction == "出错")
                {
                    dgvProgress.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }

        public void ShowWordAnalysisProcedure(WordAnalyze wa)
        {
            WordAnalyzeRes.Clear();
            for (int i = 0; i < wa.logList.Count; i++)
            {
                if (wa.logList[i].strString != "")
                {
                    DataGridViewRow row = new DataGridViewRow();

                    dgvWords.Rows.Add(row);

                    dgvWords.Rows[i].Cells[0].Value = wa.logList[i].strString;
                    dgvWords.Rows[i].Cells[1].Value = wa.logList[i].strPair;
                    dgvWords.Rows[i].Cells[2].Value = wa.logList[i].strType;
                    dgvWords.Rows[i].Cells[3].Value = wa.logList[i].strLocation;

                    string log = new string(wa.logList[i].strString + "\t\t" + wa.logList[i].strPair + "\t\t" + wa.logList[i].strType + "\t\t" + wa.logList[i].strLocation);
                    WordAnalyzeRes.Add(log);

                    if (i%2 == 0)
                    {
                        dgvWords.Rows[i].DefaultCellStyle.BackColor = Color.LightYellow;
                    }
                    else
                    {
                        dgvWords.Rows[i].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                    }
                    if (wa.logList[i].strType == "Error")
                    {
                        dgvWords.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                        tbRes.Text = tbRes.Text.Insert(tbRes.Text.Length, "词法分析出错\n");
                        tbRes.BackColor = Color.Red;
                        isGoOn = false;
                    }
                }
                }

        }

        public void ShowTransProcedure(Transformer ts)
        {
            TransformerRes.Clear();
            for (int i = 0; i < ts.QuarList.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                dgvTrans.Rows.Add(row);

                dgvTrans.Rows[i].Cells[0].Value = ts.QuarList[i].quad;
                dgvTrans.Rows[i].Cells[1].Value = ts.QuarList[i].op;
                dgvTrans.Rows[i].Cells[2].Value = ts.QuarList[i].arg1;
                dgvTrans.Rows[i].Cells[3].Value = ts.QuarList[i].arg2;
                dgvTrans.Rows[i].Cells[4].Value = ts.QuarList[i].res;

                string log = new string(ts.QuarList[i].quad + "\t\t" + ts.QuarList[i].op + "\t\t" + ts.QuarList[i].arg1 + "\t\t" + ts.QuarList[i].arg2 + "\t\t" + ts.QuarList[i].res);
                TransformerRes.Add(log);

                if (i % 2 == 0)
                {
                    dgvTrans.Rows[i].DefaultCellStyle.BackColor = Color.LightPink;
                }
                else
                {
                    dgvTrans.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
                }
            }

        }

        //控制台设置-开关
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern void FreeConsole();//关闭控制台
        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }

        #endregion

        #region 输入输出
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "C:\\Users\\hw\\Desktop\\编译原理课程设计";
            ofd.Filter = "txt files (*.txt)|*.txt";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                strReadFilePath = ofd.FileName;
                lbPath.Text = strReadFilePath.Substring(strReadFilePath.LastIndexOf("\\") + 1); 
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FileStream fs;
            StreamWriter sw;

            if (isGoOn)
            {
                for (int i = 1; ;i++)
                {
                    if (!System.IO.File.Exists("C:\\Users\\hw\\Desktop\\编译原理课程设计\\输出文件\\WordAnalyzeRes"+i.ToString()+".txt"))
                    {
                        //没有则创建这个文件
                        fs = new FileStream("C:\\Users\\hw\\Desktop\\编译原理课程设计\\输出文件\\WordAnalyzeRes" + i.ToString() + ".txt", FileMode.Create, FileAccess.Write);//创建写入文件
                        sw = new StreamWriter(fs);
                        sw.WriteLine();
                        sw.WriteLine();
                        foreach (var item in WordAnalyzeRes)
                        {
                            sw.WriteLine(item);
                        }
                        sw.Close();
                        fs.Close();
                        break;
                    }
                }

                for (int i = 1; ; i++)
                {
                    if (!System.IO.File.Exists("C:\\Users\\hw\\Desktop\\编译原理课程设计\\输出文件\\GrammarAnalyzeRes" + i.ToString() + ".txt"))
                    {
                        //没有则创建这个文件
                        fs = new FileStream("C:\\Users\\hw\\Desktop\\编译原理课程设计\\输出文件\\GrammarAnalyzeRes" + i.ToString() + ".txt", FileMode.Create, FileAccess.Write);//创建写入文件
                        sw = new StreamWriter(fs);
                        sw.WriteLine();
                        sw.WriteLine();
                        foreach (var item in GrammarAnalyzeRes)
                        {
                            sw.WriteLine(item);
                        }
                        sw.Close();
                        fs.Close();
                        break;
                    }
                }


                for (int i = 1; ; i++)
                {
                    if (!System.IO.File.Exists("C:\\Users\\hw\\Desktop\\编译原理课程设计\\输出文件\\TransformerRes" + i.ToString() + ".txt"))
                    {
                        //没有则创建这个文件
                        fs = new FileStream("C:\\Users\\hw\\Desktop\\编译原理课程设计\\输出文件\\TransformerRes" + i.ToString() + ".txt", FileMode.Create, FileAccess.Write);//创建写入文件
                        sw = new StreamWriter(fs);
                        sw.WriteLine();
                        sw.WriteLine();
                        foreach (var item in TransformerRes)
                        {
                            sw.WriteLine(item);
                        }
                        sw.Close();
                        fs.Close();
                        break;
                    }
                }

                lbPath.Text = "保存成功";
            }
            else
            {
              lbPath.Text = "保存失败,请打开需要分析的文件";
            }

        }
        #endregion
    }
}
