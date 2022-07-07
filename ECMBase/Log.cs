using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMBase
{
    public static class Log
    {
        public static void Message(string str)
        {
            Console.WriteLine(str);
        }
        public static void Message(object? str) => Console.WriteLine(str);
        public static void Message(object?[] str) => str.ForEach((s)=> Message(s));

        public static void Warning(string str)
        {
            Console.WriteLine(str);
        }


        public static void Start(string name, int lev=0)
        {
            Debug($"{name}", lev);
            Debug("{");
            Names.Add(name);
        }
        public static void End(int lev=0)
        {
            Names.RemoveAt(Names.Count - 1);
            Debug("}", lev);
        }
        public static void Debug(string str, int lev=0)
        {
            if (lev >= Lev) Message($"{Tabs(Names.Count)}{str}");
        }
        public static void Debug(string name, string obj, int lev = 0)
        {
            if (lev >= Lev) Message($"{Tabs(Names.Count)}{name} : {obj}");
        }
        public static void Debug<T>(string name, IEnumerable<T> values, int lev = 0)
        {
            Start(name, lev);
            values.ForEach((v) => Debug(v.ToString(), lev));
            End(lev);
        }
        public static void Debug(string name, object? obj, int lev = 0)
        {
            if (lev >= Lev) Message($"{Tabs(Names.Count)}{name} : {obj}");
        }




        public static int Lev=0;

        static List<string> Names = new List<string>();

        static readonly string Tab = "    ";

        static string Tabs(int count)
        {
            StringBuilder sb = new StringBuilder();
            for(int i=0;i<count;i++)
            {
                sb.Append(Tab);
            }
            return sb.ToString();
        }
    }
}
