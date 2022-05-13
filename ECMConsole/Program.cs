


namespace ECMConsole
{
    class Tester
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            loadproject();
        }

        static void loadproject()
        {
            string path = @"C:\Users\whitelava3203\source\repos\EZ2ONChartMaker\ECM\project\8k슈랜클리어";

            var b = ECMBase.ECMDataLoader.LoadProject(path);

            Console.WriteLine();

            Console.WriteLine();
        }


        static void loadscript()
        {
            string path = @"C:\Users\whitelava3203\source\repos\EZ2ONChartMaker\ECM\project\8k슈랜클리어\data\data.txt";

            var b = ECMBase.ECMDataLoader.LoadScript(path);

            Console.WriteLine();

            Console.WriteLine();
        }
    }
}