


using System.Drawing;

namespace ECMConsole
{
    class Tester
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            SaveImage();
        }

        static void SaveImage()
        {
            string path = @"C:\Users\whitelava3203\source\repos\EZ2ONChartMaker\ECM\project\8k슈랜클리어";
            string outputpath = @"C:\Users\whitelava3203\source\repos\EZ2ONChartMaker\ECM\project\8k슈랜클리어\output.png";


            var project = ECMBase.ECMLoader.LoadProject(path);

            ECMBase.ECMDrawer drawer = new ECMBase.ECMDrawer();
            Image image = drawer.Draw(project);
            



            image.Save(outputpath);
        }

        static void loadproject()
        {
            string path = @"C:\Users\whitelava3203\source\repos\EZ2ONChartMaker\ECM\project\8k슈랜클리어";

            var b = ECMBase.PreECMDataLoader.LoadPreProject(path);

            Console.WriteLine();

            Console.WriteLine();
        }


        static void loadscript()
        {
            string path = @"C:\Users\whitelava3203\source\repos\EZ2ONChartMaker\ECM\project\8k슈랜클리어\data\data.txt";

            var b = ECMBase.PreECMDataLoader.LoadPreScript(path);

            Console.WriteLine();

            Console.WriteLine();
        }
    }
}