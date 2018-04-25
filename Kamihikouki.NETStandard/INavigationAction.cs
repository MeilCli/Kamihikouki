using System.Threading.Tasks;

namespace Kamihikouki.NETStandard
{
    public interface INavigationAction
    {
        Task NavigateAsync<TParam>(TParam parameter = default);
    }
}
