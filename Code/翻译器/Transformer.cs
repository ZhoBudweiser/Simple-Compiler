using System;
using System.Collections.Generic;
using System.Text;

namespace 编译翻译器
{
    public class Transformer
    {
        public class Quartrtte
        {
            public int quad;    //当前四元式位置
            public string op;   //运算符
            public string arg1; //操作数1
            public string arg2; //操作数2
            public string res;  //结果
        }
        public List<Quartrtte> QuarList = new List<Quartrtte>();

        private int nextquad = 100;

        private int tempNum = 0;

        private Queue<string> insys = new Queue<string>();//常数和符号

        private class Undef
        {
            public int quad;
        }
        private class Sen
        {
            public List<int> nextlist;
        }
        private class Ctrl
        {
            public List<int> truelist;
            public List<int> falselist;
        }
        private class Sys
        {
            public string place;
        }

        private Stack<Undef> NStack = new Stack<Undef>();
        private Stack<Sen> SenStack = new Stack<Sen>();
        private Stack<Ctrl> CtlStack = new Stack<Ctrl>();
        private Stack<Sys> SysStack = new Stack<Sys>();

        void emit(Quartrtte qua)
        {
            QuarList.Add(qua);
            nextquad++;
        }

        List<int> makelist(int quad)
        {
            List<int> l = new List<int>();

            l.Add(quad);

            return l;
        }
        string newtemp()
        {
            tempNum++;
            return "T" + tempNum.ToString();
        }


        void backpatch(List<int> list, int quad)
        {
            foreach (var q in QuarList)
            {
                foreach (var l in list)
                {
                    if (q.quad == l)
                    {
                        q.res = quad.ToString();
                    }
                }
            }
        }

        List<int> merge(List<int> l1, List<int> l2)
        {
            List<int> l = new List<int>();

            foreach (var item in l1)
            {
                foreach (var jtem in l2)
                {
                    if (!l.Contains(item))
                    {
                        l.Add(item);
                    }
                    if (!l.Contains(jtem))
                    {
                        l.Add(jtem);
                    }
                }
            }

            return l;
        }

        void Action1(Sen X, Ctrl W, Sen S, Undef N3, Undef N1)
        {
            backpatch(X.nextlist, N3.quad);
            backpatch(W.truelist, N1.quad);
            S.nextlist = W.falselist;
            Quartrtte q = new Quartrtte();
            q.quad = nextquad;
            q.op = "j";
            q.arg1 = "-";
            q.arg2 = "-";
            q.res = N3.quad.ToString();
            emit(q);
        }

        void Action2(Ctrl W, Ctrl B, Sen D, Undef N)
        {
            W.truelist = B.truelist;
            W.falselist = B.falselist;
            backpatch(D.nextlist, N.quad);
        }

        void Action3(Sys E)
        {
            Quartrtte q = new Quartrtte();
            q.quad = nextquad;
            q.op = "=";
            q.arg1 = E.place;
            q.arg2 = "-";
            q.res = insys.Dequeue();
            emit(q);
        }

        void Action4(Sys E)
        {
            E.place = insys.Dequeue();
        }

        void Action5(Ctrl B)
        {
            B.truelist = makelist(nextquad);
            B.falselist = makelist(nextquad + 1);

            Quartrtte q1 = new Quartrtte();
            q1.quad = nextquad;
            q1.arg1 = insys.Dequeue();
            q1.op = "j" + insys.Dequeue();
            q1.arg2 = insys.Dequeue();
            q1.res = "-";
            emit(q1);
            Quartrtte q2 = new Quartrtte();
            q2.quad = nextquad;
            q2.op = "j";
            q2.arg1 = "-";
            q2.arg2 = "-";
            q2.res = "-";
            emit(q2);
        }

        void Action6(Sen D)
        {
            D.nextlist = makelist(nextquad + 1);

            Quartrtte q1 = new Quartrtte();
            q1.quad = nextquad;
            q1.arg1 = insys.Dequeue();
            q1.op = "+";
            q1.arg2 = "1";
            q1.res = q1.arg1;
            emit(q1);
            Quartrtte q2 = new Quartrtte();
            q2.quad = nextquad;
            q2.op = "j";
            q2.arg1 = "-";
            q2.arg2 = "-";
            q2.res = "-";
            emit(q2);
        }

        void Action7(Sen D)
        {
            D.nextlist = makelist(nextquad + 1);

            Quartrtte q1 = new Quartrtte();
            q1.quad = nextquad;
            q1.arg1 = insys.Dequeue();
            q1.op = "-";
            q1.arg2 = "1";
            q1.res = q1.arg1;
            emit(q1);
            Quartrtte q2 = new Quartrtte();
            q2.quad = nextquad;
            q2.op = "j";
            q2.arg1 = "-";
            q2.arg2 = "-";
            q2.res = "-";
            emit(q2);
        }

        void Action8(Sen X, Sen A)
        {
            A.nextlist = makelist(nextquad);
            X.nextlist = A.nextlist;

        }

        void Action9(Sen X, Sen M)
        {
            X.nextlist = M.nextlist;
        }

        void Action10(Sen M1, Sen M, Sen A, Undef N)
        {
            A.nextlist = makelist(nextquad);
            //backpatch(M1.nextlist, N.quad);
            M.nextlist = A.nextlist;
        }

        void Action11(Sen M, Sen A)
        {
            A.nextlist = makelist(nextquad);
            M.nextlist = A.nextlist;
        }

        void Action12(Undef N)
        {
            N.quad = nextquad;
        }

        void Action13(Sys E, Sys E1, Sys E2)
        {
            E.place = newtemp();
            Quartrtte q = new Quartrtte();
            q.quad = nextquad;
            q.op = "+";
            q.arg1 = E1.place;
            q.arg2 = E2.place;
            q.res = E.place;
            emit(q);
        }

        public void setSys(Queue<string> sys)
        {
            insys = sys;
            foreach (var item in insys)
            {
                System.Console.Write(item + "  ");
            }
            System.Console.WriteLine();
        }

        public void Action(int n)
        {
            Ctrl W = new Ctrl();
            Ctrl B = new Ctrl();
            Sen X = new Sen();
            Sen M = new Sen();
            Sen M1 = new Sen();
            Sen A = new Sen();
            Sen D = new Sen();
            Sen S = new Sen();
            Sys E = new Sys();
            Sys E1 = new Sys();
            Sys E2 = new Sys();
            Undef N = new Undef();
            Undef N0 = new Undef();
            Undef N1 = new Undef();
            switch (n)
            {
                case 1:
                    N1 = NStack.Pop();
                    N0 = NStack.Pop();
                    X = SenStack.Pop();
                    W = CtlStack.Pop();
                    Action1(X, W, S, N0, N1);
                    SenStack.Push(S);
                    break;
                case 2:
                    B = CtlStack.Pop();
                    D = SenStack.Pop();
                    N1 = NStack.Pop();
                    N0 = NStack.Pop();
                    Action2(W, B, D, N0);
                    CtlStack.Push(W);
                    NStack.Push(N1);
                    break;
                case 3:
                    E = SysStack.Pop();
                    Action3(E);
                    break;
                case 4:
                    Action4(E);
                    SysStack.Push(E);
                    break;
                case 5:
                    Action5(B);
                    CtlStack.Push(B);
                    break;
                case 6:
                    Action6(D);
                    SenStack.Push(D);
                    break;
                case 7:
                    Action7(D);
                    SenStack.Push(D);
                    break;
                case 8:
                    Action8(X, A);
                    SenStack.Push(X);
                    break;
                case 9:
                    M = SenStack.Pop();
                    Action9(X, M);
                    SenStack.Push(X);
                    break;
                case 10:
                    M1 = SenStack.Pop();
                    N = NStack.Pop();
                    Action10(M1, M, A, N);
                    SenStack.Push(M);
                    break;
                case 11:
                    Action11(M, A);
                    SenStack.Push(M);
                    break;
                case 12:
                    Action12(N);
                    NStack.Push(N);
                    break;
                case 13:
                    E2 = SysStack.Pop();
                    E1 = SysStack.Pop();
                    string tmp = E1.place;
                    E1.place = E2.place;
                    E2.place = insys.Dequeue();
                    int cnt = insys.Count;
                    insys.Enqueue(tmp);
                    while (cnt-- > 0)
                    {
                        string str = insys.Dequeue();
                        insys.Enqueue(str);
                    }
                    Action13(E, E1, E2);
                    SysStack.Push(E);
                    break;
            }
        }
    }
}
