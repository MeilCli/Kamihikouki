﻿using Kamihikouki.NETStandard.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Kamihikouki.NETStandard
{
    public sealed class Navigator : BaseNavigator
    {

        public static INavigator Inject(INavigationProvider navigationProvider, object view, params object[] viewModels)
        {
            var navigator = new Navigator(navigationProvider);
            navigator.ClassInject(view, viewModels);
            navigator.MethodInject(view, viewModels);
            return navigator;
        }

        public Navigator(INavigationProvider navigationProvider) : base(navigationProvider) { }

        public override void ClassInject(object view, params object[] viewModels)
        {
            if (NavigationProvider == null)
            {
                throw new InvalidOperationException($"{nameof(NavigationProvider)} is null");
            }

            var navigationProviderMethods = NavigationProvider.GetType()
                .GetRuntimeMethods()
                .Select(x => new { method = x, attributes = x.GetCustomAttributes<NavigationProviderAttribute>() })
                // 属性を持っているかどうか
                .Where(x => x.attributes?.Any() ?? false)
                // 返り値がTaskかどうか
                .Where(x => x.method.ReturnType == typeof(Task))
                // 型パラメーターは1つかどうか
                .Where(x => x.method.IsGenericMethod && x.method.IsGenericMethodDefinition && x.method.GetGenericArguments().Length == 1)
                .ToList();

            var navigationAttributes = view.GetType().GetTypeInfo().GetCustomAttributes<BaseNavigationAttribute>();
            var items = new List<(MethodInfo, INavigationAttribute, INavigationRequest)>();
            foreach (var navigationAttribute in navigationAttributes)
            {
                var navigationProviderMethod = navigationProviderMethods
                    // INavigationProviderに実装されているメソッドの属性に対象の属性があるかどうか
                    .Where(x => x.attributes.Any(attribute => attribute.NavigationAttributeType == navigationAttribute.GetType()))
                    // INavigationProviderに実装されているメソッドの引数が対象の属性に適しているかどうか
                    .Where(x => x.method.GetParameters().Length == shoudMethodParameterCount(navigationAttribute))
                    .FirstOrDefault();
                if (navigationProviderMethod == null)
                {
                    throw new InvalidOperationException("not provided method, " +
                        $"ViewModelType: {navigationAttribute.ViewModelType.FullName}," +
                        $" BindingPropertyName: {navigationAttribute.BindingPropertyName}");
                }

                object viewModel = getViewModel(navigationAttribute, viewModels);
                INavigationRequest navigationRequest = getNavigationRequest(navigationAttribute, viewModel);
                Type typeArgument = GetTypeArgument(navigationRequest);
                MethodInfo method = navigationProviderMethod.method.MakeGenericMethod(typeArgument);

                items.Add((method, navigationAttribute, navigationRequest));
            }

            var func = createDelegate(items, NavigationProvider);
            var navigationAction = new NavigationAction(func);

            foreach (var item in items)
            {
                item.Item3.NavigationAction = navigationAction;
            }
        }

        public override void MethodInject(object view, params object[] viewModels)
        {
            var items = new List<(MethodInfo, INavigationAttribute, INavigationRequest)>();
            foreach (var navigationMethod in view.GetType()
                .GetRuntimeMethods()
                .Select(x => new { method = x, attributes = x.GetCustomAttributes<NavigationAttribute>() })
                // 属性を持っているかどうか
                .Where(x => x.attributes?.Any() ?? false)
                // 返り値がTaskかどうか
                .Where(x => x.method.ReturnType == typeof(Task))
                // 引数が1つかどうか
                .Where(x => x.method.GetParameters().Length == 1))
            {
                foreach (var attribute in navigationMethod.attributes)
                {
                    object viewModel = getViewModel(attribute, viewModels);
                    INavigationRequest navigationRequest = getNavigationRequest(attribute, viewModel);

                    items.Add((navigationMethod.method, attribute, navigationRequest));
                }
            }

            var func = createDelegate(items, view);
            var navigationAction = new NavigationAction(func);

            foreach (var item in items)
            {
                item.Item3.NavigationAction = navigationAction;
            }
        }

        /// <summary>
        /// NavigationActionへ渡すためのデリゲートを作成
        /// </summary>
        /// <param name="items">デリゲートとするメソッド群に関する情報</param>
        /// <param name="instance">デリゲートとする実際のメソッド群が定義されているオブジェクトのインスタンス</param>
        /// <returns></returns>
        private Func<object, INavigationRequest, Task> createDelegate(
            List<(MethodInfo method, INavigationAttribute attribute, INavigationRequest request)> items, object instance)
        {
            return (x, request) =>
            {
                foreach (var item in items)
                {
                    if (item.request != request)
                    {
                        continue;
                    }
                    if (item.attribute is ITargetableNavigationAttribute targetableNavigationAttribute)
                    {
                        return (Task)item.method.Invoke(instance, new[] { x, targetableNavigationAttribute.TargetViewType });
                    }
                    return (Task)item.method.Invoke(instance, new[] { x });
                }
                throw new InvalidOperationException("Request not handled");
            };

        }
    }
}
