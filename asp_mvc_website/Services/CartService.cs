using asp_mvc_website.Models;

namespace asp_mvc_website.Services
{
	public class CartService
	{
		public List<CartItemModel> cartItems { get; set; }
		public CartService(List<CartItemModel> cartItems) {
			this.cartItems = cartItems;
		}

		public double CalculateTotalPrice()
		{
			double totalPrice = 0;
			foreach (var item in cartItems)
			{
				totalPrice += item.Price;
			}
			return totalPrice;
		}

		public bool IsDuplicateArtworkId(List<CartItemModel> cartItems, int artworkId)
		{
            var existingItem = cartItems.FirstOrDefault(item => item.artworkId == artworkId);
			if (existingItem != null)
			{
				return true;
			}
			return false;
        }
	}
}
