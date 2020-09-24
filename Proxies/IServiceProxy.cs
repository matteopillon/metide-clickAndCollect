using ClickAndCollect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickAndCollect.Proxies
{
    public interface IServiceProxy
    {

        ExternalLogin LoginUser(string username, string password);

        ExternalUser GetUser(string username);
    }
}
