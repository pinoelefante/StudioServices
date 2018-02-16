﻿using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ProjectRunner.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Dictionary<string, Type> _pagesByKey = new Dictionary<string, Type>();
        private NavigationPage _navigation;
        private string MainPageKey;

        public string CurrentPageKey
        {
            get
            {
                lock (_pagesByKey)
                {
                    if (_navigation.CurrentPage == null)
                    {
                        return null;
                    }

                    var pageType = _navigation.CurrentPage.GetType();

                    return _pagesByKey.ContainsValue(pageType)
                        ? _pagesByKey.First(p => p.Value == pageType).Key
                        : null;
                }
            }
        }
        public Action ExitAction { get; set; }
        public void GoBack()
        {
            if (_navigation.Navigation.NavigationStack.Count > 1)
            {
                var taskNav = _navigation.PopAsync();
                taskNav.ContinueWith((task) =>
                {
                    Page page = task.Result;
                    if (MainPageKey != null)
                    {
                        Type mainPageType = _pagesByKey[MainPageKey];
                        Type currPageType = page.GetType();
                        if (mainPageType == currPageType)
                            ClearBackstack();
                    }
                });
            }
            else
                ExitAction?.Invoke();
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }
        private static object objtype = new object();
        public void NavigateTo(string pageKey, object parameter)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(pageKey))
                {
                    var type = _pagesByKey[pageKey];
                    ConstructorInfo constructor;
                    object[] parameters;

                    if (parameter == null)
                    {
                        constructor = type.GetTypeInfo()
                            .DeclaredConstructors
                            .FirstOrDefault(c => !c.GetParameters().Any());

                        parameters = new object[]
                        {
                        };
                    }
                    else
                    {
                        constructor = type.GetTypeInfo()
                            .DeclaredConstructors
                            .FirstOrDefault(
                                c =>
                                {
                                    var p = c.GetParameters();
                                    return p.Count() == 1
                                           && p[0].ParameterType == objtype.GetType();  /*parameter.GetType();*/
                                });

                        parameters = new[]
                        {
                            parameter
                        };
                    }

                    if (constructor == null)
                    {
                        throw new InvalidOperationException(
                            "No suitable constructor found for page " + pageKey);
                    }
                    var page = constructor.Invoke(parameters) as Page;
                    var lastPage = _navigation.Navigation.NavigationStack.LastOrDefault();
                    
                    var task = _navigation.PushAsync(page);
                    task.ContinueWith((c) =>
                    {
                        if (MainPageKey != null && pageKey.CompareTo(MainPageKey) == 0)
                            ClearBackstack();
                    });

                    if (lastPage != null && lastPage.GetType() == page.GetType())
                        _navigation.Navigation.RemovePage(lastPage);
                }
                else
                {
                    throw new ArgumentException(
                        string.Format(
                            "No such page: {0}. Did you forget to call NavigationService.Configure?",
                            pageKey),
                        "pageKey");
                }
            }
        }

        public void Configure(string pageKey, Type pageType)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(pageKey))
                {
                    _pagesByKey[pageKey] = pageType;
                }
                else
                {
                    _pagesByKey.Add(pageKey, pageType);
                }
            }
        }
        public void ClearBackstack()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var stack = _navigation.Navigation.NavigationStack.ToList();
                for (int i = 0; i < stack.Count-1; i++)
                    _navigation.Navigation.RemovePage(stack[i]);
            });
        }
        
        public void Initialize(NavigationPage navigation, string mainPageKey, Action OnExit)
        {
            _navigation = navigation;
            MainPageKey = mainPageKey;
            ExitAction = OnExit;
        }
        public void PrintBackstack()
        {
            var stack = _navigation.Navigation.NavigationStack.ToList();
            foreach (var item in stack)
            {
                Debug.WriteLine("classid: "+item.ClassId + " typeof: "+item.GetType().FullName);
            }
        }
        public void RemovePagesFromBackstack(IEnumerable<Type> types)
        {
            foreach (var item in types)
                RemovePageFromBackstack(item);
        }
        public void RemovePageFromBackstack(Type t, bool one = false)
        {
            var stack = _navigation.Navigation.NavigationStack.ToList();
            foreach (var item in stack)
            {
                if(item.GetType() == t)
                {
                    _navigation.Navigation.RemovePage(item);
                    if (one)
                        break;
                }
            }
        }
    }
}
