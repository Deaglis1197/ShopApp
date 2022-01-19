using System.Collections.Generic;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.business.Concrete
{
    public class OrderManager : IOrderService
    {
        private IUnitOfWork _unitofWork;
        public OrderManager(IUnitOfWork unitofWork){
            _unitofWork= unitofWork;
        }
        public void Create(Order entity)
        {
            _unitofWork.Orders.Create(entity);
            _unitofWork.Save();
        }

        public List<Order> GetOrders(string userId)
        {
            return _unitofWork.Orders.GetOrders(userId);
        }
    }
}