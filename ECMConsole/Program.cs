

using ECMBase;
using System.Drawing;

namespace ECMConsole
{
    class Tester
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Console.WriteLine(Math.Ceiling((double)1/8));
            //loadpreproject();
            //SaveImage();
            MapTest();
        }


        static void MapTest()
        {
            SuperMap<int?> smap = new SuperMap<int?>();
            smap.SetAt(new Point(1,3), 112233);
            smap.SetAt(new Point(-2, -7), 15543);

            //var v = smap.IndexOf(112233);

            int? val = smap.GetAt(new Point(1,3));

            Console.WriteLine();
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

        static void loadpreproject()
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