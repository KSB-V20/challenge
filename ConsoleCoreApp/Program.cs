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
                
                //DETERMINANT

                if (tipe == "determinant")
                {
                    double SDet(double a, double b, double c, double d)
                    {
                        return a * d - b * c;
                    }
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
                    answer = result.ToString();
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
