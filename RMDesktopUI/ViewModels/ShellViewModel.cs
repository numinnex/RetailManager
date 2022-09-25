﻿using Caliburn.Micro;
using RMDesktopUI.EventModels;
using RMDesktopUI.Library.API;
using RMDesktopUI.Library.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEventModel>
    {
        private IEventAggregator _eventAggregator;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _loggedInUser;
        private IAPIHelper _apiHelper;
        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel loggedInUser, IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
            _loggedInUser = loggedInUser;

            _salesVM = salesVM;
            _eventAggregator = events;

            _eventAggregator.SubscribeOnPublishedThread(this);

            //Activate Login Screen
            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }
        public void ExitApplication()
        {
            TryCloseAsync();
        }
        public void LogOut()
        {
            _loggedInUser.LogOffUser();

            _apiHelper.LogOffUser();
            ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
        public bool IsLoggedIn
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_loggedInUser.Token))
                    return true;
                else
                    return false;
            }


        }

        public async Task HandleAsync(LogOnEventModel message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
