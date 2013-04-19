using Raven.Client;

namespace geeks_nancy.Modules
{
    public class LoginModule : RavenNancyModule
    {
        public LoginModule(IDocumentSession session)  : base(session)
        {
            Get["/login"] = x =>
            {
                return View["login"];
            };
        }   
    }
}