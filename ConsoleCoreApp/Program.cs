using Challenge;
using Challenge.DataContracts;
using System;
using Task = System.Threading.Tasks.Task;

namespace ConsoleCoreApp
{
    // Это рекомендуемый вариант приложения.
    // Данное приложение можно запускать под Windows, Linux, Mac.
    // Для запуска приложения необходимо скачать и установить подходящую версию .NET Core.
    // Скачать можно тут: https://dotnet.microsoft.com/download/dotnet-core
    // Какая версия .NET Core нужна можно посмотреть в свойствах проекта.
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

            //ИЗМЕНИТЬ ТИП ЗАДАНИЯ
            const string taskType = "";

            var utcNow = DateTime.UtcNow;
            string currentRound = null;
            foreach (var round in challenge.Rounds)
            {
                if (round.StartTimestamp < utcNow && utcNow < round.EndTimestamp)
                    currentRound = round.Id;
            }

            //ИЗМЕНИТЬ КОЛ-ВО ИТЕРАЦИЙ
            for (var g = 0; g < 1; g++)
            {
                Console.WriteLine(
                    $"Нажми ВВОД, чтобы получить первые 50 взятых командой задач типа {taskType} в раунде {currentRound}");
                Console.ReadLine();
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
                Console.ReadLine();
                Console.WriteLine("Ожидание...");
                var newTask = await challengeClient.AskNewTaskAsync(currentRound, taskType);
                Console.WriteLine($"  Новое задание, статус {newTask.Status}");
                Console.WriteLine($"  Формулировка: {newTask.UserHint}");
                Console.WriteLine($"                {newTask.Question}");
                Console.WriteLine();
                Console.WriteLine("----------------");
                Console.WriteLine();

                //const string answer = "42";
                var str = newTask.Question;
                var tipe = newTask.TypeId;
                var answer = "";

                //СЮДА КОД С ЗАДАНИЯМИ
                
                if (tipe == "math")
                {
                    try
                    {
                        var result = new System.Data.DataTable().Compute(str, "");
                        answer = result.ToString();
                    }
                    catch
                    { }
                }

                //CYPHER

                if (tipe == "cypher")
                {
                    if (str.IndexOf("reversed") == 0)
                    {
                        var stroke = str.Split('#');
                        var operation = stroke[0];
                        var text = stroke[1];
                        var output = new StringBuilder();
                        if (operation == "reversed")
                            for (var i = 0; i < text.Length; i++)
                                output.Append(text[text.Length - 1 - i]);
                        answer = output.ToString();
                    }
                    // То что ниже не работает!!!
                    if (str.IndexOf("Caesar's code") == 0)
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
                        answer = result;
                    }
                }

                Console.WriteLine(
                    $"Нажми ВВОД, чтобы ответить на полученную задачу самым правильным ответом: {answer}");
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
                    Console.WriteLine($"Похоже ответ не подошел и задачу больше сдать нельзя...");
                Console.WriteLine();
                Console.WriteLine("----------------");
                Console.WriteLine();

                Console.WriteLine($"Нажми ВВОД, чтобы завершить работу программы");
                Console.ReadLine();
            }
        }
    }
}
