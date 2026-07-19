using Monitor.Maui.ViewModel;

namespace Monitor.Maui;

public partial class MainPage : ContentPage
{
    private readonly ProcessListViewModel _viewModel;

    public MainPage(ProcessListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Initialize();
    }
}
