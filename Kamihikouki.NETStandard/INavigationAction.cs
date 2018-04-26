using System.Threading.Tasks;

namespace Kamihikouki.NETStandard
{
    public interface INavigationAction
    {
        Task NavigateAsync<TParam>(INavigationRequest navigationRequest, TParam parameter = default);
    }
}
