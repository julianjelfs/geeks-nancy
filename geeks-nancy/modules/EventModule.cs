using Nancy.Security;
using Raven.Client;

namespace geeks_nancy.Modules
{
    public class EventModule : RavenNancyModule
    {
        public EventModule(IDocumentSession session) : base(session, "/events")
        {
            this.RequiresAuthentication();

            Get["/"] = parameters => "the list of events";
            Get["/{id}"] = parameters => "a specific event " + parameters.id;
        }
    }
}