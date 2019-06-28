using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingCode
{
    class IsTest
    {
        Base myBase;
        ChildA myChildA;
        ChildB myChildB;
        Other myOther;

        public void Run()
        {
            myBase = new Base();
            myChildA = new ChildA();
            myChildB = new ChildB();
            myOther = new Other();

            Console.WriteLine("myBase is Base = {0}", (myBase is Base));
            Console.WriteLine("myBase is ChildA = {0}", (myBase is ChildA));
            Console.WriteLine("myBase is ChildB = {0}", (myBase is ChildB));
            Console.WriteLine("myBase is Other = {0}", (myBase is Other));
            Console.WriteLine();

            Console.WriteLine("myChildA is Base = {0}", (myChildA is Base));
            Console.WriteLine("myChildA is ChildA = {0}", (myChildA is ChildA));
            Console.WriteLine("myChildA is ChildB = {0}", (myChildA is ChildB));
            Console.WriteLine("myChildA is Other = {0}", (myChildA is Other));
            Console.WriteLine();

            Console.WriteLine("myChildB is Base = {0}", (myChildB is Base));
            Console.WriteLine("myChildB is ChildA = {0}", (myChildB is ChildA));
            Console.WriteLine("myChildB is ChildB = {0}", (myChildB is ChildB));
            Console.WriteLine("myChildB is Other = {0}", (myChildB is Other));
            Console.WriteLine();

            Console.ReadKey();
        }
    }

    class Base { }
    class ChildA : Base { }
    class ChildB : Base { }
    class Other { }
}
