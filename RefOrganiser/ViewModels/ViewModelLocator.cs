using System;
using RefOrganiser.Models;
using RefOrganiser.Services;
using RefOrganiser.Views;
using SimpleInjector;

namespace RefOrganiser.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            //container.Register<>();
            MainViewModel = InjectorService.Default.GetInstance<MainViewModel>();
            ReferenceViewModel = InjectorService.Default.GetInstance<ReferenceViewModel>();
        }

        public MainViewModel MainViewModel { get; set; }
        public ReferenceViewModel ReferenceViewModel { get; set; }
    }
}
