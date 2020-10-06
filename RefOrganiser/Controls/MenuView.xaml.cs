using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using RefOrganiser.Models;
using RefOrganiser.Services;
using RefOrganiser.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RefOrganiser.Controls
{
    public sealed partial class MenuView : UserControl, INotifyPropertyChanged
    {
        private ImageSourceManager _sourceManager;

        public MenuView()
        {
            this.InitializeComponent();
            SourceManager = InjectorService.Default.GetInstance<ImageSourceManager>();
            
        }

        public ImageSourceManager SourceManager
        {
            get => _sourceManager;
            set
            {
                _sourceManager = value;
                OnPropertyChanged();
            }
        }

        public async void ControlLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
            //OnPropertyChanged($"ImageSources");
            await Task.CompletedTask;
        }

        

        
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
