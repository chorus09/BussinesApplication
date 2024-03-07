using BussinesApplication.Models;

namespace BussinesApplication.Utils.Observer; 
public interface IProductObserver {
    void Update(Product product, string propertyName);
}
