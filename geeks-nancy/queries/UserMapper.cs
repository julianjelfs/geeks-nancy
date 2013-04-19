using System;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using Raven.Client;
using geeks_nancy.models;

namespace geeks_nancy.queries
{
    public class UserMapper : IUserMapper
    {
        private readonly IDocumentSession _session;

        public UserMapper(IDocumentSession session)
        {
            _session = session;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return _session.Load<User>(identifier.ToString());
        }
    }
}