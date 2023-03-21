using System.Drawing;

namespace Gobang_AI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("StarWing 2023.3.16");
            /*DateTime dateTime = DateTime.Now;
            Console.WriteLine(dateTime.Second);*/
            Console.WriteLine();       
            GameTest();
        }


        static void GameTest()
        {
            GameTree.GameTree game = new GameTree.GameTree();        
            game.PrintMap();
            Console.WriteLine("Choose First or Second(0:First, 1:Second): ");
            bool isAI = Convert.ToInt32(Console.ReadLine()) == 1;
            Tuple<int, int> nextStep = new Tuple<int, int>(0, 0);
            int k = 0;
            while (game.GameOver() == 0)
            {
               
                if (isAI && k > 0)
                {
                    TimeSpan ts1 = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    game.AlphaBetaSearch(isAI);
                    //game.MinMaxSearch(isAI);
                    TimeSpan ts2 = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    nextStep = game.GetNextStep();
                    Console.WriteLine("AI: ({0}, {1})", nextStep.Item1 + 1, nextStep.Item2 + 1);
                    Console.WriteLine(ts2 - ts1);
                }else if (isAI && k == 0)
                {
                    TimeSpan ts1 = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    nextStep = new Tuple<int, int>(5, 5);
                    //game.MinMaxSearch(isAI);
                    TimeSpan ts2 = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    Console.WriteLine("AI: ({0}, {1})", nextStep.Item1 + 1, nextStep.Item2 + 1);
                    Console.WriteLine(ts2 - ts1);
                }
                else
                {

                    Console.WriteLine("Input: ");
                    Console.Write("x: ");
                    int x = Convert.ToInt32(Console.ReadLine());
                    Console.Write("y: ");
                    int y = Convert.ToInt32(Console.ReadLine());
                    nextStep = new Tuple<int, int>(x - 1, y - 1);
                }
                game.MakeNextMove(nextStep, isAI);
                game.PrintMap();
                isAI = !isAI;
                k++;
            }
            switch (game.GameOver())
            {
                case 1:
                    Console.WriteLine("AI Win ");
                    break;
                case 2:
                    Console.WriteLine("Player Win ");
                    break;
                case 3:
                    Console.WriteLine("Draw ");
                    break;

            }
            

        }

    }
}