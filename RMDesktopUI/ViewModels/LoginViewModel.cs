using Caliburn.Micro;
using RMDesktopUI.Helpers;
using System;
using System.Threading.Tasks;
using RMDesktopUI.Library.API;
using System.Xml.XPath;

namespace RMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _username;
        private string _password;
        private string _errorMessage;
        private IAPIHelper _apiHelper;

        public LoginViewModel(IAPIHelper apihelper)
        {
            _apiHelper = apihelper;
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

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

            }
        }

    }
}
