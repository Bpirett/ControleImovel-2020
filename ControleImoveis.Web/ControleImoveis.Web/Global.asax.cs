using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace ControleImoveis.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var coockie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (coockie != null && coockie.Value != string.Empty)
            {
                FormsAuthenticationTicket ticket;

                try
                {
                   ticket = FormsAuthentication.Decrypt(coockie.Value);
                }
                catch (Exception)
                {

                    return;
                }

                var partes = ticket.UserData.Split('|');
                var id = Convert.ToInt32(partes[0]);
                var perfis = partes[1].Split(';');


                if (Context.User != null)
                {
                    Context.User = new AplicacaoPrincipal(Context.User.Identity, perfis, id);
                }

            }
        }
    }
}
