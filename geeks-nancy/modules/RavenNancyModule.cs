using System;
using System.Linq;
using Nancy;
using Raven.Client;
using geeks_nancy.commands;
using geeks_nancy.models;
using geeks_nancy.queries;

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

        protected void Command(Command command)
        {
            command.CurrentUserId = GetCurrentUserId();
            command.Session = Session;
            command.Execute();
        }

        protected T Query<T>(Query<T> query)
        {
            query.CurrentUserId = GetCurrentUserId();
            query.Session = Session;
            return query.Execute();
        }

        protected string GetCurrentUserId()
        {
            if (Context.Request.Session["UserId"] == null)
            {
                var user = Session.Query<User>()
                                       .SingleOrDefault(u => u.UserName == Context.CurrentUser.UserName);

                if (user == null)
                {
                    throw new ApplicationException(string.Format("Unknown user {0}", Context.CurrentUser.UserName));
                }
                Context.Request.Session["UserId"] = user.Id;
            }
            return Context.Request.Session["UserId"] as string;
        }

        protected string GetCurrentPersonId()
        {
            if (Context.Request.Session["PersonId"] == null)
            {
                var person = Query(new PersonByUserId { UserId = GetCurrentUserId() });
                if (person == null)
                {
                    throw new ApplicationException(string.Format("Unknown person {0}", Context.CurrentUser.UserName));
                }
                Context.Request.Session["PersonId"] = person.Id;
            }
            return Context.Request.Session["PersonId"] as string;
        }
    }
}