using System;
using System.Collections.Generic;
using System.IO;

namespace 编译翻译器
{
    public class WordAnalyze
    {
        // #pragma warning(disable:4996)；
        private List<string> k = new List<string> { "do", "end", "for", "if", "printf", "scanf", "then", "while" };//1-关键字表
        private List<string> s1 = new List<string> { ",", ";", "(", ")", "[", "]", "{", "}" };//2-分界符表
        private List<string> s2 = new List<string> { "+", "-", "*", "/", "="};//3-算数运算符表
        private List<string> s3 = new List<string> { "<", "<=", "==", ">", ">=", "!=" };//4-关系运算符表
        private List<string> ci = new List<string>();//5-常数
        private List<string> id = new List<string>();//6-标识符
        public Queue<string> sys = new Queue<string>();//常数和符号

        private int row = 1, col = 0;
        private int cilen = 0;//常数表长度
        private int idlen = 0;//标识符表长度

        private string outstring = "";

        public class Log
        {
            public string strString;
            public string strPair;
            public string strType;
            public string strLocation;
        }
        public List<Log> logList = new List<Log>();//记录分析过程

        //判断是否为数字 
        bool IsDigit(char ch)
        {
            if (ch >= '0' && ch <= '9')
                return true;
            return false;
        }

        //判断是否为字母 
        bool IsLetter(char ch)
        {
            if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z')
                return true;
            return false;
        }

        bool IsKey(string instring)
        {
            int n = k.Count;
            for (int i = 0; i < n; i++)
            {
                if (instring == k[i])
                {
                    return true;
                }
            }
            return false;
        }

        bool IsInId(string instring)
        {
            for (int i = 0; i < idlen; i++)
            {
                if (instring == id[i])
                {
                    return true;
                }
            }
            return false;
        }

        int InsertId(string instring)
        {
            id.Add(instring);
            idlen++;

            return idlen - 1;
        }

        bool IsInConst(string instring)
        {
            for (int i = 0; i < cilen; i++)
            {
                if (instring == ci[i])
                {
                    return true;
                }
            }
            return false;
        }

        int InsertConst(string instring)
        {
            ci.Add(instring);
            cilen++;

            return cilen - 1;
        }

        bool IsInS1(string instring)
        {
            int n = s1.Count;
            for (int i = 0; i < n; i++)
            {
                if (instring == s1[i])
                {
                    return true;
                }
            }
            return false;
        }
        bool IsInS2(string instring)
        {
            int n = s2.Count;
            for (int i = 0; i < n; i++)
            {
                if (instring[0] == s2[i][0])
                {
                    return true;
                }
            }
            return false;
        }
        bool IsInS3(string instring)
        {
            int n = s3.Count;
            for (int i = 0; i < n; i++)
            {
                if (instring == s3[i])
                {
                    return true;
                }
            }
            return false;
        }

        string Dec2Bin(string instring)
        {
            int num = 0;
            int i;
            for (i = instring.Length - 1; i >= 0; i--)
            {
                num = num * 10 + instring[i] - '0';
            }
            return Convert.ToString(num, 2);
        }

        int fingPoint(string instring)
        {
            int num = 0;
            for (int i = 0; i < instring.Length; i++)
            {
                if (instring[i] == '.')
                {
                    num++;
                }
            }
            return num;
        }

        #region 词法分析总程序
        public void Analyze(StreamReader sr)
        {
            //System.Console.Clear();
            System.Console.WriteLine("单词\t\t二元序列\t\t类型\t\t位置(行, 列)");
            string instring;
            char ch;
            int loc = 0;
            outstring = "";
            logList.Clear();

            while (!sr.EndOfStream)
            {
                Log l = new Log();
                l.strString = "";
                ch = (char)sr.Read();
                loc++;
                instring = "";
                
                //判断空格换行
                if (ch == ' ' || ch == '\t' || ch == '\n')
                {
                    if (ch == '\n')
                    {
                        row++;
                        col = 0;
                    }
                }
                //判断注释
                else if (ch == '/')
                {
                    instring = ch.ToString();
                    ch = (char)sr.Read();
                    loc++;
                    if (ch == '/')
                    {
                        while (!sr.EndOfStream)
                        {
                            ch = (char)sr.Read();
                            loc++;
                            if (ch == '\n')
                            {
                                row++;
                                col = 0;
                                break;
                            }
                        }
                    }
                    else if (ch == '*')
                    {
                        while (!sr.EndOfStream)
                        {
                            ch = (char)sr.Read();
                            loc++;
                            if (ch == '*')
                            {
                                ch = (char)sr.Read();
                                loc++;
                                if (ch == '/')
                                    break;
                                else if (ch == '\n')
                                {
                                    row++;
                                    col = 0;
                                }
                            }
                            else if (ch == '\n')
                            {
                                row++;
                                col = 0;
                            }
                        }
                    }
                    else
                    {
                        col++;
                        l.strString = instring;
                        l.strPair = "(3," + instring + ")";
                        l.strType = "算术运算符";
                        l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                        System.Console.WriteLine(instring + "\t\t" + "(3," + instring + ")\t\t\t" + "算术运算符\t(" + row.ToString() + "," + col.ToString() + ")");
                        sr.DiscardBufferedData();
                        loc -= 1;
                        sr.BaseStream.Seek(loc, SeekOrigin.Begin);
                    }
                }
                //判断是否为关键字标识符
                else if (IsLetter(ch))
                {
                    while (IsLetter(ch) || IsDigit(ch))
                    {
                        instring = instring + ch;
                        ch = (char)sr.Read();
                        loc++;
                    }
                    if (IsKey(instring))//识别关键字
                    {
                        col++;
                        l.strString = instring;
                        l.strPair = "(1," + instring + ")";
                        l.strType = "关键字";
                        l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                        System.Console.WriteLine(instring + "\t\t" + "(1," + instring + ")\t\t\t" + "关键字\t\t(" + row.ToString() + "," + col.ToString() + ")");
                        if (instring == "for")  outstring += "f";
                    }
                    else//识别标识符
                    {
                        col++;
                        if (!IsInId(instring))
                        {
                            int ptr;
                            ptr = InsertId(instring);
                        }
                        l.strString = instring;
                        l.strPair = "(6," + instring + ")";
                        l.strType = "标识符";
                        l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                        System.Console.WriteLine(instring + "\t\t" + "(6," + instring + ")\t\t\t" + "标识符\t\t(" + row.ToString() + "," + col.ToString() + ")");
                        sys.Enqueue(instring);
                        outstring += "i";
                    }
                    sr.DiscardBufferedData();
                    loc -= 1;
                    sr.BaseStream.Seek(loc, SeekOrigin.Begin);

                }
                //判断是否为常数 
                else if (IsDigit(ch))
                {
                    while (IsDigit(ch) || ch == '.')
                    {
                        instring = instring + ch;
                        ch = (char)sr.Read();
                        loc++;
                    }
                    if (IsLetter(ch))//判断数字后是否紧跟字符，是则报错
                    {
                        while (IsLetter(ch) || IsDigit(ch))
                        {
                            instring = instring + ch;
                            ch = (char)sr.Read();
                            loc++;
                        }
                        col++;
                        l.strString = instring;
                        l.strPair = "Error";
                        l.strType = "Error";
                        l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                        System.Console.WriteLine(instring + "\t\t" + "Error\t\t\t" + "Error\t\t(" + row.ToString() + "," + col.ToString() + ")");
                    }
                    else
                    {
                        int num = fingPoint(instring);
                        if (num == 0)
                        {
                            string bin = "";
                            bin = Dec2Bin(instring);
                            if (!IsInConst(bin))
                            {
                                int ptr;
                                ptr = InsertConst(bin);
                            }
                            col++;
                            l.strString = instring;
                            l.strPair = "(5," + bin + ")";
                            l.strType = "常数";
                            l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                            System.Console.WriteLine(instring + "\t\t" + "(5," + bin + ")\t\t\t" + "常数\t\t(" + row.ToString() + "," + col.ToString() + ")");
                            sys.Enqueue(instring);
                            outstring += "i";
                        }
                        else if (num == 1)
                        {
                            col++;
                            l.strString = instring;
                            l.strPair = "(5," + instring + ")";
                            l.strType = "浮点数";
                            l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                            System.Console.WriteLine(instring + "\t\t" + "(5," + instring + ")\t\t\t" + "浮点数\t\t(" + row.ToString() + "," + col.ToString() + ")");
                            sys.Enqueue(instring);
                            outstring += "i";
                        }
                        else
                        {
                            col++;
                            l.strString = instring;
                            l.strPair = "Error";
                            l.strType = "Error";
                            l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                            System.Console.WriteLine(instring + "\t\t" + "Error\t\t" + "Error\t\t(" + row.ToString() + "," + col.ToString() + ")");
                        }
                    }
                    sr.DiscardBufferedData();
                    loc -= 1;
                    sr.BaseStream.Seek(loc, SeekOrigin.Begin);
                }
                //判断是否为符号 
                else if (s2.Contains(ch.ToString()) || s3.Contains(ch.ToString()) || s1.Contains(ch.ToString()))
                {
                    bool ins3 = false;
                    while (s3.Contains(ch.ToString()))
                    {
                        instring += ch.ToString();
                        ch = (char)sr.Read();
                        loc++;
                        ins3 = true;
                    }
                    if (ins3)
                    {
                        sr.DiscardBufferedData();
                        loc -= 1;
                        sr.BaseStream.Seek(loc, SeekOrigin.Begin);
                    }
                    //是否为分界符和算数运算符
                    else
                    {
                        instring = ch.ToString();
                        //System.Console.WriteLine();
                        //ch = (char)sr.Read();
                    }
                    //判断分界符
                    if (s1.Contains(instring))
                    {
                        col++;
                        l.strString = instring;
                        l.strPair = "(2," + instring + ")";
                        l.strType = "分界符";
                        l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                        System.Console.WriteLine(instring + "\t\t" + "(2," + instring + ")\t\t\t" + "分界符\t\t(" + row.ToString() + "," + col.ToString() + ")");
                        outstring += instring;
                    }
                    //判断算术运算符
                    else if (s2.Contains(instring))
                    {
                        col++;
                        l.strString = instring;
                        l.strPair = "(3," + instring + ")";
                        l.strType = "算术运算符";
                        l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                        System.Console.WriteLine(instring + "\t\t" + "(3," + instring + ")\t\t\t" + "算术运算符\t(" + row.ToString() + "," + col.ToString() + ")");
                        outstring += instring;

                    }
                    //判断关系运算符
                    else if (s3.Contains(instring))
                    {
                        col++;
                        l.strString = instring;
                        l.strPair = "(4," + instring + ")";
                        l.strType = "关系运算符";
                        l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                        System.Console.WriteLine(instring + "\t\t" + "(4," + instring + ")\t\t\t" + "关系运算符\t(" + row.ToString() + "," + col.ToString() + ")");
                        sys.Enqueue(instring);
                        outstring += "<";
                    }
                    else
                    {
                        col++;
                        l.strString = instring;
                        l.strPair = "Error";
                        l.strType = "Error";
                        l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                        System.Console.WriteLine(instring + "\t\t" + "Error\t\t" + "Error\t\t(" + row.ToString() + "," + col.ToString() + ")");

                    }
                }
                else if (instring != "")
                {
                    col++;
                    instring = ch.ToString();
                    l.strString = instring;
                    l.strPair = "Error";
                    l.strType = "Error";
                    l.strLocation = "(" + row.ToString() + ", " + col.ToString() + ")";
                    System.Console.WriteLine(instring+"\t\t" + "Error\t\t\t" + "Error\t\t(" + row.ToString() + "," + col.ToString() + ")");
                }
                if (l.strString != "")  logList.Add(l);
            }
        }

        #endregion

        public string getOutstring()
        {
            return outstring;
        }

        int Main()
        {
            string strReadFilePath = "test.txt";
            StreamReader srReadFile = new StreamReader(strReadFilePath);
            if (srReadFile.EndOfStream)
            {
                System.Console.WriteLine("文件打开失败！");
            }
            System.Console.WriteLine("单词\t\t二元序列\t\t类型\t\t位置(行, 列)");
//          WordAnalyze(srReadFile);
            srReadFile.Close();
            return 0;
        }
    }
}
