using BussinesApplication.Models;

namespace BussinesApplication.Utils.Observer; 
public class ClientUpdater : IClientObserver {
    private Action<Client, string> _updateDataClient;
    public ClientUpdater(Action<Client, string> action) {
        _updateDataClient = action;
    }

    public void Update(Client client, string propertyName) {
        _updateDataClient?.Invoke(client, propertyName);
    }
}
