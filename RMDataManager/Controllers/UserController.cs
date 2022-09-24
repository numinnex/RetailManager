using Microsoft.AspNet.Identity;
using RMDataManger.Library.DataAccess;
using RMDataManger.Library.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;

namespace RMDataManager.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {

        public UserModel GetById()
        {
            string id = RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData();

            return data.GetUserById(id).First();

        }

    }

}

