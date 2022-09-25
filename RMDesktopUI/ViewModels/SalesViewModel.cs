using AutoMapper;
using Caliburn.Micro;
using RMDesktopUI.Library.API;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;
using RMDesktopUI.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private BindingList<ProductDisplayModel> _products;
		private IProductEndPoint _productEndPoint;
		private IConfigHelper _configHelper;
		private ISaleEndPoint _saleEndPoint;
		private IMapper _mapper;

		public SalesViewModel(IProductEndPoint productEndPoint, IConfigHelper configHelper, ISaleEndPoint saleEndPoint, IMapper mapper)
		{
			_productEndPoint = productEndPoint;
			_saleEndPoint = saleEndPoint;
			_configHelper = configHelper;
			_mapper = mapper;
		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			await LoadProducts();
		}
		private async Task LoadProducts()
		{
			var productList = await _productEndPoint.GetAll();
			var products = _mapper.Map<List<ProductDisplayModel>>(productList);
			Products = new BindingList<ProductDisplayModel>(products);
		}

		public BindingList<ProductDisplayModel> Products
		{
			get { return _products; }
			set
			{
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		private ProductDisplayModel _selectedProduct;

		public ProductDisplayModel SelectedProduct
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

		private async Task ResetSalesViewModel()
		{
			Cart = new BindingList<CartItemDisplayModel>();
			await LoadProducts();

			NotifyOfPropertyChange(() => CanCheckOut);
			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);


		}

		private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

		public BindingList<CartItemDisplayModel> Cart
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
			sale.SaleDetails = new List<ISaleDetailModel>();

			foreach (var item in Cart)
			{
				sale.SaleDetails.Add(new SaleDetailModel
				{
					ProductId = item.Product.Id,
					Quantity = item.QuantityInCart

				});
			}
			await _saleEndPoint.PostSale(sale);

			await ResetSalesViewModel();

		}
		public bool CanAddToCart => (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity ? true : false);

		public void AddToCart()
		{

			CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

			if (existingItem != null)
			{
				existingItem.QuantityInCart += ItemQuantity;
			}
			else
			{

				CartItemDisplayModel item = new CartItemDisplayModel
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

		private CartItemDisplayModel _selectedCartItem;

		public CartItemDisplayModel SelectedCartItem
		{
			get
			{
				return _selectedCartItem;

			}
			set
			{
				_selectedCartItem = value;
				NotifyOfPropertyChange(() => SelectedCartItem);
				NotifyOfPropertyChange(() => CanRemoveFromCart);

			}
		}
		//&& SelectedCartItem?.Product.QuantityInStock > 0
		public bool CanRemoveFromCart => SelectedCartItem != null ? true : false;
		public void RemoveFromCart()
		{


			SelectedCartItem.Product.QuantityInStock++;

			if (SelectedCartItem.QuantityInCart > 1)
				SelectedCartItem.QuantityInCart--;
			else
				Cart.Remove(SelectedCartItem);

			NotifyOfPropertyChange(() => CanCheckOut);
			NotifyOfPropertyChange(() => CanAddToCart);
			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
		}




	}
}
