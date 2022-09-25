using Caliburn.Micro;
using RMDesktopUI.Library.API;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private BindingList<ProductModel> _products;
		private IProductEndPoint _productEndPoint;
		private IConfigHelper _configHelper;
		private ISaleEndPoint _saleEndPoint;

		public SalesViewModel(IProductEndPoint productEndPoint, IConfigHelper configHelper ,ISaleEndPoint saleEndPoint)
		{
			_productEndPoint = productEndPoint;
			_saleEndPoint = saleEndPoint;
			_configHelper = configHelper;
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

		private ProductModel _selectedProduct;

		public ProductModel SelectedProduct
		{
			get { return _selectedProduct; }
			set { _selectedProduct = value; NotifyOfPropertyChange(() => SelectedProduct); NotifyOfPropertyChange(() => CanAddToCart); }
		}

		private int _itemQuantity = 1;

		public int ItemQuantity
		{
			get { return _itemQuantity; }
			set { _itemQuantity = value; NotifyOfPropertyChange(() => ItemQuantity); NotifyOfPropertyChange(() => CanAddToCart); }
		}
		private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

		public BindingList<CartItemModel> Cart
		{
			get { return _cart; }
			set { _cart = value; NotifyOfPropertyChange(() => Cart); }
		}

		public string SubTotal
		{
			get
			{
				return CalculateSubTotal().ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("en-us"));
			}
		}
		private decimal CalculateSubTotal()
		{
			decimal subTotal = 0;

			foreach (var item in Cart)
			{
				subTotal += (item.Product.RetailPrice * item.QuantityInCart);
			}

			return subTotal;

		}
		public string Tax
		{
			get
			{
				return CalculateTax().ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-us"));
			}
		}
		private decimal CalculateTax()
		{

			decimal taxAmount = 0;
			decimal taxRate = _configHelper.GetTaxRate() / 100;

			taxAmount = Cart.Where(x => x.Product.IsTaxable).Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

			//foreach (var item in Cart)
			//{
			//	if (item.Product.IsTaxable)
			//	{
			//		taxAmount += (item.Product.RetailPrice * item.QuantityInCart * taxRate);

			//	}
			//}
			return taxAmount;
		}
		public string Total
		{
			get
			{
				decimal total = CalculateSubTotal() + CalculateTax();
				return total.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-us"));
			}
		}


		public bool CanCheckOut => Cart.Count > 0 ? true : false;
		public async Task CheckOut()
		{
			//Create sale model and post to the API
			SaleModel sale = new SaleModel();
			sale.SaleDetails = new List<SaleDetailModel>();

			foreach (var item in Cart)
			{
				sale.SaleDetails.Add(new SaleDetailModel
				{
					ProductId = item.Product.Id,
					Quantity = item.QuantityInCart

				});
			}
			await _saleEndPoint.PostSale(sale);

		}
		public bool CanAddToCart => (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity ? true : false);

		public void AddToCart()
		{

			CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

			if (existingItem != null)
			{
				existingItem.QuantityInCart += ItemQuantity;
				SelectedProduct.QuantityInStock -= ItemQuantity;
				// HACK - should be better solution
				Cart.Remove(existingItem);
				Cart.Add(existingItem);
			}
			else
			{

				CartItemModel item = new CartItemModel
				{
					Product = SelectedProduct,
					QuantityInCart = ItemQuantity

				};

				Cart.Add(item);
			}

			SelectedProduct.QuantityInStock -= ItemQuantity;
			ItemQuantity = 1;

			NotifyOfPropertyChange(() => CanCheckOut);
			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
		}
		public bool CanRemoveFromCart()
		{
			// TODO - Remove from cart can method
			return false;
		}
		public void RemoveFromCart()
		{

			NotifyOfPropertyChange(() => CanCheckOut);
			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
		}




	}
}
