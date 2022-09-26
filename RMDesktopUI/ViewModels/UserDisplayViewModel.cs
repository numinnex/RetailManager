using Caliburn.Micro;
using RMDesktopUI.Library.API;
using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RMDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
		private StatusInfoViewModel _status;
		private IWindowManager _window;
		private IUserEndPoint _userEndPoint;
		private BindingList<ApplicationUserModel> _users;

		public BindingList<ApplicationUserModel> Users
		{
			get
			{
				return _users;
			}
			set
			{
				_users = value;
				NotifyOfPropertyChange(() => Users);
			}
		}

		public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IUserEndPoint userEndPoint)
		{
			_status = status;
			_userEndPoint = userEndPoint;
			_window = window;

		}
        protected override async void OnViewLoaded(object view)
        {
			base.OnViewLoaded(view);
			try
			{
				await LoadUsers();

			}
			catch (Exception ex)
			{
				dynamic settings = new ExpandoObject();
				settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				settings.ResizeMode = ResizeMode.NoResize;
				settings.Title = "System Error";

				if (ex.Message == "Unauthorized")
				{
					_status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form");
					await _window.ShowDialogAsync(_status, null, settings); 
				}
				else
				{
					_status.UpdateMessage("Fatal Exception", ex.Message);
					await _window.ShowDialogAsync(_status, null, settings); 

				}
				await TryCloseAsync();

			}
        }

		private async Task LoadUsers()
		{
			var usersList = await _userEndPoint.GetAll();
			Users = new BindingList<ApplicationUserModel>(usersList);

		}

    }
}
