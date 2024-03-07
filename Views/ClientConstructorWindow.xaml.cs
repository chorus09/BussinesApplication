using BussinesApplication.ViewModels;
using System.Windows;

namespace BussinesApplication.Views;
public partial class ClientConstructorWindow : Window {
    private ClientConstructorWindowViewModel _viewModel;
    public ClientConstructorWindow() {
        InitializeComponent();
        _viewModel = new();
        _viewModel.ClientInserted += (sender, message) => MessageBox.Show(message);
        DataContext = _viewModel;
    }
}
