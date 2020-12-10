using Challenge;
using Challenge.DataContracts;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace ConsoleCoreApp
{
    [TestFixture]
    public class JsonTests 
    {
        [TestCase("{\"tenable\":\"-65\",\"fghf\":\"45\",\"houds\":\"4\",\"agdgdy\":\"37\",\"mso\":\"614\"}", 631)]
        [TestCase("{\"ghfcgfe\":\"505\",\"fgchhfgjf\":\"-86765\",\"hfhhds\":\"4\",\"aggjhgy\":\"37\"}", -86219)]
        [TestCase("{\"te\":\"0\"}", 0)]
        [TestCase("{\"adsf\":\"-6\",\"afasfs\":\"6\"}", 0)]
        [TestCase("{\"dgde\":\"-34\",\"ssddgf\":\"345\",\"sdgdgs\":\"0\",\"dgdfgy\":\"7\",\"dgdfgdg\":\"4\"}", 322)]
        [TestCase("{\"a\":\"-1\",\"b\":\"2\",\"c\":\"-3\"}", -2)]
        [TestCase("{\"sfsf\":\"1000000000000\",\"fdvdshf\":\"-1\"}", 999999999999)]

        public void TestCases(string input, int expected)
        {
            Assert.AreEqual(expected, BotJson.Json(input));
        }
    }
    
    [TestFixture]
    public class DeterminantTests 
    {
        [TestCase(@"5 & 9 \\ 5 & 5", -20)]  
        [TestCase(@"7 & -1 \\ 5 & 8", 61)] 
        [TestCase(@"0 & 0 \\ 0 & 0", 0)]
        [TestCase(@"0 & 65 \\ 98 & 0", -6370)]
        [TestCase(@"1 & 1 \\ 1 & 1", 0)]
        [TestCase(@"85 & 1 \\ 1 & 97", 8244)]
        [TestCase(@"84 & 76 \\ 75 & 65", -240)]

        public void TestCases(string input, int expected)
        {
            Assert.AreEqual(expected, BotDeterminant.Determinant(input));
        }
    }
    
    [TestFixture]
    public class MathTests 
    {
        [TestCase("2", 2)]  
        [TestCase("2 + 2", 4)] 
        [TestCase("2 / 5", 0.4)]
        [TestCase("2 * (5 + 3)", 16)]
        [TestCase("15 + 8 - (35 * 68) - (133253 * 65) / 65", -135610)]
        [TestCase("65 % 7", 2)]
        public void TestCases(string input, object expected)
        {
            Assert.AreEqual(expected, BotMath.Math(input));
        }
    }

    //СЮДА КЛАССЫ

    class BotMath
    {
        public static string Math(string str)
        {
                var result = new System.Data.DataTable().Compute(str, "");
                return result.ToString();
        }
    }
    
    class Cypher
    {
        public static string Reverse(string str)
        {
            var stroke = str.Split('#');
            var operation = stroke[0];
            var text = stroke[1];
            var output = new StringBuilder();
            if (operation == "reversed")
                for (var i = 0; i < text.Length; i++)
                    output.Append(text[text.Length - 1 - i]);
            return output.ToString();
        }

        public static string Caesars(string str)
        {
            var t = str.IndexOf('#');
            var a = new List<char>();
            var i = t - 1;
            var sum = 0;
            var value = false;
            while (Char.IsNumber(str[i]))
            {
                a.Add(str[i]);
                i -= 1;
            }
            if (str[i] == '+') value = true;
            var n = a.Count;
            if (n > 1)
            {
                a.Reverse();
                for (var j = 0; j < a.Count; j++)
                {
                    n -= 1;
                    sum += (int)(a[j] * Math.Pow(10, n));
                }
            }
            else sum = (int)a[0];
            if (value) sum = -sum;
            var result = "";
            var alfavit = "\' abcdefghijklmnopqrstuvwxyz0123456789";
            for (var k = t + 1; k < str.Length; k++)
                result += alfavit[(alfavit.IndexOf(str[k]) + sum) % 38];
            return result;
        }
    }
    
    class BotDeterminant
    {
        public static double SDet(double a, double b, double c, double d)
        {
            return a * d - b * c;
        }

        public static string Determinant(string str)
        { 
            var lines = str.Split(@"\\");
            var rank = lines.Length;
            var matrix = new double[rank, rank];
            double result;
            for (var i = 0; i < rank; i++)
            {
                var line = lines[i].Split('&');
                for (var j = 0; j < rank; j++)
                {
                    matrix[i, j] = double.Parse(line[j]);
                }
            }

            if (rank == 3)
                result = matrix[0, 0] * SDet(matrix[1, 1], matrix[1, 2], matrix[2, 1], matrix[2, 2]) -
                            matrix[0, 1] * SDet(matrix[1, 0], matrix[1, 2], matrix[2, 0], matrix[2, 2]) +
                            matrix[0, 2] * SDet(matrix[1, 0], matrix[1, 1], matrix[2, 0], matrix[2, 1]);
            else if (rank == 2)
                result = SDet(matrix[0, 0], matrix[0, 1], matrix[1, 0], matrix[1, 1]);
            else
                result = matrix[0, 0];
            return result.ToString();
        }
    }
    
    class Statistics
    {
        public static string Min(string str)
        {
            var array = str.Substring(str.IndexOf('|') + 1);
            var result = array.Split(' ');
            var min = 1000000000000;
            for (var h = 0; h < result.Length; h++)
                if (int.Parse(result[h]) < min) min = int.Parse(result[h]);
            return min.ToString();
        }

        public static string Max(string str)
        {
            var array = str.Substring(str.IndexOf('|') + 1);
            var result = array.Split(' ');
            var max = -1000000000000;
            for (var h = 0; h < result.Length; h++)
                if (int.Parse(result[h]) > max) max = int.Parse(result[h]);
            return max.ToString();
        }

        public static string Sum(string str)
        {
            var array = str.Substring(str.IndexOf('|') + 1);
            var result = array.Split(' ');
            var sum = 0;
            for (var h = 0; h < result.Length; h++)
                sum += int.Parse(result[h]);
            return sum.ToString();
        }

        public static string Median(string str)
        {
            var array = str.Substring(str.IndexOf('|') + 1);
            var result = array.Split(' ');
            var mas = new double[result.Length];
            for (var i = 0; i < mas.Length; i++)
                mas[i] = int.Parse(result[i]);
            Array.Sort(mas);
            if (mas.Length % 2 == 0) 
                return (((int)(mas[mas.Length / 2]) + mas[(int)(mas.Length / 2) - 1]) / 2).ToString();
            else 
                return (mas[mas.Length / 2]).ToString();
        }

        public static string FirstMostFrequent (string str)
        {
            var array = str.Substring(str.IndexOf('|') + 1);
            var result = array.Split(' ');
            var dict = new Dictionary<string, int>();
            for (var i = 0; i < result.Length; i++)
            {
                if (!dict.ContainsKey(result[i]))
                    dict.Add(result[i], 1);
                else
                    dict[result[i]]++;
            }
            var max = -1;
            var value = "";
            foreach (var i in dict)
            {
                if (i.Value > max)
                {
                    max = i.Value;
                    value = i.Key;
                }
            }
            return value;
        }
    }
    
    class BotJson
    {
        public static string Json(string str)
        {
            var sum = 0;
            var currentNumber = new StringBuilder();
            foreach (var e in str)
            {
                if (e == '-' || e >= '0' && e <= '9')
                    currentNumber.Append(e);
                else if (currentNumber.Length != 0)
                {
                    sum += int.Parse(currentNumber.ToString());
                    currentNumber.Clear();
                }
            }
            return sum.ToString();
        }
    }
    
    class BotStatisticsComposition
    {
        public static string StatisticsComposition(string str)
        {
            var array = str.Substring(str.IndexOf('|') + 1).Split(' ');
            var operations = str.Substring(0, str.IndexOf('|')).Split('.').Reverse().ToArray();
            var mas = new List<int>();
            var answer = "";
            for (var i = 0; i < array.Length; i++)
            {
                var result = new System.Data.DataTable().Compute(array[i], "").ToString();
                mas.Add(int.Parse(result));
            }
            foreach (var i in operations)
            {
                if (i == "double")
                    for (var j = 0; j < mas.Count(); j++)
                        mas[j] *= 2;
                if (i == "decrement")
                    for (var j = 0; j < mas.Count(); j++)
                        mas[j]--;
                if (i == "increment")
                    for (var j = 0; j < mas.Count(); j++)
                        mas[j]++;
                if (i == "odd")
                {
                    var clear = mas;
                    mas.Clear();
                    foreach (var j in clear)
                        if (j % 2 != 0)
                            mas.Add(j);
                }
                if (i == "even")
                {
                    var clear = mas;
                    mas.Clear();
                    foreach (var j in clear)
                        if (j % 2 == 0)
                            mas.Add(j);
                }
                if (i.IndexOf("skip") == 0)
                {
                    var n = "";
                    for (var j = 5; j < i.Length - 1; j++)
                        n += i[j];
                    var n_int = int.Parse(n);
                    var clear = new List<int>();
                    for (var k = 0; k < mas.Count(); k++)
                        clear.Add(mas[k]);
                    mas.Clear();
                    for (var j = n_int; j < clear.Count(); j++)
                        mas.Add(clear[j]);
                }
                if (i.IndexOf("take") == 0)
                {
                    var n = "";
                    for (var j = 5; j < i.Length - 1; j++)
                        n += i[j];
                    var n_int = int.Parse(n);
                    var clear = new List<int>();
                    for (var k = 0; k < mas.Count(); k++)
                        clear.Add(mas[k]);
                    mas.Clear();
                    for (var j = 0; j < Math.Min(n_int, clear.Count()); j++)
                        mas.Add(clear[j]);
                }

                if (i == "sum")
                {
                    if (mas.Count() == 0)
                    {
                        answer = "0";
                        break;
                    }
                    var sum = 0;
                    for (var h = 0; h < mas.Count(); h++)
                        sum += mas[h];
                    answer = sum.ToString();
                }
                if (i == "min")
                {
                    if (mas.Count() == 0)
                    {
                        answer = "0";
                        break;
                    }
                    var min = 1000000000000;
                    for (var h = 1; h < mas.Count(); h++)
                        if (mas[h] < min)
                            min = mas[h];
                    answer = min.ToString();
                }
                if (i == "max")
                {
                    if (mas.Count() == 0)
                    {
                        answer = "0";
                        break;
                    }
                    var max = -1000000000000;
                    for (var h = 1; h < mas.Count(); h++)
                        if (mas[h] > max)
                            max = mas[h];
                    answer = max.ToString();
                }
            }
            if (answer == "")
                for (var f = 0; f < mas.Count(); f++)
                    answer += mas[f] + " ";
            return answer;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            const string teamSecret = "upvfPcVHxvgW4d7lyZXrXKqobhzjkoip"; // Вставь сюда ключ команды
            if (string.IsNullOrEmpty(teamSecret))
            {
                Console.WriteLine("Задай секрет своей команды, чтобы можно было делать запросы от ее имени");
                return;
            }
            var challengeClient = new ChallengeClient(teamSecret);

            const string challengeId = "projects-course";
            Console.WriteLine($"Нажми ВВОД, чтобы получить информацию о соревновании {challengeId}");
            Console.ReadLine();
            Console.WriteLine("Ожидание...");
            var challenge = await challengeClient.GetChallengeAsync(challengeId);
            Console.WriteLine(challenge.Description);
            Console.WriteLine();
            Console.WriteLine("----------------");
            Console.WriteLine();

            const string taskType = "";

            var utcNow = DateTime.UtcNow;
            string currentRound = null;
            foreach (var round in challenge.Rounds)
            {
                if (round.StartTimestamp < utcNow && utcNow < round.EndTimestamp)
                    currentRound = round.Id;
            }
            for (var g = 0; g < 40; g++)
            {
                Console.WriteLine($"Нажми ВВОД, чтобы получить первые 50 взятых командой задач типа {taskType} в раунде {currentRound}");
                //Console.ReadLine();
                Console.WriteLine("Ожидание...");
                var firstTasks = await challengeClient.GetTasksAsync(currentRound, taskType, TaskStatus.Pending, 0, 50);
                for (int i = 0; i < firstTasks.Count; i++)
                {
                    var task = firstTasks[i];
                    Console.WriteLine($"  Задание {i + 1}, статус {task.Status}");
                    Console.WriteLine($"  Формулировка: {task.UserHint}");
                    Console.WriteLine($"                {task.Question}");
                    Console.WriteLine();
                }
                Console.WriteLine("----------------");
                Console.WriteLine();

                Console.WriteLine($"Нажми ВВОД, чтобы получить задачу типа {taskType} в раунде {currentRound}");
                //Console.ReadLine();
                Console.WriteLine("Ожидание...");
                var newTask = await challengeClient.AskNewTaskAsync(currentRound, taskType);
                Console.WriteLine($"  Новое задание, статус {newTask.Status}");
                Console.WriteLine($"  Формулировка: {newTask.UserHint}");
                Console.WriteLine($"                {newTask.Question}");
                Console.WriteLine();
                Console.WriteLine("----------------");
                Console.WriteLine();

                var str = newTask.Question;
                var tipe = newTask.TypeId;
                var answer = "";

                /////////////////////////
                //MATH

                if (tipe == "math")
                {
                    try
                    {
                        answer = BotMath.Math(str);
                    }
                    catch
                    { }
                }
                
                //MOMENT

                if (tipe == "moment") answer = "10 декабря " + str.Substring(0, 5);
                
                //CYPHER

                if (tipe == "cypher")
                    if (str.IndexOf("reversed") == 0) answer = Cypher.Reverse(str);
                    //  ЦЕЗАРЬ НЕ РАБОТАЕТ!!!
                    if (str.IndexOf("Caesar's code") == 0) answer = Cypher.Caesars(str);
                
                //DETERMINANT
                
                if (tipe == "determinant") answer = BotDeterminant.Determinant(str);
                
                //STATISTICS
                
                if (tipe == "statistics")
                    if (str.IndexOf("min") == 0) answer = Statistics.Min(str);
                    if (str.IndexOf("max") == 0) answer = Statistics.Max(str);
                    if (str.IndexOf("sum") == 0) answer = Statistics.Sum(str);
                    if (str.IndexOf("median") == 0) answer = Statistics.Median(str);
                    if (str.IndexOf("firstmostfrequent") == 0) answer = Statistics.FirstMostFrequent(str);
                
                //JSON

                if (tipe == "json") answer = BotJson.Json(str);
                
                //STATISTICS-COMPOSITION

                if (tipe == "statistics-composition") answer = BotStatisticsComposition.StatisticsComposition(str);

                Console.WriteLine($"Нажми ВВОД, чтобы ответить на полученную задачу самым правильным ответом: {answer}");
                //Console.ReadLine();
                Console.WriteLine("Ожидание...");
                var updatedTask = await challengeClient.CheckTaskAnswerAsync(newTask.Id, answer);
                Console.WriteLine($"  Новое задание, статус {updatedTask.Status}");
                Console.WriteLine($"  Формулировка:  {updatedTask.UserHint}");
                Console.WriteLine($"                 {updatedTask.Question}");
                Console.WriteLine($"  Ответ команды: {updatedTask.TeamAnswer}");
                Console.WriteLine();
                if (updatedTask.Status == TaskStatus.Success)
                    Console.WriteLine($"Ура! Ответ угадан!");
                else if (updatedTask.Status == TaskStatus.Failed)
                {
                    Console.WriteLine($"Похоже ответ не подошел и задачу больше сдать нельзя...");
                    break
                }
                Console.WriteLine();
                Console.WriteLine("----------------");
                Console.WriteLine();
            }

            Console.WriteLine($"Нажми ВВОД, чтобы завершить работу программы");
            Console.ReadLine();
        }
    }
}
