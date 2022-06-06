using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMBase
{

    public abstract class GridMapBase<T>
    {

        public abstract Rectangle rect { get; }

        public T? GetAt(Point pos)
        {
            if (rect.ContainsBoundary(pos))
                return _GetAt(pos.X - rect.X, pos.Y - rect.Y);
            else return default;
        }
        public bool IsNullorDefault(Point pos) => GetAt(pos) == null;

        public Point IndexOf(T item)
        {
            foreach (Point p in DT.Filler(rect))
            {
                if (GetAt(p).Equals(item)) return p;
            }
            return default;
        }
        public IEnumerable<T> ValuesByPositions(IEnumerable<Point> poss)
        {
            foreach (Point pos in poss)
            {
                yield return GetAt(pos);
            }
            yield break;
        }
        public IEnumerable<T> ValuesByY(int y) => ValuesByPositions(DT.Mover(DT.Right, rect.Height).ToPositions(new Point(0, y)));
        public IEnumerable<T> ValuesByX(int x) => ValuesByPositions(DT.Mover(DT.Down, rect.Width).ToPositions(new Point(x, 0)));


        protected abstract T _GetAt(int x, int y);
    }

    public class GridMap<T> : GridMapBase<T>
    {
        ReadOnlyCollection<ReadOnlyCollection<T>> map { get; init; }
        Rectangle _rect;
        public override Rectangle rect => _rect;



        public GridMap(IList<IList<T>> basemap, Rectangle rect)
        {
            this._rect = rect;
            List<ReadOnlyCollection<T>> clist = new List<ReadOnlyCollection<T>>();
            foreach(var v in basemap)
            {
                clist.Add(new ReadOnlyCollection<T>(v));
            }
            this.map = new ReadOnlyCollection<ReadOnlyCollection<T>>(clist);
        }

        protected override T _GetAt(int x, int y) => map[y][x];
    }
    public class GridMapBuilder<T> : GridMapBase<T>
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
        public override Rectangle rect => _rect;

        public GridMapBuilder()
        {
            map = new ObservableCollection<ObservableCollection<T>>();
            map.Add(GetRow());
            _rect = new Rectangle(0, 0, 0, 0);
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

        void Extend(Size direction)
        {
            if(direction == DT.Up)
            {
                Log.Message("Up");
                map.Insert(0, GetRow());
            }
            else if(direction == DT.Down)
            {
                Log.Message("Down");
                map.Add(GetRow());
            }
            else if(direction == DT.Left)
            {
                Log.Message("Left");
                foreach (var v in map)
                {
                    v.Insert(0, default);
                }
            }
            else if(direction == DT.Right)
            {
                Log.Message("Right");
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




        public T[,] ToArray()
        {
            T[,] arraymap = new T[rect.Width,rect.Height];
            foreach (Point p in DT.Filler(rect))
            {
                arraymap[p.X-rect.X, p.Y-rect.Y] = GetAt(p);
            }
            return arraymap;
        }
        public GridMap<T> ToMap()
        {
            return new GridMap<T>(this.map.Cast<IList<T>>().ToArray(), this.rect);
        }

        public BlockMap<T> ToBlockMap(Size middle)
        {
            return new BlockMap<T>(this.map.Cast<IList<T>>().ToArray(), this.rect, middle);
        }

        protected override T _GetAt(int x, int y)
        {
            return map[y][x];
        }
    }



    public class BlockMap<T> : GridMap<T>
    {
        public Size middle { get; init; }
        public BlockMap(IList<IList<T>> basemap, Rectangle rect, Size middle) : base(basemap, rect)
        {
            this.middle = middle;
        }
    }


    public class PuzzleMap<T> : GridMapBase<T>
    {
        public override Rectangle rect => throw new NotImplementedException();

        public void TryStackBlock(GridMap<T> block, IEnumerator<Point> location)
        {

        }
        public void TryStackBlock()
        {

        }

        protected override T _GetAt(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
    public class PuzzleMapBuilder<T>
    {

        List<(Size offset, BlockMap<T> map)> puzzlemap;

        Rectangle rect;

        public PuzzleMapBuilder()
        {
            this.puzzlemap = new List<(Size offset, BlockMap<T> map)>();
            this.rect = new Rectangle(0,0,0,0);
        }

        public int StackMap(BlockMap<T> map) => StackMap(map, new Size(0, 0));
        public int StackMap(BlockMap<T> map, Size offset)
        {
            this.puzzlemap.Add((offset-map.middle, map));
            return puzzlemap.Count - 1;
        }

        public bool TryStackMap(BlockMap<T> map, IEnumerable<Size> offsets)
        {
            foreach(Size offset in offsets)
            {
                Size newoffset = offset - (Size)map.middle;
                if(TryStackMap(map, newoffset))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryStackMap(BlockMap<T> map, Size offset)
        {
            var poss = DT.Filler(map.rect);

            puzzlemap.Add((offset, map));

            var b = IsPuzzled(poss);

            if(!b) puzzlemap.RemoveAt(puzzlemap.Count-1);

            return b;
        }

        public IEnumerable<T> XRay(Point pos)
        {
            foreach ((Size offset, GridMapBase<T> map) in puzzlemap)
            {
                yield return map.GetAt(pos + offset);
            }
            yield break;
        }

        public bool IsPuzzled(IEnumerable<Point> poss)
        {
            foreach(Point pos in poss)
            {
                if (!IsPuzzled(pos))
                    return false;
            }
            return true;
        }
        public bool IsPuzzled(Point pos)
        {
            bool isConquered = false;
            foreach ((var offset, var mapcomp) in puzzlemap)
            {
                if (!mapcomp.IsNullorDefault(pos))
                {
                    if(isConquered)
                        return false;
                    isConquered = true;
                }
            }
            return true;
        }

        public void Move(int index, Size offset)
        {
            var tuple = puzzlemap[index];
            tuple.offset += offset;
        }

        public void GetPuzzleMap()
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
            int left = rect.Left - point.X;
            int right = point.X - rect.Right;
            int top = rect.Top - point.Y;
            int bottom = point.Y - rect.Bottom;

            int xoffset = (left, right) switch
            {
                var (l, r) when l <= 0 && r <= 0 => 0,
                var (l, r) when l >= 0 && r <= 0 => -left,
                var (l, r) when l <= 0 && r >= 0 => right,
                var (l, r) when l > 0 && r > 0 => throw new Exception("???????"),
                var (_,_) => throw new Exception("??")
            };
            int yoffset = (top, bottom) switch
            {
                var (t, b) when t <= 0 && b <= 0 => 0,
                var (t, b) when t >= 0 && b <= 0 => -top,
                var (t, b) when t <= 0 && b >= 0 => bottom,
                var (t, b) when t > 0 && b > 0 => throw new Exception("???????"),
                var (_, _) => throw new Exception("??")
            };

            //int xoffset = Math.Min(point.X - rect.Left, point.X - rect.Right);
            //int yoffset = Math.Min(point.Y - rect.Top, point.Y - rect.Bottom);
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


        public static IEnumerable<Point> OffsetToPosition(IEnumerable<Size> offsets, Point home)
        {
            Point mover = home;
            foreach(Size offset in offsets)
            {
                mover += offset;
                yield return mover;
            }
            yield break;
        }
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

        public static IEnumerable<Point> ToPositions(this IEnumerable<Size> offsets, Point home) => DT.OffsetToPosition(offsets, home);
    }


    public static class ExtensionMethod
    {
        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach(var v in values)
            {
                action(v);
            }
        }
    }
}
