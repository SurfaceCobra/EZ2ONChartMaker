using System.Drawing;

namespace ECMBase
{


    public class ECMDivider : ECMDrawer.IImageDrawable
    {
        public int topgap=20;
        public int leftgap=20;
        public int boxheight=80;
        public int boxgap=15;

        public int height { get; set; }

        public StackableImage SelfDraw(ECMDrawer drawer)
        {
            using Pen pineapple = new Pen(Color.Black, 1);
            Bitmap bitmap = drawer.GetEmptyBitmap();
            using(Graphics canvas = Graphics.FromImage(bitmap))
            {
                bool turn = true;

                for (int i = topgap; i < drawer.MAX_HEIGHT; i += turn ? boxheight : boxgap)
                {
                    canvas.DrawLine(pineapple, leftgap, i, drawer.MAX_WIDTH, i);
                    turn = !turn;
                }
            }
            return new StackableImage(bitmap,new Rectangle(0,0,drawer.MAX_WIDTH,drawer.MAX_HEIGHT), 0);
        }
    }

    public class ECMOption
    {
        [ObjectManager.IgnoreSave]
        public List<StackableImage> imagelist;

        public ECMDivider divider;

        public int height { get; set; }

        public ECMOption()
        {
            imagelist = new List<StackableImage>(8);
            divider = new ECMDivider();
        }
    }






    public class ECMDrawer
    {
        public int TOPGAP;
        public int LEFTGAP;
        public int BOXHEIGHT;
        public int BOXGAP;

        public int BOX_YCOUNT;
        public int BOX_XCOUNT;

        public int MAX_HEIGHT;
        public int MAX_WIDTH;


        public interface IImageDrawable
        {
            public StackableImage SelfDraw(ECMDrawer drawer);
        }


        public Image Draw(ECMProject project)
        {
            TOPGAP = project.option.divider.topgap;
            LEFTGAP = project.option.divider.leftgap;
            BOXHEIGHT = project.option.divider.boxheight;
            BOXGAP = project.option.divider.boxgap;

            BOX_YCOUNT = project.LevelList.Count;
            BOX_XCOUNT = project.LevelList.MaxBy((val)=>val.Value.Count).Value.Count;

            MAX_HEIGHT = TOPGAP + (BOX_YCOUNT * (BOXHEIGHT + BOXGAP));
            MAX_WIDTH = LEFTGAP + (BOX_XCOUNT*BOXHEIGHT);





            Image output = GetEmptyBitmap();
            List<StackableImage> drawStack = new List<StackableImage>();
            for(int i=0;i< project.LevelList.Values.Count; i++)
            {
                var vv = project.LevelList.Values.ElementAt(i);
                for (int j = 0; j < vv.Count; j++)
                {
                    int x2 = LEFTGAP + (j * BOXHEIGHT);
                    int y2 = TOPGAP + BOXGAP + (i * (BOXHEIGHT + BOXGAP));
                    ECMLevel? v = vv[j];
                    var w = v.SelfDraw(this);
                    w.rect = new Rectangle(x2,y2,BOXHEIGHT,BOXHEIGHT);
                    drawStack.Add(w);
                }
            }






            drawStack.Sort((left, right) => left.height < right.height ? -1 : 1);
            using (Graphics canvas = Graphics.FromImage(output))
            {
                foreach (var tuple in drawStack)
                {
                    canvas.DrawImage(tuple.image, tuple.rect);
                }
            }


            



            return output;
        }
        public Bitmap GetEmptyBitmap()
        {
            Bitmap bitmap = new Bitmap(MAX_WIDTH, MAX_HEIGHT, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            /*
            for (int i = 0; i < MAX_HEIGHT; i++)
            {
                for (int j = 0; j < MAX_WIDTH; j++)
                {
                    bitmap.SetPixel(i, j, Color.FromArgb(0, 0, 0, 0));
                }
            }
            */
            return bitmap;
        }
    }


    public partial class ECMLevel : ECMDrawer.IImageDrawable
    {
        public StackableImage SelfDraw(ECMDrawer drawer)
        {
            return new StackableImage(this.image, new Rectangle(0,0,drawer.BOXHEIGHT,drawer.BOXHEIGHT),0);
        }
    }


    public record struct StackableImage(Image image, Rectangle rect, int height)
    {
        public static implicit operator (Image image, Rectangle rect, int height)(StackableImage value)
        {
            return (value.image, value.rect, value.height);
        }

        public static implicit operator StackableImage((Image image, Rectangle rect, int height) value)
        {
            return new StackableImage(value.image, value.rect, value.height);
        }
    }
}