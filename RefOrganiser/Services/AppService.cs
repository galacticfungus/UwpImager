using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using RefOrganiser.Views;

namespace RefOrganiser.Services
{
    public class AppService
    {
        public INavigationService NavigationService { get; }

        public AppService(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public async Task HandleLaunch(LaunchActivatedEventArgs args)
        {
            // CoreApplication.EnablePrelaunch was introduced in Windows 10 version 1607
            bool canEnablePrelaunch = Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Configure the Titlebar to be customizable
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated || args.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
                {
                    //TODO: Load state from previously terminated application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (args.PrelaunchActivated == false)
            {
                // On Windows 10 version 1607 or later, this code signals that this app wants to participate in prelaunch
                if (canEnablePrelaunch)
                {
                    TryEnablePrelaunch();
                }

                // TODO: This is not a prelaunch activation. Perform operations which
                // assume that the user explicitly launched the app such as updating
                // the online presence of the user on a social network, updating a
                // what's new feed, etc.

                if (rootFrame.Content == null)
                {
                    //TODO: Restore the navigation stack but I probably do this earlier or HandleLeaveBackground

                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    NavigationService.NavigateToFirst(args.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }


        }

        /// <summary>
        /// Encapsulates the call to CoreApplication.EnablePrelaunch() so that the JIT
        /// won't encounter that call (and prevent the app from running when it doesn't
        /// find it), unless this method gets called. This method should only
        /// be called when the caller determines that we are running on a system that
        /// supports CoreApplication.EnablePrelaunch().
        /// </summary>
        private void TryEnablePrelaunch()
        {
            Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
        }

        private void OnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            throw new NotImplementedException();
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public async void HandleLeaveBackground(object sender, LeavingBackgroundEventArgs e)
        {
            //LeavingBackground is the best place to verify that your UI is ready

            //LeavingBackground is the time to ensure the first frame of UI is ready.
            //Then, long-running storage or network calls should be handled asynchronously
            //so that the event handler may return.


            //TODO: Start loading the image sources here but return immediately
            //throw new NotImplementedException();
        }

        public async void HandleEnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            //if you are doing work in the background it is good practice to save your data asynchronously from your EnteredBackground

            //If you make an asynchronous call within your handler, control returns immediately from that asynchronous call.
            //That means that execution can then return from your event handler and your app will move to the next state even
            //though the asynchronous call hasn't completed yet. Use the GetDeferral method on the EnteredBackgroundEventArgs
            //object that is passed to your event handler to delay suspension until after you call the Complete method on the
            //returned Windows.Foundation.Deferral object

            //Prior to Windows 10, version 1607, you would put the code to save your state in the Suspending event. Now the recommendation is to
            //save your state when you enter the background state, as described above

            // TODO: This is the time to save app data in case the process is terminated
            // TODO: You should release exclusive resources and file handles so that other apps can access them while your app is suspended
            //The system doesn't notify an app when it's terminated
            //The app should use the LocalSettings storage API to save simple application data synchronously.


            //throw new NotImplementedException();
        }

        public event RangeBaseValueChangedEventHandler ThumbnailSizeChanged;
        
        private async void OnThumbnailSizeChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ThumbnailSizeChanged?.Invoke(sender, e);
            await Task.CompletedTask;
        }

        public async void ChangeThumbnailSize(object sender, RangeBaseValueChangedEventArgs e)
        {
            
            ThumbnailSize = Convert.ToInt32(e.NewValue);
            OnThumbnailSizeChanged(this, e);
            await Task.CompletedTask;
        }

        public int ThumbnailSize { get; set; }
    }
}
