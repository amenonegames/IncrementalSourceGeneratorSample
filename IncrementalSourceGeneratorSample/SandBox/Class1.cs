using System;
using SourceGeneratorSample;

namespace SandBox
{
    [Sample]
    public partial class Class1
    {
        public Class1()
        {
        }
    }

    public class Class2
    {
        private void Test()
        {
            var c1 = new Class1();
            c1.Hello();
        }   
    }

}

