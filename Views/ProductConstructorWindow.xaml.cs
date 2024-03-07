using BussinesApplication.ViewModels;
using System.Windows;

namespace BussinesApplication.Views {
    public partial class ProductConstructorWindow : Window {
        private ProductConstructorWindowViewModel _viewModel;

        public ProductConstructorWindow() {
            InitializeComponent();
            _viewModel = new ProductConstructorWindowViewModel();
            _viewModel.ProductInserted += (sender, message) => MessageBox.Show(message);
            DataContext = _viewModel;
        }
    }
}
