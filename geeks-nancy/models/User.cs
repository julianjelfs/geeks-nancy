using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FlexProviders.Membership;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace geeks_nancy.models
{
    public class User : IFlexMembershipUser, IUserIdentity
    {
        public User()
        {
            OAuthAccounts = new Collection<FlexOAuthAccount>();
        }

        public string Id { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime PasswordResetTokenExpiration { get; set; }
        public virtual ICollection<FlexOAuthAccount> OAuthAccounts { get; set; }

        public override string ToString()
        {
            return UserName;
        }

        public string UserName { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}