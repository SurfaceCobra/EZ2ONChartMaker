
namespace ECMBase
{
    public static class Beauty
    {

    }


    public interface IRanged
    {
        public Range range { get; init; }
    }



    public class Ranged<T> : IRanged
    {
        public Ranged(T item, Range range)
        {
            this.o = item;
            this.range = range;
        }

        public T o { get; init; }
        public Range range { get; init; }

        public static implicit operator T(Ranged<T> rs) => rs.o;

        public Ranged<T2> Convert<T2>(T2 newitem)
        {
            return new Ranged<T2>(newitem, range);
        }


        public static Ranged<T> Merge(T item, IRanged start, IRanged end)
        {
            return new Ranged<T>(item, start.range.Start..end.range.End);
        }

        public static Ranged<T> Merge(T item, params IRanged[] iranges)
        {
            int start = iranges.Select(x => x.range.Start.Value).Min();
            int end = iranges.Select(x => x.range.End.Value).Max();
            return new Ranged<T>(item, start..end);
        }

        public override string ToString() => this.o.ToString();





    }

    public static class Ranged
    {
        public static Range Merge(params Range[] ranges) => Merge(ranges);
        public static Range Merge(IEnumerable<Range> ranges)
        {
            int start = ranges.Select(x => x.Start.Value).Min();
            int end = ranges.Select(x => x.End.Value).Max();
            return start..end;
        }
    }

    public interface IRangeInside
    {
        public Range range { get; }

    }


}
