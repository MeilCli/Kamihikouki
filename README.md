# Kamihikouhi
An ideal MVVM Navigator - Kamihikouki  
Inspired by [KAMISHIBAI for Xamarin.Forms](https://github.com/nuitsjp/KAMISHIBAI)

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
|     CachedInject | 496,597.09 ns | 5,409.5617 ns | 5,060.1069 ns | 487,335.17 ns | 506,934.37 ns | 2.9297 | 0.9766 |   14039 B |
| ReflectionInject |  37,487.55 ns |   450.5291 ns |   399.3823 ns |  36,748.56 ns |  38,106.65 ns | 1.3428 |      - |    5690 B |
|      CachedRaise |      22.50 ns |     0.1602 ns |     0.1498 ns |      22.24 ns |      22.83 ns |      - |      - |       0 B |
|  ReflectionRaise |     304.07 ns |     1.9303 ns |     1.8056 ns |     301.28 ns |     306.93 ns | 0.0148 |      - |      64 B |


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
|  PropertyCount5 | 56.71 us | 0.5632 us | 0.4993 us | 55.81 us | 57.39 us | 2.6245 |  10.93 KB |
| PropertyCount10 | 76.75 us | 0.5432 us | 0.5081 us | 75.86 us | 77.99 us | 4.2725 |  17.83 KB |


More properties is higher cost when Inject().  
But, it is acceptable error range.

## License

This library is under MIT License.

And using library:

- [System.ValueTuple](https://www.nuget.org/packages/System.ValueTuple/) is under [MIT License](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)