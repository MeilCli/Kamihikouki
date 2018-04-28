# Kamihikouhi
An ideal MVVM Navigator - Kamihikouki  
Inspired by [KAMISHOBAI for Xamarin.Forms](https://github.com/nuitsjp/KAMISHIBAI)

**Be still making this, now!**  
Please give time for this job.

## Benchmark

Using [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)`s benchmark is under.

``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.16299.371 (1709/FallCreatorsUpdate/Redstone3)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3328122 Hz, Resolution=300.4698 ns, Timer=TSC
.NET Core SDK=2.1.104
  [Host] : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  Core   : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|           Method |          Mean |         Error |         StdDev |           Min |           Max |  Gen 0 |  Gen 1 | Allocated |
|----------------- |--------------:|--------------:|---------------:|--------------:|--------------:|-------:|-------:|----------:|
|     CachedInject | 504,195.05 ns | 9,579.2652 ns | 11,403.4339 ns | 482,220.59 ns | 532,506.14 ns | 3.4180 | 1.4648 |   14887 B |
| ReflectionInject |  35,509.79 ns |   197.6164 ns |    175.1817 ns |  35,242.78 ns |  35,835.17 ns | 1.4038 |      - |    5898 B |
|      CachedRaise |      20.70 ns |     0.4236 ns |      0.3962 ns |      19.99 ns |      21.19 ns |      - |      - |       0 B |
|  ReflectionRaise |     316.65 ns |     2.6040 ns |      2.4358 ns |     314.06 ns |     321.28 ns | 0.0434 |      - |     184 B |


CachedNavigator is high cost when Inject().  
But low cost when RaiseAsync().

``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.16299.371 (1709/FallCreatorsUpdate/Redstone3)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3328122 Hz, Resolution=300.4698 ns, Timer=TSC
.NET Core SDK=2.1.104
  [Host] : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  Core   : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|          Method |     Mean |     Error |    StdDev |      Min |      Max |  Gen 0 | Allocated |
|---------------- |---------:|----------:|----------:|---------:|---------:|-------:|----------:|
|  PropertyCount5 | 55.28 us | 1.1038 us | 1.7185 us | 53.16 us | 60.27 us | 2.8076 |  11.73 KB |
| PropertyCount10 | 74.67 us | 0.4624 us | 0.3861 us | 74.10 us | 75.32 us | 4.6387 |  19.37 KB |


More properties is higher cost when Inject().  
But, it is acceptable error range.

## License

This library is under MIT License.

And using library:

- [System.ValueTuple](https://www.nuget.org/packages/System.ValueTuple/) is under [MIT License](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)