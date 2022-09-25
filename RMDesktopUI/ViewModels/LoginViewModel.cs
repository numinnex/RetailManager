using Caliburn.Micro;
using RMDesktopUI.Helpers;
using System;
using System.Threading.Tasks;
using RMDesktopUI.Library.API;
using System.Xml.XPath;
using RMDesktopUI.EventModels;

namespace RMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _username = "admin@admin.com";
        private string _password = "Pwd12345";
        private string _errorMessage;
        private IAPIHelper _apiHelper;
        private IEventAggregator _eventAggregator;

        public LoginViewModel(IAPIHelper apihelper, IEventAggregator events)
        {
            _apiHelper = apihelper;
            _eventAggregator = events;  
        }
        public string UserName
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }
        public bool IsErrorVisible
        {
            get {
                bool output = false;

                if (ErrorMessage?.Length > 0)
                {
                    output = true;

                }
                return output;
            }


        }
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public bool CanLogin
        {
            get
            {
                if (UserName?.Length > 0 && Password?.Length > 0)
                    return true;
                else
                    return false;
            }
        }
        public async Task Login()
        {
            try
            {
                ErrorMessage = "";
                var result = await _apiHelper.Authenticate(UserName, Password);

                //Capture more information about user

                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                await _eventAggregator.PublishOnUIThreadAsync(new LogOnEventModel());
                

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

            }
        }

    }
}
