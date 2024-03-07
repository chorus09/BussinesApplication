using BussinesApplication.Frameworks.EntityFrameworkNpg;
using BussinesApplication.Models;
using BussinesApplication.Utils;
using System.ComponentModel;

namespace BussinesApplication.ViewModels;
public class ProductConstructorWindowViewModel : INotifyPropertyChanged {
    private string _code;
    private string _name;
    private string _email;
    private readonly NpgApplicationContext _dbContext;

    public string Code {
        get { return _code; }
        set {
            _code = value;
            OnPropertyChanged(nameof(Code));
        }
    }

    public string Name {
        get { return _name; }
        set {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string Email {
        get { return _email; }
        set {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    public RelayCommand CreateProductCommand => new RelayCommand(async execute => await CreateProduct());

    public ProductConstructorWindowViewModel() {
        _dbContext = new NpgApplicationContext();
    }

    private async Task CreateProduct() {
        await Task.Run(() => {
            try {
                var product = new Product(_code, _name, _email);
                _dbContext.Products.AddAsync(product);
                ProductInserted?.Invoke(this, $"Created new product with name: {_name}");
                _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                ProductInserted?.Invoke(this, "Error: " + ex.Message);
            }
        });
    }

    public event EventHandler<string>? ProductInserted;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
