using BenchmarkDotNet.Running;
using System.Reflection;

namespace Kamihikouki.Benchmark.NETCore
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
        }
    }
}
