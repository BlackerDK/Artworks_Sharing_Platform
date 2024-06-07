using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }
        public Task<ResponseDTO> CreateOrder(OrderCreateDTO order) 
        {
            try
            {
                var newOrder = new Order
                {
                    Date = DateTime.Now,
                    Code = order.Code,
                    ReOrderStatus = order.ReOrderStatus,
                    ArtworkId = order.ArtworkId,
                    UserId = order.UserId
				};
                _unitOfWork.Repository<Order>().AddAsync(newOrder);
                _unitOfWork.Save();
                return Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Order added successfully", Data = order });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }

        }
        public async Task<OrderDTO> GetOrder(int id)
        {
            var Order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
            OrderDTO orderDTO = _mapper.Map<OrderDTO>(Order);
           
            orderDTO.UserId = await _unitOfWork.Repository<Order>().GetQueryable().Where(a => a.OrderId == id).Select(a => a.User.Id).FirstOrDefaultAsync();

            return orderDTO;
        }

        public async Task<ResponseDTO> UpdateOrder(OrderUpdateDTO order)
        {
            try
            {
                var existingOrder = _unitOfWork.Repository<Order>().GetQueryable().FirstOrDefault(a => a.OrderId == order.OrderId);
                if (existingOrder == null)
                {
                    return (new ResponseDTO { IsSuccess = false, Message = "Order not found" });
                }
                existingOrder = submitCourseChange(existingOrder, order);
                await _unitOfWork.Repository<Order>().UpdateAsync(existingOrder);
                _unitOfWork.Save();
                return (new ResponseDTO { IsSuccess = true, Message = "Order updated successfully", Data = order });
            }
            catch (Exception ex)
            {
                return (new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }
        private Order submitCourseChange(Order existingOrder, OrderUpdateDTO order)
        {
            existingOrder.Code = order.Code;
            existingOrder.ReOrderStatus = order.ReOrderStatus;
            
            return existingOrder;
        }

        public async Task<ResponseDTO> DeleteOrder(OrderDeleteDTO order)
        {
            try
            {
                var existingOrder = _unitOfWork.Repository<Order>().GetQueryable().FirstOrDefault(a => a.OrderId == order.OrderId);
                if (existingOrder == null)
                {
                    return (new ResponseDTO { IsSuccess = false, Message = "Order not found" });
                }
                existingOrder = submitCourse(existingOrder, order);
                await _unitOfWork.Repository<Order>().DeleteAsync(existingOrder);
                _unitOfWork.Save();
                return (new ResponseDTO { IsSuccess = true, Message = "Order deleted successfully", Data = order });
            }
            catch (Exception ex)
            {
                return (new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }

        private Order submitCourse(Order existingOrder, OrderDeleteDTO order)
        {
            existingOrder.OrderId = order.OrderId;

            return existingOrder;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrder()
        {
            var OrderList = await _unitOfWork.Repository<Order>().GetAllAsync();
            var OrderDTOList = _mapper.Map<List<OrderDTO>>(OrderList);
            foreach (var order in OrderDTOList)
            {
                
                order.UserId = await _unitOfWork.Repository<Order>().GetQueryable().Where(a => a.OrderId == order.OrderId).Select(a => a.User.Id).FirstOrDefaultAsync();
            }
            return OrderDTOList;
        }

		public async Task<IEnumerable<OrderDTO>> GetOrderByUser(string userId)
		{
            var OrderList = _unitOfWork.Repository<Order>().GetQueryable().Where(u => u.User.Id.Equals(userId)).ToList();
			var OrderDTOList = _mapper.Map<List<OrderDTO>>(OrderList);
			return OrderDTOList;
		}
	}
}
