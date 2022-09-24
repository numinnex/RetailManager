using Caliburn.Micro;
using RMDesktopUI.Library.API;
using RMDesktopUI.Library.Models;
using System.ComponentModel;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private BindingList<ProductModel> _products;
		private IProductEndPoint _productEndPoint;

		public SalesViewModel(IProductEndPoint productEndPoint)
		{
			_productEndPoint = productEndPoint;

		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			await LoadProducts();
		}
		private async Task LoadProducts()
		{
			var productList = await _productEndPoint.GetAll();
			Products = new BindingList<ProductModel>(productList);
		}

		public BindingList<ProductModel> Products
		{
			get { return _products; }
			set
			{
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}
		private int _itemQuantity;

		public int ItemQuantity
		{
			get { return _itemQuantity; }
			set { _itemQuantity = value; NotifyOfPropertyChange(() => ItemQuantity); }
		}
		private BindingList<ProductModel> _cart;

		public BindingList<ProductModel> Cart
		{
			get { return _cart; }
			set { _cart = value; NotifyOfPropertyChange(() => Cart); }
		}

		public string SubTotal
		{
			get
			{
				// TODO - Replace with calculations
				return "$0.00";
			}
		}
		public string Tax
		{
			get
			{
				// TODO - Replace with calculations
				return "$0.00";
			}
		}
		public string Total
		{
			get
			{
				// TODO - Replace with calculations
				return "$0.00";
			}
		}


		public bool CanCheckOut()
		{
			//TODO - Make sure something is on the cart
			return false;
		}
		public void CheckOut()
		{

		}
		public bool CanAddToCart()
		{
			// TODO - Make sure something is selected, and there is an item quantity
			return false;
		}

		public void AddToCart()
		{

		}
		public bool CanRemoveFromCart()
		{
			// TODO - Remove from cart can method
			return false;
		}
		public void RemoveFromCart()
		{

		}




	}
}
