using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GameTree
{
    public class GameTree
    {
        //棋盘高度
        const int high = 10;
        //棋盘宽度
        const int length = 10;
        
        //搜索深度
        const int DEPTH = 3;

        int skip_num = 0;
        int pruning_num = 0;
        int search_num = 0;
        int cut_search_num = 0;
        int alpha = int.MinValue;
        int beta = int.MaxValue;

        //棋盘
        private int[,] Map = new int[high, length];

        //下一步
        Tuple<int, int> NextStep = new Tuple<int, int>(0, 0);

        //棋形分数
        Dictionary<Tuple<int, int, int, int, int, int>, int> FormationScore = new Dictionary<Tuple<int, int, int, int, int, int>, int>
        {
            {new Tuple<int, int, int, int, int, int>(1, 0, 0, 0, 0, 0), 10 },
            {new Tuple<int, int, int, int, int, int>(1, 1, 0, 0, 0, 0), 50 },
            {new Tuple<int, int, int, int, int, int>(0, 1, 1, 0, 0, 0), 100 },
            {new Tuple<int, int, int, int, int, int>(1, 1, 0, 1, 0, 0), 400 },
            {new Tuple<int, int, int, int, int, int>(0, 1, 1, 0, 1, 0), 800 },
            {new Tuple<int, int, int, int, int, int>(1, 1, 1, 0, 0, 0), 2000 },
            {new Tuple<int, int, int, int, int, int>(0, 1, 1, 1, 0, 0), 4000 },
            {new Tuple<int, int, int, int, int, int>(1, 1, 1, 0, 1, 0), 7000 },
            {new Tuple<int, int, int, int, int, int>(0, 1, 1, 1, 0, 1), 8000 },
            {new Tuple<int, int, int, int, int, int>(1, 1, 1, 1, 0, 0), 10000 },
            {new Tuple<int, int, int, int, int, int>(0, 1, 1, 1, 1, 0), 50000 },
            {new Tuple<int, int, int, int, int, int>(1, 1, 1, 1, 1, 0), int.MaxValue },

            {new Tuple<int, int, int, int, int, int>(2, 0, 0, 0, 0, 0), -10 },
            {new Tuple<int, int, int, int, int, int>(2, 2, 0, 0, 0, 0), -50 },
            {new Tuple<int, int, int, int, int, int>(0, 2, 2, 0, 0, 0), -200 },
            {new Tuple<int, int, int, int, int, int>(2, 2, 0, 2, 0, 0), -500 },
            {new Tuple<int, int, int, int, int, int>(2, 2, 2, 0, 0, 0), -2000 },
            {new Tuple<int, int, int, int, int, int>(0, 2, 2, 2, 0, 0), -4000 },
            {new Tuple<int, int, int, int, int, int>(2, 2, 2, 0, 2, 0), -7000 },
            {new Tuple<int, int, int, int, int, int>(0, 2, 2, 2, 0, 2), -8000 },
            {new Tuple<int, int, int, int, int, int>(2, 2, 2, 2, 0, 0), -10000 },
            {new Tuple<int, int, int, int, int, int>(0, 2, 2, 2, 2, 0), -50000 },
            {new Tuple<int, int, int, int, int, int>(2, 2, 2, 2, 2, 0), int.MinValue },

            {new Tuple<int, int, int, int, int, int>(2, 1, 0, 1, 0, 0), 10 },
            {new Tuple<int, int, int, int, int, int>(2, 1, 1, 0, 0, 0), 20 },
            {new Tuple<int, int, int, int, int, int>(2, 1, 1, 0, 1, 0), 200 },
            {new Tuple<int, int, int, int, int, int>(2, 1, 0, 1, 1, 0), 300 },
            {new Tuple<int, int, int, int, int, int>(2, 1, 1, 1, 0, 0), 500 },
            {new Tuple<int, int, int, int, int, int>(2, 0, 1, 1, 0, 1), 700 },
            {new Tuple<int, int, int, int, int, int>(2, 0, 1, 0, 1, 1), 800 },
            {new Tuple<int, int, int, int, int, int>(2, 0, 1, 1, 1, 0), 2000 },
            {new Tuple<int, int, int, int, int, int>(2, 1, 1, 1, 1, 0), 5000 },
            {new Tuple<int, int, int, int, int, int>(2, 1, 0, 1, 0, 1), 2000 },
            {new Tuple<int, int, int, int, int, int>(2, 1, 1, 0, 0, 1), 2000 },
            {new Tuple<int, int, int, int, int, int>(2, 1, 1, 0, 1, 1), 5000 },
            {new Tuple<int, int, int, int, int, int>(2, 1, 0, 1, 1, 1), 5000 },
            {new Tuple<int, int, int, int, int, int>(2, 1, 1, 1, 0, 1), 6000 },
            {new Tuple<int, int, int, int, int, int>(2, 0, 1, 1, 1, 1), 15000 },
            {new Tuple<int, int, int, int, int, int>(2, 1, 1, 1, 1, 1), int.MaxValue },

            {new Tuple<int, int, int, int, int, int>(1, 2, 0, 2, 0, 0), -10 },
            {new Tuple<int, int, int, int, int, int>(1, 2, 2, 0, 0, 0), -20 },
            {new Tuple<int, int, int, int, int, int>(1, 2, 2, 0, 2, 0), -200 },
            {new Tuple<int, int, int, int, int, int>(1, 2, 0, 2, 2, 0), -300 },
            {new Tuple<int, int, int, int, int, int>(1, 2, 2, 2, 0, 0), -500 },
            {new Tuple<int, int, int, int, int, int>(1, 0, 2, 2, 0, 2), -700 },
            {new Tuple<int, int, int, int, int, int>(1, 0, 2, 0, 2, 2), -800 },
            {new Tuple<int, int, int, int, int, int>(1, 0, 2, 2, 2, 0), -2000 },
            {new Tuple<int, int, int, int, int, int>(1, 2, 2, 2, 2, 0), -5000 },
            {new Tuple<int, int, int, int, int, int>(1, 2, 0, 2, 0, 2), -2000 },
            {new Tuple<int, int, int, int, int, int>(1, 2, 2, 0, 0, 2), -2000 },
            {new Tuple<int, int, int, int, int, int>(1, 2, 2, 0, 2, 2), -5000 },
            {new Tuple<int, int, int, int, int, int>(1, 2, 0, 2, 2, 2), -5000 },
            {new Tuple<int, int, int, int, int, int>(1, 2, 2, 2, 0, 2), -6000 },
            {new Tuple<int, int, int, int, int, int>(1, 0, 2, 2, 2, 2), -15000 },
            {new Tuple<int, int, int, int, int, int>(1, 2, 2, 2, 2, 2), int.MinValue },

        };

        //构造函数，初始化棋盘
        public GameTree()
        {
            for (int i = 0; i < high; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    Map[i, j] = 0;
                }
            }
            
            
        }

        //获取下一步行动(无逻辑处理)
        public Tuple<int, int> GetNextStep()
        {
            return NextStep;
        }

        //极大极小值搜索
        public int MinMaxSearch(bool isAI)
        {
            pruning_num = 0;
            if (isAI)
            {
                return Max(DEPTH);
            }
            else
            {
                return Min(DEPTH);
            }          
        }

        //alpha-beta剪枝搜索(封装, 记录信息并返回下一步)
        public Tuple<int, int> AlphaBetaSearch()
        {

            Console.WriteLine("搜索深度: {0}", DEPTH);
            Console.WriteLine("high: {0}, length: {1}", high, length);
            Console.WriteLine("初始化逻辑棋盘...");
            PrintMap();


            search_num = 0;
            pruning_num = 0;
            cut_search_num = 0;
            alpha = int.MinValue;
            beta = int.MaxValue;
            Console.WriteLine("开始计算...");
            TimeSpan ts1 = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            AlphaBetaSearch(true);
            TimeSpan ts2 = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            PrintMap();

            Console.WriteLine("计算时间: {0}", ts2-ts1);
            Console.WriteLine("搜索次数: {0}", search_num);
            Console.WriteLine("跳过搜索的格子数目: {0}", skip_num);
            Console.WriteLine("剪枝次数: {0}", pruning_num);
            Console.WriteLine("剪枝优化掉的搜索次数: {0}", cut_search_num);      
            Console.WriteLine("AI: ({0}, {1})", NextStep.Item1, NextStep.Item2);

            search_num = 0;
            pruning_num = 0;
            cut_search_num = 0;
            alpha = int.MinValue;
            beta = int.MaxValue;

            return NextStep;
        }

        //alpha-beta剪枝搜索
        public int AlphaBetaSearch(bool isAI)
        {

            
            if (isAI)
            {
                return Max(DEPTH, alpha, beta);
            }
            else
            {
                return Min(DEPTH, alpha, beta);
            }
        }

        //极大值搜索
        private int Max(int depth)
        {
            int best = int.MinValue;
            if (depth <= 0 || GameOver() > 0)
            {
                return Evaluate();
            }
            List<Tuple<int, int>> nextPositionList = NextMovePosition();
            for (int i = 0; i < nextPositionList.Count; i++)
            {
                
                MakeNextMove(nextPositionList[i], true);
                int value = Min(depth - 1);
                UnmakeMove(nextPositionList[i]);
                if (value > best)
                {
                    best = value;
                    if (depth == DEPTH)
                    {
                        NextStep = nextPositionList[i];
                    }
                }


                if (depth == DEPTH)
                {
                    Console.WriteLine("value:{0} , step:({1},{2}) ||| best:{3} , best_step:({4},{5})", value, nextPositionList[i].Item1 + 1, nextPositionList[i].Item2 + 1, best, NextStep.Item1 + 1, NextStep.Item2 + 1);

                }

            }

            return best;

        }

        //极小值搜索
        private int Min(int depth)
        {
            int best = int.MaxValue;
            if (depth <= 0 || GameOver() > 0)
            {
                return Evaluate();
            }
            List<Tuple<int, int>> nextPositionList = NextMovePosition();
            for (int i = 0; i < nextPositionList.Count; i++)
            {
                MakeNextMove(nextPositionList[i], false);
                int value = Max(depth - 1);
                UnmakeMove(nextPositionList[i]);
                if (value < best)
                {
                    best = value;
                }
            }
            return best;
        }

        //极大值搜索(alpha剪枝)
        private int Max(int depth, int alpha, int beta)
        {
            search_num++;
            int v = GameOver();
            if (depth <= 0)
            {
                return Evaluate();
            }
            if (v == 1)
            {
                return int.MaxValue;
            }
            if (v == 2)
            {
                return int.MinValue;
            }
            List<Tuple<int, int>> nextPositionList = NextMovePosition();
            for (int i = 0; i < nextPositionList.Count; i++)
            {

                if (!CheckRound(nextPositionList[i]))
                {
                    skip_num++;
                    continue;
                }

                MakeNextMove(nextPositionList[i], true);
                int value = Min(depth - 1, alpha, beta);
                UnmakeMove(nextPositionList[i]);
                if (value > alpha)
                {

                    alpha = value;
                    if (depth == DEPTH)
                    {
                        NextStep = nextPositionList[i];
                    }
                    if (alpha >= beta)
                    {
                        pruning_num++;
                        cut_search_num = cut_search_num + (nextPositionList.Count - i)*Convert.ToInt32(Math.Pow(nextPositionList.Count, depth-1));
                        return alpha; 
                    }
                }

                if (depth == DEPTH)
                {

                    Console.WriteLine("value:{0} , step:({1},{2}) ||| alpha:{3} , beta: {4} , best_step:({5},{6})", value, nextPositionList[i].Item1 + 1, nextPositionList[i].Item2 + 1, alpha, beta, NextStep.Item1 + 1, NextStep.Item2 + 1);

                }
            }

            return alpha;

        }

        //极小值搜索(beta剪枝)
        private int Min(int depth, int alpha, int beta)
        {
            search_num++;
            int v = GameOver();
            if (depth <= 0)
            {
                return Evaluate();
            }
            if(v == 1)
            {
                return int.MaxValue;
            }
            if(v == 2)
            {
                return int.MinValue;
            }
            List<Tuple<int, int>> nextPositionList = NextMovePosition();
            for (int i = 0; i < nextPositionList.Count; i++)
            {
                if (!CheckRound(nextPositionList[i]))
                {
                    skip_num++;
                    continue;
                }

                MakeNextMove(nextPositionList[i], false);
                int value = Max(depth - 1, alpha, beta);
                UnmakeMove(nextPositionList[i]);
                if (value < beta)
                {
                    beta = value;
                    if (alpha >= beta)
                    {
                        pruning_num++;
                        cut_search_num = cut_search_num + (nextPositionList.Count - i) * Convert.ToInt32(Math.Pow(nextPositionList.Count, depth - 1));
                        return beta;
                    }
                }
            }
            return beta;
        }

        //评估函数
        private int Evaluate()
        {
            int score = 0;
            int[] values;
            int k = 1;
            for (int i = 0; i < high; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    for (int m = 0; m < 8; m++)
                    {
                        values = new int[6];
                        switch (m)
                        {

                            case 0:
                                for (int n = 0; n < 6; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (j + n >= 0 && j + n < length)
                                    {
                                        values[n] = Map[i, j + n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 1:
                                for (int n = 0; n < 6; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (j + n >= 0 && j + n < length && i + n >= 0 && i + n < high)
                                    {
                                        values[n] = Map[i + n, j + n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 2:
                                for (int n = 0; n < 6; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (i + n >= 0 && i + n < high)
                                    {
                                        values[n] = Map[i + n, j];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 3:
                                for (int n = 0; n < 6; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (i + n >= 0 && i + n < high && j - n >= 0 && j - n < length)
                                    {
                                        values[n] = Map[i + n, j - n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 4:
                                for (int n = 0; n < 6; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (j - n >= 0 && j - n < length)
                                    {
                                        values[n] = Map[i, j - n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 5:
                                for (int n = 0; n < 6; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (j - n >= 0 && j - n < length && i - n >= 0 && i - n < length)
                                    {
                                        values[n] = Map[i - n, j - n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 6:
                                for (int n = 0; n < 6; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (i - n >= 0 && i - n < length)
                                    {
                                        values[n] = Map[i - n, j];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 7:
                                for (int n = 0; n < 6; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (j + n >= 0 && j + n < length && i - n >= 0 && i - n < length)
                                    {
                                        values[n] = Map[i - n, j + n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                        }
                        if (k == 1)
                        {
                            score = score + CalcScore(values);
                        }
                        else
                        {
                            k = 1;
                        }
                    }
                }
            }

            return score;
        }

        //计算分数
        private int CalcScore(int[] values)
        {
            Tuple<int, int, int, int, int, int> formation = new Tuple<int, int, int, int, int, int>(values[0], values[1], values[2], values[3], values[4], values[5]);
            if (FormationScore.ContainsKey(formation))
            {
                return FormationScore[formation];
            }
            else
            {
                return 0;
            }
        }

        //进行下一步行动
        public void MakeNextMove(Tuple<int, int> position, bool isAI)
        {
            if (isAI)
            {
                Map[position.Item1, position.Item2] = 1;
            }
            else
            {
                Map[position.Item1, position.Item2] = 2;
            }

        }

        //撤回行动
        private void UnmakeMove(Tuple<int, int> position)
        {
            Map[position.Item1, position.Item2] = 0;
        }

        //下一步的所有走法
        private List<Tuple<int, int>> NextMovePosition()
        {
            List<Tuple<int, int>> positions = new List<Tuple<int, int>>();
            for (int i = 0; i < high; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (Map[i, j] == 0)
                    {
                        positions.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
            return positions;
        }

        //判断游戏是否结束;
        // 0:未结束; 1:A胜利; 2:B胜利;
        public int GameOver()
        {
            int[] values;
            int k = 1;
            int l = 999;
            for (int i = 0; i < high; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (Map[i, j]<l)
                    {
                        l = Map[i, j];
                    }
                    for (int m = 0; m < 8; m++)
                    {
                        values = new int[6];
                        switch (m)
                        {

                            case 0:
                                for (int n = 0; n < 5; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (j + n >= 0 && j + n < length)
                                    {
                                        values[n] = Map[i, j + n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 1:
                                for (int n = 0; n < 5; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (j + n >= 0 && j + n < length && i + n >= 0 && i + n < high)
                                    {
                                        values[n] = Map[i + n, j + n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 2:
                                for (int n = 0; n < 5; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (i + n >= 0 && i + n < high)
                                    {
                                        values[n] = Map[i + n, j];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 3:
                                for (int n = 0; n < 5; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (i + n >= 0 && i + n < high && j - n >= 0 && j - n < length)
                                    {
                                        values[n] = Map[i + n, j - n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 4:
                                for (int n = 0; n < 5; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (j - n >= 0 && j - n < length)
                                    {
                                        values[n] = Map[i, j - n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 5:
                                for (int n = 0; n < 5; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (j - n >= 0 && j - n < length && i - n >= 0 && i - n < length)
                                    {
                                        values[n] = Map[i - n, j - n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 6:
                                for (int n = 0; n < 5; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (i - n >= 0 && i - n < length)
                                    {
                                        values[n] = Map[i - n, j];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                            case 7:
                                for (int n = 0; n < 5; n++)
                                {
                                    if (k == 0)
                                    {
                                        break;
                                    }
                                    if (j + n >= 0 && j + n < length && i - n >= 0 && i - n < length)
                                    {
                                        values[n] = Map[i - n, j + n];
                                    }
                                    else
                                    {
                                        k = 0;
                                    }
                                }
                                break;
                        }
                        if (k == 1)
                        {
                            if (values[0] == 1 && values[1] == 1 && values[2] == 1 && values[3] == 1 && values[4] == 1)
                            {
                                return 1;
                            }
                            if (values[0] == 2 && values[1] == 2 && values[2] == 2 && values[3] == 2 && values[4] == 2)
                            {
                                return 2;
                            }
                        }
                        else
                        {
                            k = 1;
                        }
                    }
                }
            }

            if(l > 0)
            {
                return 3;
            }

            return 0;
        }


        public void PrintMap()
        {
            for (int i = 0; i < high; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    Console.Write("{0} ", Map[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }




        //判断周围是否有棋子
        private bool CheckRound(Tuple<int, int> position)
        {
            int x = position.Item1;
            int y = position.Item2;
            if (x + 1 < high)
            {
                if (Map[x+1, y] != 0)
                {
                    return true;
                }
            }
            if (x - 1 >= 0)
            {
                if (Map[x - 1, y] != 0)
                {
                    return true;
                }
            }
            if (y +1 < length)
            {
                if (Map[x, y+1] != 0)
                {
                    return true;
                }
            }
            if (y - 1 >= 0)
            {
                if (Map[x, y - 1] != 0)
                {
                    return true;
                }
            }
            if (x+1 < high && y + 1 < length)
            {
                if (Map[x+1, y + 1] != 0)
                {
                    return true;
                }
            }
            if (x - 1 >=0 && y -1 >=0)
            {
                if (Map[x - 1, y - 1] != 0)
                {
                    return true;
                }
            }
            if (x + 1 < high && y - 1 >= 0)
            {
                if (Map[x + 1, y - 1] != 0)
                {
                    return true;
                }
            }
            if (x - 1 >= 0 && y + 1 < length)
            {
                if (Map[x - 1, y + 1] != 0)
                {
                    return true;
                }
            }

            
            return false;

        }

    }
}
