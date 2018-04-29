using Android.Content;
using Android.OS;
using Java.IO;
using Java.Lang;
using System;
using System.Collections.Generic;
using AActivity = Android.App.Activity;

namespace Kamihikouki.Platform.Android
{
    public class ActivityTransitionProvider : ITransitionProvider
    {

        private string argumentKey => NavigationProvider.ArgumentKey;
        private AActivity activity;

        public ActivityTransitionProvider(AActivity activity)
        {
            this.activity = activity;
        }

        public void Push<T>(T parameter, Type targetView)
        {
            Intent intent = ResolveIntent<T>(parameter, targetView);
            activity.StartActivity(intent);
        }

        public void Pop<T>(T parameter, Type targetView)
        {
            if (targetView != null)
            {
                Push<T>(parameter, targetView);
            }
            activity.Finish();
        }

        protected Intent ResolveIntent<T>(T parameter, Type targetView)
        {
            var intent = new Intent(activity, targetView);
            switch (parameter)
            {
                case Bundle p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case IParcelable p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case IParcelable[] p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case ISerializable p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case ICharSequence p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case ICharSequence[] p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case IList<ICharSequence> p:
                    intent.PutCharSequenceArrayListExtra(argumentKey, p);
                    break;
                case IList<Integer> p:
                    intent.PutIntegerArrayListExtra(argumentKey, p);
                    break;
                case IList<IParcelable> p:
                    intent.PutParcelableArrayListExtra(argumentKey, p);
                    break;
                case bool p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case bool[] p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case byte[] p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case char p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case char[] p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case double p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case double[] p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case short p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case short[] p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case int p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case int[] p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case long p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case long[] p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case sbyte p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case float p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case float[] p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case string p:
                    intent.PutExtra(argumentKey, p);
                    break;
                case string[] p:
                    intent.PutExtra(argumentKey, p);
                    break;
                // string[]より前にIList<string>があるとコンパイルエラー in C#7.2
                case IList<string> p:
                    intent.PutStringArrayListExtra(argumentKey, p);
                    break;
                default: throw new InvalidOperationException("invalid type to intent extra");
            }
            return intent;
        }
    }
}