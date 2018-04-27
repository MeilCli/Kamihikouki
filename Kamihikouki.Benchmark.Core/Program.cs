using BenchmarkDotNet.Running;
using System.Reflection;

namespace Kamihikouki.Benchmark.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
        }
    }
}
