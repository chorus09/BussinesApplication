using BussinesApplication.ViewModels;
using System.Windows;

namespace BussinesApplication.Views;

public partial class ProductsManageWindow : Window {
    private ProductsManageWindowViewModel _viewModel;
    public ProductsManageWindow() {
        InitializeComponent();
        _viewModel = new();
        _viewModel.ErrorOccurred += (sender, e) => MessageBox.Show(e);
        DataContext = _viewModel;
    }

    private async void ProductConstructorWindow_OnClick(object sender, RoutedEventArgs e) {
        await Application.Current.Dispatcher.InvokeAsync(() => {
            var window = new ProductConstructorWindow();
            window.Show();
        });
    }

    private async void ClientsManageWindow_OnClick(object sender, RoutedEventArgs e) {
        await Application.Current.Dispatcher.InvokeAsync(() => {
            var window = new ClientsManageWindow();
            window.Show();
        });
    }
}
