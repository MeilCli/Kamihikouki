using Android.App;
using Android.OS;
using Android.Util;
using Java.IO;
using Java.Lang;
using Kamihikouki.NETStandard;
using System;
using System.Collections.Generic;

namespace Kamihikouki.Platform.Android
{
    public class FragmentTransitionProvider : ITransitionProvider
    {
        private string argumentKey => NavigationProvider.ArgumentKey;
        private FragmentManager fragmentManager;

        public FragmentTransitionProvider(FragmentManager fragmentManager)
        {
            this.fragmentManager = fragmentManager;
        }

        public void Push<T>(T parameter, Type targetView)
        {
            var fragment = Activator.CreateInstance(targetView) as Fragment
                ?? throw new InvalidOperationException("target view is not Fragment");
            fragment.Arguments = ResolveBundle<T>(fragment.Arguments, parameter);

            FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();
            fragmentTransaction.Add(fragment, targetView.Name);
            fragmentTransaction.AddToBackStack(targetView.Name);
            fragmentTransaction.Commit();
        }

        public void Pop<T>(T parameter, Type targetView)
        {
            if (targetView != null)
            {
                fragmentManager.PopBackStack(targetView.Name, PopBackStackFlags.Inclusive);
            }
            else
            {
                fragmentManager.PopBackStack();
            }
        }

        protected Bundle ResolveBundle<T>(Bundle initializedBundle, T parameter)
        {
            Bundle bundle = initializedBundle ?? new Bundle();
            switch (parameter)
            {
                case IBinder p:
                    bundle.PutBinder(argumentKey, p);
                    break;
                case Bundle p:
                    bundle.PutBundle(argumentKey, p);
                    break;
                case IParcelable p:
                    bundle.PutParcelable(argumentKey, p);
                    break;
                case IParcelable[] p:
                    bundle.PutParcelableArray(argumentKey, p);
                    break;
                case IList<IParcelable> p:
                    bundle.PutParcelableArrayList(argumentKey, p);
                    break;
                case ISerializable p:
                    bundle.PutSerializable(argumentKey, p);
                    break;
                case ICharSequence p:
                    bundle.PutCharSequence(argumentKey, p);
                    break;
                case ICharSequence[] p:
                    bundle.PutCharSequenceArray(argumentKey, p);
                    break;
                case IList<ICharSequence> p:
                    bundle.PutCharSequenceArrayList(argumentKey, p);
                    break;
                case IList<Integer> p:
                    bundle.PutIntegerArrayList(argumentKey, p);
                    break;
                case IStateSerializable p:
                    bundle.PutByteArray(argumentKey, p.StateSerialize());
                    break;
                case Size p:
                    bundle.PutSize(argumentKey, p);
                    break;
                case SizeF p:
                    bundle.PutSizeF(argumentKey, p);
                    break;
                case SparseArray p:
                    bundle.PutSparseParcelableArray(argumentKey, p);
                    break;
                case bool p:
                    bundle.PutBoolean(argumentKey, p);
                    break;
                case bool[] p:
                    bundle.PutBooleanArray(argumentKey, p);
                    break;
                case byte[] p:
                    bundle.PutByteArray(argumentKey, p);
                    break;
                case char p:
                    bundle.PutChar(argumentKey, p);
                    break;
                case char[] p:
                    bundle.PutCharArray(argumentKey, p);
                    break;
                case double p:
                    bundle.PutDouble(argumentKey, p);
                    break;
                case double[] p:
                    bundle.PutDoubleArray(argumentKey, p);
                    break;
                case short p:
                    bundle.PutShort(argumentKey, p);
                    break;
                case short[] p:
                    bundle.PutShortArray(argumentKey, p);
                    break;
                case int p:
                    bundle.PutInt(argumentKey, p);
                    break;
                case int[] p:
                    bundle.PutIntArray(argumentKey, p);
                    break;
                case long p:
                    bundle.PutLong(argumentKey, p);
                    break;
                case long[] p:
                    bundle.PutLongArray(argumentKey, p);
                    break;
                case sbyte p:
                    bundle.PutByte(argumentKey, p);
                    break;
                case float p:
                    bundle.PutFloat(argumentKey, p);
                    break;
                case float[] p:
                    bundle.PutFloatArray(argumentKey, p);
                    break;
                case string p:
                    bundle.PutString(argumentKey, p);
                    break;
                case string[] p:
                    bundle.PutStringArray(argumentKey, p);
                    break;
                // string[]より前にIList<string>があるとコンパイルエラー in C#7.2
                case IList<string> p:
                    bundle.PutStringArrayList(argumentKey, p);
                    break;
                default: throw new InvalidOperationException("invalid type to bundle extra");
            }
            return bundle;
        }
    }
}