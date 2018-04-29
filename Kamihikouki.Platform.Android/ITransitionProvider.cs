using System;

namespace Kamihikouki.Platform.Android
{
    public interface ITransitionProvider
    {
        void Push<T>(T parameter, Type targetView);

        void Pop<T>(T parameter, Type targetView);
    }
}