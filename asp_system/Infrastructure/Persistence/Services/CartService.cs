using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Model;
using Firebase.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
	public class CartService : ICartService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly UserManager<ApplicationUser> _userManager;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
			_userManager = userManager;
			_unitOfWork = unitOfWork;
        }
		public async Task<List<CartDTO>> GetCart(string userId)
		{
			//var cart = await _unitOfWork.Repository<Cart>().GetByConditionAsync(_ => _.UserId.Equals(userId));
			var cart = await _unitOfWork.Repository<Cart>().GetQueryable()
				.Where(_ => _.UserId.Equals(userId))
				.Include(_ => _.Artworks).ToListAsync();
			if (cart.Count() == 0)
			{
				return new List<CartDTO>();
			}
			var cartVM = _mapper.Map<List<CartDTO>>(cart);
			foreach(var item in cartVM)
			{
				var artWork =await _unitOfWork.Repository<Artwork>().GetByIdAsync(item.ArtWorkId);
				item.Artwork = artWork;
			}
			return cartVM;
		}
		public async Task<ResponseDTO> AddToCart(CartDTO cartCM)
		{
			var userCart = await _unitOfWork.Repository<Cart>().GetQueryable()
				.Where(_ => _.UserId.Equals(cartCM.UserId)).ToListAsync();
			foreach(var item in userCart)
			{
				if(item.ArtWorkId == cartCM.ArtWorkId)
				{
                    return new ResponseDTO { Message = "Duplicate item in cart", IsSuccess = false };
                }
			}

            var cart = _mapper.Map<Cart>(cartCM);
			var result = await _unitOfWork.Repository<Cart>().AddAsync(cart);
			if (result == null)
			{
				return new ResponseDTO { Data = "Insert item to cart failed", IsSuccess = false };
			}
			_unitOfWork.Save();
			return new ResponseDTO { Data = cart, IsSuccess = true };
		}

		public async Task<ResponseDTO> DeleteItemInCart(string id)
		{
			var itemId = Guid.Parse(id);
			var cartItem = (await _unitOfWork.Repository<Cart>().GetByConditionAsync(_ => _.Id.Equals(itemId))).FirstOrDefault();
			if(cartItem != null)
			{
				var result =  await _unitOfWork.Repository<Cart>().DeleteAsync(cartItem);
				if (result == null)
				{
					return new ResponseDTO { Data = "Insert item to cart failed", IsSuccess = false };
				}
				_unitOfWork.Save();
				return new ResponseDTO { Data = result, Message = "Remove item successfully", IsSuccess = true };
			} else
			{
				return new ResponseDTO { Message = "Cannot found that item", IsSuccess = false };
			}
		}



		public Task<CatalogyDTO> GetCatalogyById(string id)
		{
			throw new NotImplementedException();
		}

		public Task<ResponseDTO> UpdateCatalogy(string id, CartDTO catalogy)
		{
			throw new NotImplementedException();
		}

	}
}
