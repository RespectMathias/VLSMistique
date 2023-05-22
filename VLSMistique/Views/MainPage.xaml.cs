using VLSMistique.ViewModels;

namespace VLSMistique.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Set the BindingContext to an instance of the ViewModel.
            BindingContext = new MainPageViewModel();
        }

    }
}
