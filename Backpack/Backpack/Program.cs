using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backpack
{
    class Program
    {
        private static Stopwatch stopWatch = new Stopwatch();
        private static Backpack backpack;
        private static double sumCost;
        
        private static FullEnumIter FullEnumIter = new FullEnumIter(); // Алгоритм полного итеративного перебора
        private static FullEnumRec FullEnumRec = new FullEnumRec(); // Алгоритм полного рекурсивного перебора
        private static GreedyAlgorithm GreedyAlgorithm = new GreedyAlgorithm(); // Жадный алгоритм

        static void Main(string[] args)
        {
            TestFromFile("input_1.txt", FullEnumIter);
            TestFromFile("input_1.txt", FullEnumRec);
            TestFromFile("input_1.txt", GreedyAlgorithm);
            //AutoTestGreedy();
            //AutoTestBadGreedy();
            //AutoTest(FullEnumRec);
            //AutoTest(FullEnumIter);
            Console.ReadKey();
        }

        private static void TestFromFile(string fileName, IBackpackAlgorithm algorithm)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                var values = reader.ReadLine().Split(' ');

                int countItems = int.Parse(values[0]);
                double maxMass = int.Parse(values[1]);

                backpack = new Backpack(maxMass);

                var listOfMass = reader.ReadLine().Split(' ');
                var listOfCost = reader.ReadLine().Split(' ');
                for (int i = 0; i < countItems; i++)
                {
                    backpack.AddItem(Int32.Parse(listOfCost[i]), Int32.Parse(listOfMass[i]));
                }
            }

            using (StreamWriter writer = new StreamWriter("output.txt"))
            {
                var answer = algorithm.Run(backpack, ref sumCost);

                writer.WriteLine(sumCost);
                writer.WriteLine(string.Join(" ", answer));
                Console.WriteLine(sumCost);
                Console.WriteLine(string.Join(" ", answer));
            }
        }

        private static void AutoTest(IBackpackAlgorithm algorithm)
        {
            int maxCountItems = 25;
            using (StreamWriter writer = new StreamWriter($"{algorithm.ToString()}.txt"))
            {
                for (int countItems = 1; countItems <= maxCountItems; countItems++)
                {
                    Test.GenTest(countItems, ref backpack);
                    
                    int countIter = 1;
                    if (countItems < 20) countIter = 2;
                    if (countItems < 15) countIter = 20;
                    if (countItems < 10) countIter = 2000;

                    long ticks = 0;

                    for (int k = 0; k < countIter; k++)
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        algorithm.Run(backpack, ref sumCost);
                        stopWatch.Stop();
                        ticks += stopWatch.Elapsed.Ticks;
                    }

                    double milliseconds = TimeSpan.FromTicks(ticks / countIter).TotalMilliseconds;

                    Console.WriteLine($"{algorithm} N = {countItems} time = {milliseconds} milliseconds");
                    writer.WriteLine($"{countItems}\t{milliseconds}");
                }
            }
        }

        private static void AutoTestGreedy()
        {
            int maxCountItems = 25;
            int lengthTestSeries = 20;
            int countIterForTest = 10000;
            using (StreamWriter writer = new StreamWriter($"{GreedyAlgorithm}.txt"))
            {
                for (int countItems = 1; countItems <= maxCountItems; countItems++)
                {
                    double time = 0;
                    int countTrueAnswers = 0;
                    for (int numTest = 0; numTest < lengthTestSeries; numTest++)
                    {
                        Test.GenTest(countItems, ref backpack);

                        #region TrueAnswer
                        List<int> trueAnswer = FullEnumRec.Run(backpack, ref sumCost);
                        List<Item> takenItems = backpack.Items.Where(x => trueAnswer.Contains(x.Num)).ToList();
                        int trueAnswerMass = (int)takenItems.Sum(x => x.Mass);
                        int trueAnswerCost = (int)takenItems.Sum(x => x.Cost);
                        #endregion

                        long timeOfAllIterations = 0;
                        List<int> answer = new List<int>();
                        for (int iter = 0; iter < countIterForTest; iter++)
                        {
                            stopWatch.Reset();
                            stopWatch.Start();
                            answer = GreedyAlgorithm.Run(backpack, ref sumCost);
                            stopWatch.Stop();
                            timeOfAllIterations += stopWatch.Elapsed.Ticks;
                        }

                        takenItems = backpack.Items.Where(x => answer.Contains(x.Num)).ToList();
                        int answerMass = (int)takenItems.Sum(x => x.Mass);
                        int answerCost = (int)takenItems.Sum(x => x.Cost);

                        if (answerCost == trueAnswerCost && answerMass <= backpack.MaxMass)
                        {
                            countTrueAnswers++;
                        }

                        time += TimeSpan.FromTicks(timeOfAllIterations / countIterForTest).TotalMilliseconds / lengthTestSeries;
                    }
                    
                    Console.WriteLine($"GreedyAlgorithm: N = {countItems}; time = {time} milliseconds; True = {(double)countTrueAnswers / lengthTestSeries * 100}%");
                    writer.WriteLine($"{countItems}\t{time}\t{(double)countTrueAnswers / lengthTestSeries}");
                }
            }
        }

        private static void AutoTestBadGreedy()
        {
            int maxCountItems = 1000;
            int lengthTestSeries = 20;
            int countIterForTest = 1000;
            using (StreamWriter writer = new StreamWriter($"{GreedyAlgorithm}.txt"))
            {
                for (int countItems = 1; countItems <= maxCountItems; countItems++)
                {
                    double time = 0;
                    int countTrueAnswers = 0;
                    for (int numTest = 0; numTest < lengthTestSeries; numTest++)
                    {
                        Test.GenBadTestForGreedy(countItems, ref backpack);

                        #region TrueAnswer
                        List<int> trueAnswer = FullEnumRec.Run(backpack, ref sumCost);
                        List<Item> takenItems = backpack.Items.Where(x => trueAnswer.Contains(x.Num)).ToList();
                        int trueAnswerMass = (int)takenItems.Sum(x => x.Mass);
                        int trueAnswerCost = (int)takenItems.Sum(x => x.Cost);
                        #endregion

                        long timeOfAllIterations = 0;
                        List<int> answer = new List<int>();
                        for (int iter = 0; iter < countIterForTest; iter++)
                        {
                            stopWatch.Reset();
                            stopWatch.Start();
                            answer = GreedyAlgorithm.Run(backpack, ref sumCost);
                            stopWatch.Stop();
                            timeOfAllIterations += stopWatch.Elapsed.Ticks;
                        }

                        takenItems = backpack.Items.Where(x => answer.Contains(x.Num)).ToList();
                        int answerMass = (int)takenItems.Sum(x => x.Mass);
                        int answerCost = (int)takenItems.Sum(x => x.Cost);

                        if (answerCost == trueAnswerCost && answerMass <= backpack.MaxMass)
                        {
                            countTrueAnswers++;
                        }

                        time += TimeSpan.FromTicks(timeOfAllIterations / countIterForTest).TotalMilliseconds / lengthTestSeries;
                    }

                    Console.WriteLine($"GreedyAlgorithm: N = {countItems}; time = {time} milliseconds; True = {(double)countTrueAnswers / lengthTestSeries * 100}%");
                    writer.WriteLine($"{countItems}\t{time}\t{(double)countTrueAnswers / lengthTestSeries}");
                }
            }
        }
    }
}
