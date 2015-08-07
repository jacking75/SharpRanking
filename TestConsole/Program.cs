using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var tester = new PerformanceTest();
            tester.유저_추가하면서_정렬(100);
        }
    }
}
