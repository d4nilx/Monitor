using Monitor.Maui.ViewModel;

namespace Monitor.Maui;

public partial class MainPage : ContentPage
{
    public MainPage(ProcessListViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel; 
    }
}