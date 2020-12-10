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
                
                if (tipe == "statistics-composition") answer = BotStatisticsComposition.StatisticsComposition(str);
                /////////////////////////

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
