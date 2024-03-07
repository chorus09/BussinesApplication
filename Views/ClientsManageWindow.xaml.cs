using BussinesApplication.ViewModels;
using System.Windows;

namespace BussinesApplication.Views;
public partial class ClientsManageWindow : Window {
    private ClientsManageWindowViewModel _viewModel;
    public ClientsManageWindow() {
        InitializeComponent();
        _viewModel = new();
        _viewModel.ErrorOccurred += (sender, e) => MessageBox.Show(e);
        DataContext = _viewModel;
    }

    private async void ClientConstructorWindow_OnClick(object sender, RoutedEventArgs e) {
        await Application.Current.Dispatcher.InvokeAsync(() => {
            var window = new ClientConstructorWindow();
            window.Show();
        });
    }

    private async void ProductsManageWindow_OnClick(object sender, RoutedEventArgs e) {
        await Application.Current.Dispatcher.InvokeAsync(() => {
            var window = new ProductsManageWindow();
            window.Show();
        });
    }
}