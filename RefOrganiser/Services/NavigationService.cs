using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;
using RefOrganiser.Helpers;
using RefOrganiser.Views;

namespace RefOrganiser.Services
{
    public class NavigationService : INavigationService
    {
        public NavigationService(Type defaultPage)
        {
            DefaultPage = defaultPage;
            Pages = new Dictionary<string, Type>();
            Navigated += NavigationSuccess;
        }

        private Frame _frame;

        public Frame Frame
        {
            get
            {
                if (_frame == null)
                {
                    _frame = Window.Current.Content as Frame;
                    RegisterFrameEvents();
                }

                return _frame;
            }

            set
            {
                UnregisterFrameEvents();
                _frame = value;
                RegisterFrameEvents();
            }
        }

        private Dictionary<string, Type> Pages { get; set; }


        public bool CanGoBack => Frame.CanGoBack;

        public bool CanGoForward => Frame.CanGoForward;

        public void GoBack() => Frame.GoBack();

        public void GoForward() => Frame.GoForward();

        public void RegisterPage(Type pageType)
        {
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled; on page to enable back
            if (Pages.ContainsKey(pageType.FullName))
            {
                throw new ArgumentException(string.Format("ExceptionNavigationServiceExKeyIsInNavigationService".GetLocalized(), pageType.FullName));
            }

            if (Pages.Any(p => p.Value == pageType))
            {
                throw new ArgumentException(string.Format("ExceptionNavigationServiceExTypeAlreadyConfigured".GetLocalized(), Pages.First(p => p.Value == pageType).Key));
            }

            Pages.Add(pageType.FullName, pageType);
            
        }

        public Type DefaultPage { get; }
        public string GetKeyForPage(Type pageType)
        {
            if (!Pages.ContainsKey(pageType.FullName))
            {
                throw new ArgumentException(string.Format("ExceptionNavigationServiceExPageUnknow".GetLocalized(), pageType.FullName));
            }
            return pageType.FullName;
        }

        public bool NavigateTo(Type pageType)
        {
            lock (Pages)
            {
                if (!Pages.ContainsKey(pageType.FullName))
                {
                    throw new ArgumentException(string.Format("ExceptionNavigationServiceExPageNotFound".GetLocalized(), pageType.FullName), pageType.FullName);
                }

            }
            //If we aren't currently on the page we wish to navigate to
            if (Frame.Content?.GetType() != pageType)
            {
                return Frame.Navigate(pageType);
            }
            return false;
        }

        public bool NavigateTo(Type pageType, object parameter)
        {
            lock (Pages)
            {
                if (!Pages.ContainsKey(pageType.FullName))
                {
                    throw new ArgumentException(string.Format("ExceptionNavigationServiceExPageNotFound".GetLocalized(), pageType.FullName), pageType.FullName);
                }

            }
            //If we aren't currently on the page we wish to navigate to
            if (Frame.Content?.GetType() != pageType)
            {
                return Frame.Navigate(pageType, parameter);
            }
            return false;
        }

        public bool NavigateTo(Type pageType, object parameter, NavigationTransitionInfo transition)
        {
            lock (Pages)
            {
                if (!Pages.ContainsKey(pageType.FullName))
                {
                    throw new ArgumentException(string.Format("ExceptionNavigationServiceExPageNotFound".GetLocalized(), pageType.FullName), pageType.FullName);
                }
                
            }
            //If we aren't currently on the page we wish to navigate to
            if (Frame.Content?.GetType() != pageType)
            {
                return Frame.Navigate(pageType, parameter, transition);
            }
            return false;
        }

        public bool NavigateToFirst()
        {
            return NavigateTo(DefaultPage);
        }

        public bool NavigateToFirst(object parameter)
        {
            return NavigateTo(DefaultPage, parameter);
        }

        //TODO: override transition without parameter

        public bool NavigateToFirst(object parameter, NavigationTransitionInfo transition)
        {
           return NavigateTo(DefaultPage, parameter, transition);
        }

        private void RegisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated += FrameNavigated;
                _frame.NavigationFailed += FrameNavigationFailed;
            }
        }

        private void FrameNavigated(object sender, NavigationEventArgs e)
        {
            Navigated?.Invoke(sender, e);
        }

        private void UnregisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated -= FrameNavigated;
                _frame.NavigationFailed -= FrameNavigationFailed;
            }
        }

        private void FrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            NavigationFailed?.Invoke(sender, e);
        }

        private void NavigationSuccess(object sender, NavigationEventArgs e)
        {
            //sender is the frame
            //e.Content has the page being navigated to
            //How do we inform the page of success if they are interested
            //e.NavigationMode.
        }

        public event NavigatedEventHandler Navigated;

        public event NavigationFailedEventHandler NavigationFailed;

        /// <summary>
        /// Register the application pages
        /// </summary>
        public static void RegisterPages()
        {
            var navigationService = InjectorService.Default.GetInstance<INavigationService>();
            navigationService.RegisterPage(RefOrganiser.Pages.MainPage);
            navigationService.RegisterPage(RefOrganiser.Pages.ShowImagePage);
        }
    }

    

    public interface INavigationService
    {
        bool NavigateToFirst();
        bool NavigateToFirst(object parameter);
        bool NavigateToFirst(object parameter, NavigationTransitionInfo transition);
        bool NavigateTo(Type pageType);
        bool NavigateTo(Type pageType, object parameter);
        bool NavigateTo(Type pageType, object parameter, NavigationTransitionInfo transition);
        string GetKeyForPage(Type pageType);

        bool CanGoForward { get; }
        bool CanGoBack { get; }
        void GoBack();
        Type DefaultPage { get; }
        void GoForward();
        void RegisterPage(Type pageType);
        event NavigatedEventHandler Navigated;
        event NavigationFailedEventHandler NavigationFailed;
    }
}
