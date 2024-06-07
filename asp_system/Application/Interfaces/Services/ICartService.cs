using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
	public interface ICartService
	{
			Task<List<CartDTO>> GetCart(string userId);
			Task<ResponseDTO> AddToCart(CartDTO catalogy);
			Task<CatalogyDTO> GetCatalogyById(string id);
			Task<ResponseDTO> DeleteItemInCart(string id);
			Task<ResponseDTO> UpdateCatalogy(string id, CartDTO catalogy);
	}
}
