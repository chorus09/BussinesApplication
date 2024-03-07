using BussinesApplication.Utils;
using System.ComponentModel;
using BussinesApplication.Frameworks.EntityFramework;
using BussinesApplication.Models;

namespace BussinesApplication.ViewModels; 
public class ClientConstructorWindowViewModel : INotifyPropertyChanged {
    private string _firstName;
    private string _lastName;
    private string _middleName;
    private string _email;
    private string _phone;
    private readonly ApplicationContext _dbContext;

    public string FirstName {
        get { return _firstName; }
        set {
            _firstName = value;
            OnPropertyChanged(nameof(FirstName));
        }
    }

    public string LastName {
        get { return _lastName; }
        set {
            _lastName = value;
            OnPropertyChanged(nameof(LastName));
        }
    }

    public string MiddleName {
        get { return _middleName; }
        set {
            _middleName = value;
            OnPropertyChanged(nameof(MiddleName));
        }
    }

    public string Email {
        get { return _email; }
        set {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    public string Phone {
        get { return _phone; }
        set {
            _phone = value;
            OnPropertyChanged(nameof(Phone));
        }
    }
    public RelayCommand CreateClientCommand => new RelayCommand(async execute => await CreateClient());
    public ClientConstructorWindowViewModel() {
        _dbContext = new();
    }
    private async Task CreateClient() {
        await Task.Run(() => {
            try {
                var client = new Client(_firstName, _lastName, _middleName, _email, _phone);
                _dbContext.Clients.AddAsync(client);
                ClientInserted?.Invoke(this, $"created new client with email: {_email}");
                _dbContext.SaveChangesAsync();
            } catch (Exception ex) {
                ClientInserted?.Invoke(this, "error! : " + ex.Message);
            }
        });
    }
    public event EventHandler<string>? ClientInserted;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
