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

        public static void Warning(string str)
        {
            Console.WriteLine(str);
        }
    }
}
