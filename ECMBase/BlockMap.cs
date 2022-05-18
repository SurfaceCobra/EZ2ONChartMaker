using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMBase
{
    public class SuperMap<T>
    {
        ObservableCollection<ObservableCollection<T>> map;

        ObservableCollection<T> GetRow()
        {
            var v = new ObservableCollection<T>() { default };

            for(int i=0;i<rect.Width;i++)
            {
                v.Add(default);
            }

            return v;
        }

        Rectangle _rect;
        Rectangle rect => _rect;

        public SuperMap()
        {
            map = new ObservableCollection<ObservableCollection<T>>();
            map.Add(GetRow());
            _rect = new Rectangle(0, 0, 0, 0);
        }

        public T GetAt(Point pos)
        {
            if (rect.ContainsBoundary(pos))
                return map[pos.Y- rect.Y][pos.X- rect.X];
            else return default;
        }

        public void SetAt(Point pos, T value)
        {
            if(!rect.Contains(pos))
            {
                foreach(var d in DT.Cut(DT.Distance(rect, pos)))
                {
                    Extend(d);
                }
            }
            map[pos.Y- rect.Y][pos.X- rect.X] = value;
            return;
        }

        public void RemoveAt(Point pos)
        {
            SetAt(pos,default);

        }

        void Extend(Size direction, int count)
        {
            for (int i = 0; i < count; i++) Extend(direction);
        }

        void Extend(Size direction)
        {
            if(direction == DT.Up)
            {
                map.Insert(0, GetRow());
            }
            else if(direction == DT.Down)
            {
                map.Add(GetRow());
            }
            else if(direction == DT.Left)
            {
                foreach (var v in map)
                {
                    v.Insert(0, default);
                }
            }
            else if(direction == DT.Right)
            {
                foreach (var v in map)
                {
                    v.Add(default);
                }
            }
            else
            {
                throw new Exception("?????");
            }
            _rect = _rect.SizeOffset(direction);
        }


        public Point IndexOf(T item)
        {
            foreach(Point p in DT.Filler(rect))
            {
                if (GetAt(p).Equals(item)) return p;
            }
            return default;
        }

        public T[,] ToArray()
        {
            T[,] arraymap = new T[rect.Width,rect.Height];
            foreach (Point p in DT.Filler(rect))
            {
                arraymap[p.X-rect.X, p.Y-rect.Y] = GetAt(p);
            }
            return arraymap;
        }
    }

    public class StackedBlockMap<T>
    {
        List<(Size offset, BlockMap<T> map)> stackedmap;

        public int StackMap(BlockMap<T> map) => StackMap(map, new Size(0,0));
        public int StackMap(BlockMap<T> map, Size offset)
        {
            this.stackedmap.Add((offset,map));
            return stackedmap.Count-1;
        }

        public IEnumerable<T> XRay(Point pos)
        {
            foreach((Size offset, BlockMap<T> map) in stackedmap)
            {
                yield return map.GetAt(pos+offset);
            }
            yield break;
        }

        public void Move(int index, Size offset)
        {
            var tuple = stackedmap[index];
            tuple.offset += offset;
        }
    }
    public record class BlockMap<T>
    {
        public Point middle { get; init; }
        public T[,] map { get; init; }
        public BlockMap(Point middle, T[,] map)
        {
            this.middle = middle;
            this.map = map;
        }



        public T GetAt(Point pos) => map[pos.X,pos.Y];
        public void SetAt(T data, Point pos) => map[pos.X,pos.Y] = data;

    }
    public class BlockBuilder<T> : SuperMap<T>
    {
        public Point middle;

        public Point currentPos;

        public BlockBuilder() : base()
        {

            middle = new Point(0,0);
            currentPos = new Point(0,0);

        }
        public IEnumerable<T> ValuesByPosition(IEnumerable<Point> poss)
        {
            foreach(Point pos in poss)
            {
                yield return GetAt(pos);
            }
            yield break;
        }
        public IEnumerable<T> ValuesByOffset(Point home, IEnumerable<Size> offsets)
        {
            Point newhome = home;
            foreach (Size offset in offsets)
            {
                home += offset;
                yield return GetAt(newhome);
            }
            yield break;
        }

        public IEnumerable<T> ValuesByY(int y) => ValuesByOffset(new Point(0, y), DT.Mover(DT.Right,MAX_SIZE));
        public IEnumerable<T> ValuesByX(int x) => ValuesByOffset(new Point(x, 0), DT.Mover(DT.Down,MAX_SIZE));


        public void Move(Size offset)
        {
            currentPos += offset;
        }
        public void Set(T data) => SetAt(data, currentPos);
        public void MoveSet(T data, Size offset)
        {
            Move(offset);
            Set(data);
        }

        public BlockMap<T> GetBlockMap()
        {

            return new BlockMap<T>(middle, Trim());
        }
    }


    public class PuzzleMap<T>
    {
        public void TryStackBlock(BlockMap<T> block, Point starting, IEnumerator<Size> mover)
        {

        }
        public void TryStackBlock(BlockMap<T> block, IEnumerator<Point> location)
        {

        }
        public void TryStackBlock()
        {

        }
    }



    public static class DT
    {
        public static readonly Size Up = new Size(0, -1);
        public static readonly Size Down = new Size(0, 1);
        public static readonly Size Left = new Size(-1,0);
        public static readonly Size Right = new Size(1, 0);

        public static IEnumerable<Size> Mover(Size direction, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return direction;
            }
            yield break;
        }
        public static IEnumerable<Size> Mover(Size direction) => Mover(direction, int.MaxValue);

        public static IEnumerable<Point> Filler(Rectangle rect)
        {
            for(int x=rect.X; x<rect.Width; x++)
            {
                for(int y=rect.Y; y<rect.Height; y++)
                {
                    yield return new Point(x,y);
                }
            }
            yield break;
        }

        public static IEnumerable<Size> Cut(Size offset)
        {
            if(offset.Width > 0)
            {
                for(int i=0;i<offset.Width;i++)
                {
                    yield return DT.Right;
                }
            }
            else if (offset.Width < 0)
            {
                for (int i = 0; i > offset.Width; i--)
                {
                    yield return DT.Left;
                }
            }
            if (offset.Height > 0)
            {
                for (int i = 0; i < offset.Height; i++)
                {
                    yield return DT.Down;
                }
            }
            else if (offset.Height < 0)
            {
                for (int i = 0; i > offset.Height; i--)
                {
                    yield return DT.Up;
                }
            }

            yield break;
        }

        public static Size Distance(Rectangle rect, Point point)
        {
            int xoffset = Math.Min(point.X - rect.Left, point.X - rect.Right);
            int yoffset = Math.Min(point.Y - rect.Top, point.Y - rect.Bottom);
            return new Size(xoffset, yoffset);
        }


        public static Size Enlarge(Size size, int length)
        {
            int width = size.Width;
            int height = size.Height;
            if (width > 0) width += length;
            else if (width < 0) width -= length;
            if (height > 0) height += length;
            else if (height < 0) height -= length;
            return new Size(width, height);
        }
        public static Size Enlarge(Size size) => Enlarge(size,1);




    }
    public static class DTExtensionMethod
    {
        public static Rectangle SizeOffset(this Rectangle rect, Size offset)
        {
            int xoffset = offset.Width;
            int yoffset = offset.Height;

            if (xoffset < 0)
            {
                rect.X += xoffset;
            }
            if (yoffset < 0)
            {
                rect.Y += yoffset;
            }

            rect.Width += Math.Abs(xoffset);
            rect.Height += Math.Abs(yoffset);
            return rect;
        }

        public static bool ContainsBoundary(this Rectangle rect, Point point)
        {
            return
                point.X <= rect.Right &&
                point.X >= rect.Left &&
                point.Y <= rect.Bottom &&
                point.Y >= rect.Top;
        }
    }
}
