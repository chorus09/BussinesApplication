using BussinesApplication.Models;

namespace BussinesApplication.Utils.Observer; 
public interface IClientObserver {
    void Update(Client client, string propertyName);
}
