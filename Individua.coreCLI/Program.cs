using Autofac;
using Individua.coreCLI.Comm;
using Individua.coreCLI.interfaces;
using System;
using System.Collections.Generic;

namespace Individua.coreCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            int a = -42;
            Console.WriteLine(Convert.ToString(a, 2));
            StrTo2jz.Show(StrTo2jz.StringToBinary("12"));
            Console.ReadKey();

        }
        
    }
}
