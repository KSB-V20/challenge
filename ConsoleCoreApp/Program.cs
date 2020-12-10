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
                
                if (tipe == "cypher")
                    if (str.IndexOf("reversed") == 0) answer = Cypher.Reverse(str);
                    //  ЦЕЗАРЬ НЕ РАБОТАЕТ!!!
                    if (str.IndexOf("Caesar's code") == 0) answer = Cypher.Caesars(str);
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
