

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
            MapTest2();
        }


        static void RectTest()
        {
            Rectangle rect = new Rectangle(0,0,4,5);
            Point p = new Point(9,15);

            var v = DT.Distance(rect, p);

            Console.WriteLine(v);
        }

        static void MapTest1()
        {
            GridMapBuilder<int?> mapbuilder = new GridMapBuilder<int?>();

            mapbuilder.SetAt(new Point(1,3), 6974);

            Log.Message("////");

            mapbuilder.SetAt(new Point(-2, -7), 3203);

            GridMap<int?> map = mapbuilder.ToMap();
            int? val = mapbuilder.GetAt(new Point(-2, -7));
            int? val2 =  map.GetAt(new Point(1, 3));

            Console.WriteLine(val??-1);
            Console.WriteLine(val2??-1);

        }

        static void MapTest2()
        {
            PuzzleMapBuilder<int?> puzzlebuilder = new PuzzleMapBuilder<int?>();
            for (int i = 0; i < 5; i++)
            {
                GridMapBuilder<int?> builder = new GridMapBuilder<int?>();

                DT.Filler(new Rectangle(0, 0, 3, 4)).ForEach((v) => builder.SetAt(v, i));

                BlockMap<int?> blockmap = builder.ToBlockMap(new Size(i, i));

                puzzlebuilder.StackMap(blockmap, new Size(i,i));
            }

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