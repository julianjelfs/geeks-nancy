using System;
using System.Collections.Generic;

namespace FlexProviders.Membership
{
    public interface IFlexMembershipUser
    {
        string Id { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string Salt { get; set; }
        string PasswordResetToken { get; set; }
        DateTime PasswordResetTokenExpiration { get; set; }
        ICollection<FlexOAuthAccount> OAuthAccounts { get; set; }       
    }
}