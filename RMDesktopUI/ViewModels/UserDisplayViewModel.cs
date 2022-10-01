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
		private readonly StatusInfoViewModel _status;
		private readonly IWindowManager _window;
		private readonly IUserEndPoint _userEndPoint;
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
		private ApplicationUserModel _selectedUser;
		public ApplicationUserModel SelectedUser
        {
			get
			{
				return _selectedUser;
			}
			set
			{
				_selectedUser = value;
				SelectedUserName = value.Email;

				UserRoles.Clear();
				AvaiableRoles.Clear();
				UserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
				//TODO - Pull this out properly
				LoadRoles();

				NotifyOfPropertyChange(() => SelectedUser);
			}

		}

		private string _selectedUserName;

		public string SelectedUserName
		{
			get
			{
				return _selectedUserName;
			}
			set
			{
				_selectedUserName = value;
				NotifyOfPropertyChange(() => SelectedUserName);
				
			}
		}
		private BindingList<string> _UserRoles = new BindingList<string>();

		public BindingList<string> UserRoles
		{
			get
			{
				return _UserRoles;
			}
			set
			{
				_UserRoles = value;
				NotifyOfPropertyChange(() => UserRoles);
			}
		}
		
		private BindingList<string> _avaiableRoles = new BindingList<string>();

		public BindingList<string> AvaiableRoles
		{
			get
			{
				return _avaiableRoles;
			}
			set
			{
				_avaiableRoles = value;
				NotifyOfPropertyChange(() => AvaiableRoles);
			}
		}
		private async Task LoadRoles()
		{
			var roles = await _userEndPoint.GetAllRoles();

			AvaiableRoles.Clear();

			foreach (var role in roles)
			{
				if (!UserRoles.Contains(role.Value))
				{
					AvaiableRoles.Add(role.Value);
				}
			}
		}
		//RemoveFromRole AddSelectedRole SelectedRoleToRemove SelectedRoleToAdd

		private string _selectedUserRole;

		public string SelectedUserRole
        {
			get
			{
				return _selectedUserRole;
			}
			set
			{
				_selectedUserRole = value;
				NotifyOfPropertyChange(() => SelectedUserRole);
				NotifyOfPropertyChange(() => CanRemoveFromRole);
			}
		}
		private string _selectedAvaiableRole;
		public string SelectedAvaiableRole 
		{
			get
			{
				return _selectedAvaiableRole;
			}
			set
			{
				_selectedAvaiableRole = value;
				NotifyOfPropertyChange(() => SelectedAvaiableRole);
				NotifyOfPropertyChange(() => CanAddSelectedRole);
			}
		}
		public bool CanAddSelectedRole
		{
			get
			{
				return SelectedUser == null || SelectedAvaiableRole == null ? false : true;
			}
		}
		public bool CanRemoveFromRole
		{
			get
			{
				return SelectedUser == null || SelectedUserRole == null ? false : true;
			}
		}

		public async void AddSelectedRole()
		{
			await _userEndPoint.AddUserToRole(SelectedUser.Id, SelectedAvaiableRole);

			UserRoles.Add(SelectedAvaiableRole);
			AvaiableRoles.Remove(SelectedAvaiableRole);
		}
		public async void RemoveFromRole()
		{
			await _userEndPoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);

			AvaiableRoles.Add(SelectedUserRole);
			UserRoles.Remove(SelectedUserRole);
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
