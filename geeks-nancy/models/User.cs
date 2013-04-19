using System;
using System.Collections.Generic;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace geeks_nancy.models
{
    public class UserMapper : IUserMapper
    {
        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return new User();
        }
    }

    public class User : IUserIdentity
    {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}