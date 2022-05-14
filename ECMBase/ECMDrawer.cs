using System.Drawing;

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
        public int MAXBOXCOUNT = 5;

        //아직안만듬
        public TextBase COVEROVERLAY1;

        //아직안만듬
        public TextBase COVEROVERLAY2;

        public bool SHOWLEFTLEVEL = true;
        //만드는중
        public TextBase LEFTLEVEL = new TextBase(new Font("Verdana",18),100,Color.Black);
        public int LEFTLEVELLEFTGAP = 2;
        public int LEFTLEVELYOFFSET = 27;
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

                for(int i=0;i<drawer.YLINEAXIS.Length;i++)
                {
                    canvas.DrawLine(pineapple, LEFTGAP, drawer.YLINEAXIS[i], drawer.MAX_WIDTH-drawer.RIGHTGAP, drawer.YLINEAXIS[i]);
                }
            }

            if(SHOWLEFTLEVEL)
            {
                for (int i = 0; i < drawer.BOX_YCOUNT; i++)
                {
                    int count = drawer.LEFTLEVELS.Skip(i+1).TakeWhile((val) => val == drawer.LEFTLEVELS[i]).Count();

                    int yaxis1 = drawer.YCOVERAXIS[i];
                    int yaxis2 = drawer.YCOVERAXIS[i+count];

                    int yaxis = (yaxis1 + yaxis2) / 2 + LEFTLEVELYOFFSET;



                    var sm = LEFTLEVEL.Draw(drawer, drawer.LEFTLEVELS[i].ToString() + LEFTLEVELSUFFIX, LEFTLEVELLEFTGAP, yaxis, 2);

                    canvas.DrawImage(sm.image, new Point(sm.rect.X, sm.rect.Y));

                    i += count;
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
        public STATEMAP TETRIS;


        public interface IImageDrawable
        {
            public StackableImage SelfDraw(ECMDrawer drawer);
        }

        public class STATEMAP
        {
            STATE[,] map;
            ECMProject project;


            public int Height= int.MinValue;
            public int Width=int.MinValue;

            public int Gapless;

            public double[] lvs;



            /*
            public IEnumerable<STATE> GetXLines(int y, out int index)
            {
                for(index=0;index<Width;index++)
                {
                    yield return map[index, y];
                }
                yield break;
            }
            */


            public STATE this[int x,int y]
            {
                get
                {
                    return this.map[x, y];
                }
                set
                {
                    this.map[x,y] = value;
                }
            }

            public STATEMAP(ECMProject project, bool doRefresh = true)
            {
                this.project = project;
                if (doRefresh)
                {
                    this.Refresh();
                }
            }

            public void Refresh()
            {
                Reset();
                Update();
            }
            public void Update()
            {
                StackProject(project);
            }
            public void Reset()
            {
                this.map = new STATE[256, 256];
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        map[i, j] = new STATE.NULL();
                    }
                }
                lvs = new double[256];
            }

            void StackProject(ECMProject project)
            {
                StackLevels(project.LevelList);
            }

            void StackLevels(Dictionary<double, List<ECMLevel>> levelList)
            {
                foreach(var level in levelList)
                {
                    StackLine(level.Key, level.Value);
                }
            }

            void StackLine(double lv, List<ECMLevel> levels)
            {
                int currenty = 0;
                int MAXBOXCOUNT = project.option.divider.MAXBOXCOUNT;

                for (int i = 0; i < 256; i++)
                {
                    if (map[0, i].GetType() == typeof(STATE.NULL))
                    {
                        currenty = i;
                        map[0, i] = new STATE.EMPTY();
                        break;
                    }
                }

                int xcount = levels.Count();


                int realy = Height;
                int realx;
                for (int x = 0; x < xcount; x++)
                {
                    realx = x % MAXBOXCOUNT;
                    realy = currenty + (int)Math.Floor((double)x / MAXBOXCOUNT);
                    map[realx, realy] = new STATE.LEVEL(levels[x]);

                    lvs[realy] = lv;
                }

                Height = realy+1;
                Width = Math.Max(Math.Min(levels.Count(),MAXBOXCOUNT), Width);
            }


        }
        public interface STATE
        {
            public class NULL : STATE
            {

            }
            public class EMPTY : STATE
            {

            }
            public class LEVEL : STATE
            {
                public ECMLevel data;

                public LEVEL(ECMLevel data)
                {
                    this.data = data;
                }
            }
            public class LEVELRANGED : STATE
            {
                public ECMLevel data;

                public LEVELRANGED(ECMLevel data)
                {
                    this.data = data;
                }
            }
            public class LEVELRANGEDLEG : STATE
            {

            }
            public class LEVELRANGEDTOP : STATE
            {

            }
            public class LEVELRANGEDBOTTOM : STATE
            {

            }
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









            TETRIS = new STATEMAP(project);

            BOX_YCOUNT = TETRIS.Height;
            BOX_XCOUNT = TETRIS.Width;

            LEFTLEVELS = TETRIS.lvs;




            MAX_HEIGHT = TOPGAP + (BOX_YCOUNT * (BOXSIZE + BOXHEIGHTGAP)) + BOTTOMGAP;
            MAX_WIDTH = LEFTGAP + (BOX_XCOUNT * (BOXSIZE + BOXWIDTHGAP)) + RIGHTGAP;

            //YLINEAXIS, YCOVERAXIS
            {
                List<int> yCoverAxisList = new List<int>();
                List<int> yLineAxisList = new List<int>();
                int currenty = TOPGAP + BOXHEIGHTGAP;

                for (int i = 0; i < TETRIS.Height; i++)
                {

                    yCoverAxisList.Add(currenty);


                    //CheckTopSame
                    if (i > 0)
                    {
                        if (LEFTLEVELS[i] == LEFTLEVELS[i-1])
                        {

                        }
                        else
                        {
                            yLineAxisList.Add(currenty);
                        }
                    }


                    //CheckBottomSame
                    if (i < TETRIS.Height - 1)
                    {
                        if (LEFTLEVELS[i] == LEFTLEVELS[i + 1])
                        {
                            currenty += BOXSIZE;
                        }
                        else
                        {
                            yLineAxisList.Add(currenty + BOXSIZE);

                            currenty += BOXSIZE + BOXHEIGHTGAP;
                        }
                    }

                    
                }

                YLINEAXIS = yLineAxisList.ToArray();
                YCOVERAXIS = yCoverAxisList.ToArray();
            }

            








            Image output = GetEmptyBitmap();
            List<StackableImage> drawStack = new List<StackableImage>();


            //곡 디스크이미지 스택
            for (int y=0;y< TETRIS.Height; y++)
            {
                for(int x=0;x<TETRIS.Width;x++)
                {
                    int x2 = LEFTGAP + (x * (BOXSIZE + BOXWIDTHGAP));
                    int y2 = YCOVERAXIS[y];

                    switch(TETRIS[x,y])
                    {
                        case STATE.EMPTY:
                        case STATE.NULL:
                            break;

                        case STATE.LEVEL level:
                            var w = level.data.SelfDraw(this);
                            w.rect = new Rectangle(x2, y2, BOXSIZE, BOXSIZE);
                            drawStack.Add(w);
                            break;

                        default:
                            throw new Exception("?>???");
                    }

                    
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