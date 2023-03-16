namespace Gobang_AI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("StarWing 2023.3.16");
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
            while (game.GameOver() == 0)
            {
                if (isAI)
                {
                    int k = game.MinMax(isAI);
                    nextStep = game.GetNextStep();
                    Console.WriteLine("AI: ({0}, {1})", nextStep.Item1 + 1, nextStep.Item2 + 1);
                    //Console.WriteLine(k);
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