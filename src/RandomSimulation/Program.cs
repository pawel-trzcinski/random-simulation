using System;
using System.Diagnostics;

namespace RandomSimulation
{
    public static class Program
    {
        [MTAThread]
        public static void Main(string[] args)
        {
            Trace.WriteLine(Engine.TestModule.hello("Pawel"));
        }
    }
}
