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
|           Method |           Mean |          Error |         StdDev |
|----------------- |---------------:|---------------:|---------------:|
|     CachedInject | 4,956,815.8 ns | 62,696.2799 ns | 58,646.1334 ns |
| ReflectionInject |   371,863.5 ns |  3,408.8187 ns |  3,021.8287 ns |
|      CachedRaise |       220.5 ns |      0.6935 ns |      0.5791 ns |
|  ReflectionRaise |     3,345.0 ns |     33.6479 ns |     31.4743 ns |


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
|          Method |     Mean |    Error |   StdDev |
|---------------- |---------:|---------:|---------:|
|  PropertyCount5 | 564.9 us | 4.392 us | 4.109 us |
| PropertyCount10 | 791.5 us | 3.511 us | 3.284 us |

More properties is higher cost when Inject().  
But, it is acceptable error range.

## License

This library is under MIT License.

And using library:

- [System.ValueTuple](https://www.nuget.org/packages/System.ValueTuple/) is under [MIT License](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)