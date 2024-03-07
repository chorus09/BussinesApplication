using BussinesApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesApplication.Utils.Observer {
    internal class ProductUpdater : IProductObserver {
        private Action<Product, string> _updateDataClient;
        public ProductUpdater(Action<Product, string> action) {
            _updateDataClient = action;
        }

        public void Update(Product product, string propertyName) {
            _updateDataClient?.Invoke(product, propertyName);
        }
    }
}
