using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace ECMBase
{

    public record TextBase
    {
        readonly Font font;
        readonly public int size;
        readonly public Color color;

        public TextBase(Font font, int size, Color color)
        {
            this.font = font;
            this.size = size;
            this.color = color;
        }

        public StackableImage Draw(ECMDrawer drawer, string text, int xoffset, int yoffset,int height)
        {
            Image image = TextDrawer.Draw(text, font, color, size);
            return new StackableImage(image, new Rectangle(xoffset,yoffset,image.Width,image.Height), height);
        }
        
    }

    public class ECMText : ECMDrawer.IImageDrawable
    {
        public TextBase value;
        public string text;
        public int xoffset;
        public int yoffset;

        public int height;
        public StackableImage SelfDraw(ECMDrawer drawer) => value.Draw(drawer,text,xoffset,yoffset,height);
    }

    public class ECMImage : ECMDrawer.IImageDrawable
    {
        public Image image;
        public Rectangle rect;
        public int height;

        public void DefaultSize()
        {
            rect.Width = image.Width;
            rect.Height = image.Height;
        }

        public StackableImage SelfDraw(ECMDrawer drawer)
        {
            throw new NotImplementedException();
        }
    }

    public class ECMDivider : ECMDrawer.IImageDrawable
    {
        public int TOPGAP=20;
        public int LEFTGAP=60;
        public int RIGHTGAP = 20;
        public int BOTTOMGAP = 20;

        public int BOXSIZE = 80;
        public int BOXHEIGHTGAP = 15;
        public int BOXWIDTHGAP = 4;

        //아직안만듬
        public int MAXBOXCOUNT = 12;

        //아직안만듬
        public TextBase COVEROVERLAY1;

        //아직안만듬
        public TextBase COVEROVERLAY2;

        public bool SHOWLEFTLEVEL = true;
        //만드는중
        public TextBase LEFTLEVEL = new TextBase(new Font("Verdana",18),100,Color.Black);
        public int LEFTLEVELLEFTGAP = 2;
        public int LEFTLEVELTOPGAP = 40;
        public string LEFTLEVELSUFFIX = "렙";

        public bool SHOWLINE = true;
        public int LINESIZE = 1;
        public Color LINECOLOR = Color.Black;


        public int height = 1;

        public StackableImage SelfDraw(ECMDrawer drawer)
        {
            Bitmap bitmap = drawer.GetEmptyBitmap();
            using Graphics canvas = Graphics.FromImage(bitmap);
            if (SHOWLINE)
            {
                using Pen pineapple = new Pen(LINECOLOR, LINESIZE);
                
                bool turn = true;

                for (int i = TOPGAP; i < drawer.MAX_HEIGHT; i += turn ? BOXSIZE : BOXHEIGHTGAP)
                {
                    canvas.DrawLine(pineapple, LEFTGAP, i, drawer.MAX_WIDTH, i);
                    turn = !turn;
                }
            }

            if(SHOWLEFTLEVEL)
            {
                for (int i = 0; i < drawer.BOX_YCOUNT; i ++)
                {
                    int y = TOPGAP + (i * (BOXSIZE + BOXHEIGHTGAP)) + LEFTLEVELTOPGAP;

                    var sm = LEFTLEVEL.Draw(drawer, drawer.LEFTLEVELS[i].ToString()+LEFTLEVELSUFFIX, LEFTLEVELLEFTGAP, y, 2);

                    canvas.DrawImage(sm.image, new Point(sm.rect.X, sm.rect.Y));
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
        public int RIGHTGAP;
        public int BOTTOMGAP;

        public int BOXSIZE;
        public int BOXHEIGHTGAP;
        public int BOXWIDTHGAP;

        public int BOX_YCOUNT;
        public int BOX_XCOUNT;

        public int MAX_HEIGHT;
        public int MAX_WIDTH;

        public double[] LEFTLEVELS;


        public int[] YCOVERAXIS;
        public int[] YLINEAXIS;
        public bool[,] TETRIS;
        


        public interface IImageDrawable
        {
            public StackableImage SelfDraw(ECMDrawer drawer);
        }


        public Image Draw(ECMProject project)
        {
            TOPGAP = project.option.divider.TOPGAP;
            LEFTGAP = project.option.divider.LEFTGAP;
            RIGHTGAP = project.option.divider.RIGHTGAP;
            BOTTOMGAP = project.option.divider.BOTTOMGAP;

            BOXSIZE = project.option.divider.BOXSIZE;
            BOXHEIGHTGAP = project.option.divider.BOXHEIGHTGAP;
            BOXWIDTHGAP = project.option.divider.BOXWIDTHGAP; ;

            BOX_YCOUNT = project.LevelList.Count;
            BOX_XCOUNT = project.LevelList.MaxBy((val)=>val.Value.Count).Value.Count;

            MAX_HEIGHT = TOPGAP + (BOX_YCOUNT * (BOXSIZE + BOXHEIGHTGAP)) + BOTTOMGAP;
            MAX_WIDTH = LEFTGAP + (BOX_XCOUNT* (BOXSIZE + BOXWIDTHGAP)) + RIGHTGAP;

            LEFTLEVELS = project.LevelList.Keys.ToArray();



            
            Image output = GetEmptyBitmap();
            List<StackableImage> drawStack = new List<StackableImage>();


            //곡 디스크이미지 스택
            for (int i=0;i< project.LevelList.Values.Count; i++)
            {
                var vv = project.LevelList.Values.ElementAt(i);
                for (int j = 0; j < vv.Count; j++)
                {
                    int x2 = LEFTGAP + (j * (BOXSIZE + BOXWIDTHGAP));
                    int y2 = TOPGAP + BOXHEIGHTGAP + (i * (BOXSIZE + BOXHEIGHTGAP));
                    ECMLevel? v = vv[j];
                    var w = v.SelfDraw(this);
                    w.rect = new Rectangle(x2,y2,BOXSIZE,BOXSIZE);
                    drawStack.Add(w);
                }
            }


            //divider 스택
            drawStack.Add(project.option.divider.SelfDraw(this));





            //drawStack 전부 쌓고나서 그리기 시작
            drawStack.Sort((left, right) => left.height < right.height ? -1 : 1);
            drawStack.Reverse();
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
            return new StackableImage(this.image, new Rectangle(0,0,drawer.BOXSIZE,drawer.BOXSIZE),0);
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