using VLSMistique.ViewModels;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Storage;

namespace VLSMistique
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }
    }
}
