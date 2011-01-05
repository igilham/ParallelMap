
#if DEBUG

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace IanGilham.ParallelUtils
{
    public class TestMapExtensions
    {   
        private int square(int x)
        {
            return x * x;
        }

        public static void Main(string[] args)
        {
            var test = new TestMapExtensions();
            var input = new List<int>();
            input.AddRange(Enumerable.Range(1, 10));
            var output = (List<int>)input.Map(test.square);
            for (int i = 0; i < output.Count(); i++)
			{
                Contract.Ensures(output[i] == input[i] * input[i]);
			}
        }
    }   
}

#endif
