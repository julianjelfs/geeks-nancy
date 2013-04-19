using Raven.Client;
using geeks_nancy.services;

namespace geeks_nancy.Modules
{
    public class HomeModule : RavenNancyModule
    {

        public HomeModule(IDocumentSession session) : base(session)
        {
            Get["/"] = parameters => View["index.sshtml"];
            Get["/about"] = parameters => "hello from about page";
        }
    }
}