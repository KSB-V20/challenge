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
    //СЮДА КЛАССЫ
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
                //STATISTICS

                if (tipe == "statistics")
                    if (str.IndexOf("min") == 0) answer = Statistics.Min(str);
                    if (str.IndexOf("max") == 0) answer = Statistics.Max(str);
                    if (str.IndexOf("sum") == 0) answer = Statistics.Sum(str);
                    if (str.IndexOf("median") == 0) answer = Statistics.Median(str);
                    if (str.IndexOf("firstmostfrequent") == 0) answer = Statistics.FirstMostFrequent(str);


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
