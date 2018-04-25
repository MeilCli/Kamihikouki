namespace Kamihikouki.NETStandard
{
    public interface INavigator
    {

        void MethodInject(object view, params object[] viewModels);

        void ClassInject(object view, params object[] viewModels);
    }
}
