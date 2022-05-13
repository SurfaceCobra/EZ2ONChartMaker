using System.Drawing;

using static ECMBase.Constants;

namespace ECMBase
{
    static class Constants
    {
        public const int MAX_HEIGHT = 1600;
        public const int MAX_WIDTH = 800;
    }

    public interface IImageDrawer
    {
        public void Draw(Graphics canvas);

        public int height { get; }


    }

    public class ECMText : IImageDrawer
    {
        public void Draw(Graphics canvas)
        {
            throw new NotImplementedException();
        }

        public int height { get; set; }
    }

    public class ECMImage : IImageDrawer
    {
        public Image image;
        public int x=100;
        public int y= MAX_HEIGHT-100;
        public int height { get; set; }

        public void Draw(Graphics canvas)
        {
            canvas.DrawImage(image,new Point(x,y));
        }

        public ECMImage(Image image)
        {
            this.image = image;
        }
    }

    public class ECMDivider : IImageDrawer
    {
        public int topgap=100;
        public int leftgap=100;
        public int boxheight=60;
        public int boxgap=10;

        public int height { get; set; }

        public void Draw(Graphics canvas)
        {
            using Pen pineapple = new Pen(Color.Black, 1);
            bool turn = true;

            for (int i = topgap; i < MAX_HEIGHT; i += turn ? boxheight : boxgap)
            {
                canvas.DrawLine(pineapple, leftgap, i, MAX_WIDTH, i);
                turn = !turn;
            }

            pineapple.Dispose();
        }
    }

    public class ECMCoverImage : IImageDrawer
    {
        [ObjectManager.IgnoreSave]
        public ECMProject parentProject;

        public void Draw(Graphics canvas)
        {
            var divider = parentProject.option.divider;


            List<double> levelList = parentProject.LEVELList.Select((val)=>val.level).Distinct().ToList();
            levelList.Sort();
            levelList.Reverse();
            Dictionary<double, List<Image>> dudu = new Dictionary<double, List<Image>>(levelList.Count);

            foreach(double level in levelList)
            {
                dudu.Add(
                    level,
                    parentProject.LEVELList
                        .FindAll((val)=>val.level==level)
                        .Select((val)=>parentProject.Images[val.name])
                        .ToList()
                    );
            }
            //여기 안예쁨



            for(int i=0;i < dudu.Count; i++)
            {
                var pair = dudu.ElementAt(i);
                for (int j = 0; j < pair.Value.Count; j++)
                {
                    Image image = pair.Value[j];
                    canvas.DrawImage(image, new Rectangle(GetX(j), GetY(i),divider.boxheight, divider.boxheight));
                }
            }



            
            int GetX(int val) => divider.leftgap +( val * divider.boxheight);
            int GetY(int val) => divider.topgap + divider.boxgap + (val * (divider.boxheight + divider.boxgap));

        }
        public int height => 0;
    }

    public class ECMOption
    {
        [ObjectManager.IgnoreSave]
        public List<ECMImage> imagelist;

        public ECMDivider divider;

        public ECMCoverImage coverImage;

        public int height { get; set; }

        public ECMOption()
        {
            imagelist = new List<ECMImage>(8);
            divider = new ECMDivider();
            coverImage = new ECMCoverImage();
        }

        public Image GetImage()
        {
            List<IImageDrawer> exports = new List<IImageDrawer>(imagelist.Count+1);
            exports.AddRange(imagelist);
            exports.Add(divider);
            exports.Add(coverImage);
            exports.Sort((left, right) => left.height < right.height ? -1 : 1 );


            Bitmap bitmap = GetEmptyBitmap();
            using Graphics canvas = Graphics.FromImage(bitmap);
            exports.ForEach((export)=>export.Draw(canvas));

            return bitmap;
        }


        private Bitmap GetEmptyBitmap()
        {
            Bitmap bitmap = new Bitmap(MAX_HEIGHT, MAX_HEIGHT, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int i = 0; i < MAX_HEIGHT; i++)
            {
                for (int j = 0; j < MAX_HEIGHT; j++)
                {
                    bitmap.SetPixel(i, j, Color.FromArgb(0, 0, 0, 0));
                }
            }
            return bitmap;
        }
    }



}