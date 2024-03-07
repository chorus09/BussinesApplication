using BussinesApplication.Frameworks.EntityFramework;
using BussinesApplication.Models;
using BussinesApplication.Utils;
using BussinesApplication.Utils.Observer;
using BussinesApplication.Utils.Threads;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BussinesApplication.ViewModels {
    public class ClientsManageWindowViewModel : INotifyPropertyChanged {
        private ObservableCollection<Client>? _clients;
        private ClientUpdater _clientUpdater;
        private ApplicationContext _dbContext;
        private string? _email;
        private int _clientsCount;
        private string? _selectedClientEmail;

        public string? SelectedClientEmail {
            get => _selectedClientEmail;
            set {
                _selectedClientEmail = value;
                OnPropertyChanged(nameof(SelectedClientEmail));
            }
        }

        public int ClientsCount {
            get => _clientsCount;
            set {
                _clientsCount = value;
                OnPropertyChanged(nameof(ClientsCount));
            }
        }
        public string? Email {
            get => _email;
            set {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public ObservableCollection<Client>? Clients {
            get => _clients;
            set {
                if (_clients == value) return;
                _clients = value;
                OnPropertyChanged(nameof(Clients));
            }
        }

        private Client? _selectedClient;

        public Client? SelectedClient {
            get => _selectedClient;
            set {
                if (_selectedClient == value) return;
                _selectedClient = value;
                SelectedClientEmail = _selectedClient?.Email;
                OnPropertyChanged(nameof(SelectedClient));
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

        public ClientsManageWindowViewModel() {
            _dbContext = new ApplicationContext(); // Создание экземпляра ApplicationContext
            Clients = new ObservableCollection<Client>();
            _clientUpdater = new ClientUpdater(async (client, propertyName) => await UpdateClient(client, propertyName));

            _updaterThread = new DataUpdaterThread {
                Action = () => ReadClientsAsync().Wait(),
                IntervalMilliseconds = 5000
            };

            IsUpdaterThreadRunning = false;
        }

        public RelayCommand ReadClientsCommand => new RelayCommand(async execute => await ReadClientsAsync());
        public RelayCommand DeleteClientCommand => new RelayCommand(async execute => await DeleteClientAsync());
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<string>? ErrorOccurred;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task DeleteClientAsync() {
            try {
                await Task.Run(() => {
                    _dbContext.Clients.Remove(SelectedClient!);
                    _dbContext.SaveChanges();
                });
                await ReadClientsAsync();
            } catch (Exception ex) {
                ErrorOccurred?.Invoke(this, "An error occurred while deleting the client: " + ex.Message);
            }
        }

        private async Task ReadClientsAsync() {
            try {
                await Task.Run(() => {
                    using (var dbContext = new ApplicationContext()) {
                        IQueryable<Client> query = dbContext.Clients;

                        if (!string.IsNullOrEmpty(_email)) {
                            query = query.Where(c => c.Email == _email);
                        }

                        var clients = query.ToList();
                        Clients = new ObservableCollection<Client>(clients);
                        ClientsCount = dbContext.Clients.Count();
                        foreach (var client in Clients) {
                            client.Attach(_clientUpdater);
                        }
                    }
                });
            } catch (Exception ex) {
                ErrorOccurred?.Invoke(this, "An error occurred while reading clients: " + ex.Message);
            }
        }

        private async Task UpdateClient(Client client, string propertyName) {
            try {
                await Task.Run(async () => {
                    var etClient = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == client.Id);
                    if (etClient != null) {
                        var propertyInfo = typeof(Client).GetProperty(propertyName);
                        if (propertyInfo != null && propertyInfo.Name != nameof(Client.Id) && propertyInfo.Name != nameof(Client.Email)) {
                            propertyInfo.SetValue(etClient, propertyInfo.GetValue(client));
                        }

                        await _dbContext.SaveChangesAsync();
                    }
                });
            } catch (Exception ex) {
                ErrorOccurred?.Invoke(this, "An error occurred while updating the client: " + ex.Message);
            }
        }
    }
}
