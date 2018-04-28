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
|           Method |          Mean |         Error |        StdDev |           Min |           Max |  Gen 0 |  Gen 1 | Allocated |
|----------------- |--------------:|--------------:|--------------:|--------------:|--------------:|-------:|-------:|----------:|
|     CachedInject | 482,894.91 ns | 3,927.6383 ns | 3,673.9149 ns | 476,375.23 ns | 489,216.35 ns | 3.4180 | 1.4648 |   14703 B |
| ReflectionInject |  36,393.87 ns |   244.1481 ns |   228.3763 ns |  36,107.17 ns |  36,890.00 ns | 1.4038 |      - |    5898 B |
|      CachedRaise |      20.70 ns |     0.2311 ns |     0.2048 ns |      20.44 ns |      21.17 ns |      - |      - |       0 B |
|  ReflectionRaise |     318.24 ns |     3.0373 ns |     2.6925 ns |     314.47 ns |     325.09 ns | 0.0434 |      - |     184 B |


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
|  PropertyCount5 | 54.62 us | 0.4447 us | 0.4160 us | 53.99 us | 55.29 us | 2.8076 |  11.73 KB |
| PropertyCount10 | 75.86 us | 0.6667 us | 0.6236 us | 74.83 us | 76.89 us | 4.6387 |  19.37 KB |


More properties is higher cost when Inject().  
But, it is acceptable error range.

## License

This library is under MIT License.

And using library:

- [System.ValueTuple](https://www.nuget.org/packages/System.ValueTuple/) is under [MIT License](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)