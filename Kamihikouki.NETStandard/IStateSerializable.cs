namespace Kamihikouki.NETStandard
{
    // for Xamarin.Android ViewModel
    public interface IStateSerializable
    {
        byte[] StateSerialize();
    }
}