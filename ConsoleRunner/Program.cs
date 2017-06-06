using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleRunner
{
    class ConcreteStrategy : IStrategy
    {
        public void ModifyMatrix(int[] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i]++;
            }
        }
    }

    internal interface IStrategy
    {
        void ModifyMatrix(int[] matrix);
    }


    class Program
    {
        static void Main(string[] args)
        {
            NextGenSpice.Misc.HilbertMatrixStabilityTest();
        }
    }
}