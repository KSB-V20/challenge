namespace ConsoleCoreApp
{
    class Solver
    {
        public string Solve(string input)
        {
            var parts = input.Split("+");
            var a = input.Parse(parts[0]);
            var b = input.Parse(parts[1]);
            return (a + b).ToString();
        }
    }
}