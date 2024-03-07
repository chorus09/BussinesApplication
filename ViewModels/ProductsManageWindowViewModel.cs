using BussinesApplication.Frameworks.EntityFrameworkNpg;
using BussinesApplication.Models;
using BussinesApplication.Utils;
using BussinesApplication.Utils.Observer;
using BussinesApplication.Utils.Threads;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BussinesApplication.ViewModels {
    public class ProductsManageWindowViewModel : INotifyPropertyChanged {
        private ObservableCollection<Product>? _products;
        private ProductUpdater _productUpdater;
        private NpgApplicationContext _dbContext;
        private string? _email;
        private int _productsCount;
        private string? _selectedProductEmail;

        public string? SelectedProductEmail {
            get => _selectedProductEmail;
            set {
                _selectedProductEmail = value;
                OnPropertyChanged(nameof(SelectedProductEmail));
            }
        }

        public int ProductsCount {
            get => _productsCount;
            set {
                _productsCount = value;
                OnPropertyChanged(nameof(ProductsCount));
            }
        }

        public string? Email {
            get => _email;
            set {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public ObservableCollection<Product>? Products {
            get => _products;
            set {
                if (_products == value) return;
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        private Product? _selectedProduct;

        public Product? SelectedProduct {
            get => _selectedProduct;
            set {
                if (_selectedProduct == value) return;
                _selectedProduct = value;
                SelectedProductEmail = _selectedProduct.Email;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        private bool _isUpdaterThreadRunning;

        public bool IsUpdaterThreadRunning {
            get => _isUpdaterThreadRunning;
            set {
                if (_isUpdaterThreadRunning == value) return;
                _isUpdaterThreadRunning = value;
                OnPropertyChanged(nameof(IsUpdaterThreadRunning));

                if (_isUpdaterThreadRunning)
                    _updaterThread?.Start();
                else
                    _updaterThread?.Stop();
            }
        }

        private DataUpdaterThread _updaterThread;

        public ProductsManageWindowViewModel() {
            _dbContext = new NpgApplicationContext();
            Products = new ObservableCollection<Product>();
            _productUpdater = new ProductUpdater(async (product, propertyName) => await UpdateProduct(product, propertyName));

            _updaterThread = new DataUpdaterThread {
                Action = () => ReadProductsAsync().Wait(),
                IntervalMilliseconds = 5000
            };

            IsUpdaterThreadRunning = false;
        }

        public RelayCommand ReadProductsCommand => new RelayCommand(async execute => await ReadProductsAsync());
        public RelayCommand DeleteProductCommand => new RelayCommand(async execute => await DeleteProductAsync());
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<string>? ErrorOccurred;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task DeleteProductAsync() {
            try {
                await Task.Run(() => {
                    _dbContext.Products.Remove(SelectedProduct!);
                    _dbContext.SaveChanges();
                });
                await ReadProductsAsync();
            } catch (Exception ex) {
                ErrorOccurred?.Invoke(this, "An error occurred while deleting the product: " + ex.Message);
            }
        }

        private async Task ReadProductsAsync() {
            try {
                await Task.Run(() => {
                    using (var dbContext = new NpgApplicationContext()) {
                        IQueryable<Product> query = dbContext.Products;

                        if (!string.IsNullOrEmpty(_email)) {
                            query = query.Where(p => p.Email == _email);
                        }

                        var products = query.ToList();
                        Products = new ObservableCollection<Product>(products);
                        ProductsCount = dbContext.Products.Count();
                        foreach (var product in Products) {
                            product.Attach(_productUpdater);
                        }
                    }
                });
            } catch (Exception ex) {
                ErrorOccurred?.Invoke(this, "An error occurred while reading products: " + ex.Message);
            }
        }

        private async Task UpdateProduct(Product product, string propertyName) {
            try {
                await Task.Run(async () => {
                    var etProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
                    if (etProduct != null) {
                        var propertyInfo = typeof(Product).GetProperty(propertyName);
                        if (propertyInfo != null && propertyInfo.Name != nameof(Product.Id) && propertyInfo.Name != nameof(Product.Email)) {
                            propertyInfo.SetValue(etProduct, propertyInfo.GetValue(product));
                        }

                        await _dbContext.SaveChangesAsync();
                    }
                });
            } catch (Exception ex) {
                ErrorOccurred?.Invoke(this, "An error occurred while updating the product: " + ex.Message);
            }
        }
    }
}
