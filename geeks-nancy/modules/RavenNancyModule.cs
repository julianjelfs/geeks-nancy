using Nancy;
using Raven.Client;

namespace geeks_nancy.Modules
{
    public abstract class RavenNancyModule : NancyModule
    {
        protected IDocumentSession Session { get; set; }
        protected RavenNancyModule(IDocumentSession session, string baseRoute) : base(baseRoute)
        {
            Session = session;
            Before += ctx => null;
            After += ctx => { };
        }

        protected RavenNancyModule(IDocumentSession session) : this(session, null)
        {
            
        }
    }
}