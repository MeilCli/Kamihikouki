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
|           Method |          Mean |         Error |       StdDev |        Median |           Min |           Max |  Gen 0 |  Gen 1 | Allocated |
|----------------- |--------------:|--------------:|-------------:|--------------:|--------------:|--------------:|-------:|-------:|----------:|
|     CachedInject | 485,493.59 ns | 5,156.1627 ns | 4,823.077 ns | 483,618.51 ns | 480,195.23 ns | 496,770.81 ns | 3.4180 | 1.4648 |   14735 B |
| ReflectionInject |  37,939.04 ns |   847.3127 ns | 1,131.138 ns |  37,690.52 ns |  36,556.66 ns |  41,141.38 ns | 1.4038 |      - |    5898 B |
|      CachedRaise |      20.84 ns |     0.4647 ns |     1.166 ns |      20.27 ns |      19.74 ns |      24.13 ns |      - |      - |       0 B |
|  ReflectionRaise |     322.27 ns |     1.1778 ns |     1.102 ns |     322.38 ns |     320.47 ns |     324.28 ns | 0.0434 |      - |     184 B |


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
|  PropertyCount5 | 54.78 us | 0.2296 us | 0.2148 us | 54.39 us | 55.25 us | 2.8076 |  11.73 KB |
| PropertyCount10 | 77.72 us | 0.6371 us | 0.5648 us | 76.62 us | 78.92 us | 4.6387 |  19.37 KB |


More properties is higher cost when Inject().  
But, it is acceptable error range.

## License

This library is under MIT License.

And using library:

- [System.ValueTuple](https://www.nuget.org/packages/System.ValueTuple/) is under [MIT License](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)