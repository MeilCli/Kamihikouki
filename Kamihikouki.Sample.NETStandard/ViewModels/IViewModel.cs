namespace Kamihikouki.Sample.NETStandard.ViewModels
{
    interface IViewModel
    {
        byte[] StateSerialize();

        void StateDeserialize(byte[] jsonRaw);
    }
}
