using System;
using System.Linq;
using DotNetOpenAuth.AspNet;
using FlexProviders.Membership;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using Nancy.Responses;
using Raven.Client;
using geeks_nancy.models;
using Nancy;

namespace geeks_nancy.Modules
{
    public class LoginModule : RavenNancyModule
    {
        private readonly IFlexMembershipProvider _membership;
        private readonly IFlexOAuthProvider _oAuthProvider;
        private readonly ISecurityEncoder _encoder = new DefaultSecurityEncoder();


        public LoginModule(IFlexMembershipProvider membership,
            IFlexOAuthProvider oAuthProvider,
            IDocumentSession session)
            : base(session)
        {
            _membership = membership;
            _oAuthProvider = oAuthProvider;
            Get["/login"] = p => View["login", new LoginModel
                {
                    ReturnUrl = Request.Query.returnUrl
                }];

            Post["/externallogin"] = p =>
                {
                    var model = this.Bind<LoginModel>();
                    _oAuthProvider.RequestOAuthAuthentication("Google", "/externalcallback?returnUrl=" + model.ReturnUrl);
                    return null;
                };

            Get["/externalfailure"] = p => View["externalfailure"];

            Get["/externalcallback"] = p =>
                {
                    var result = _oAuthProvider.VerifyOAuthAuthentication("/externalcallback");
                    if (!result.IsSuccessful)
                    {
                        return Response.AsRedirect("/externalfailure");
                    }

                    if (_oAuthProvider.OAuthLogin(result.Provider, result.ProviderUserId, persistCookie: false))
                    {
                        //Seems like OAuthLogin writes an auth cookie, but it can't be the cookie 
                        //that nancy is expecting because it doesn't work

                        //we're all good up to here but then when we try to redirect to /events or whatever
                        //we just end up at the login screen again. We need to tell nancy that we're
                        //logged in somehow. Why does this have to be so hard?
                        string retVal = Request.Query.returnUrl;
                        return Response.AsRedirect(retVal ?? "/");
                    }

                    if (Context.CurrentUser != null)
                    {
                        // If the current user is logged in add the new account
                        var user = new User
                        {
                            UserName = Context.CurrentUser.UserName,
                            Id = Guid.NewGuid().ToString()
                        };
                        _oAuthProvider.CreateOAuthAccount(result.Provider, result.ProviderUserId, user);
                        CreateOrUpdatePersonRecord(user.UserName, user);
                        return Response.AsRedirect(Request.Query.returnUrl as string);
                    }
                    else
                    {
                        // User is new, ask for their desired membership name
                        var loginData = _encoder.SerializeOAuthProviderUserId(result.Provider, result.ProviderUserId);
                        return
                            View[
                                "externalloginconfirmation",
                                new RegisterExternalLoginModel
                                    {
                                        UserName = result.UserName,
                                        ExternalLoginData = loginData,
                                        ReturnUrl = Request.Query.returnUrl
                                    }];
                    }
                };

            Post["/externalloginconfirmation"] = p =>
                {
                    string provider = null;
                    string providerUserId = null;

                    Context.Request.Session["UserId"] = null;
                    Context.Request.Session["PersonId"] = null;

                    var model = this.Bind<RegisterExternalLoginModel>();

                    if (Context.CurrentUser != null || !_encoder.TryDeserializeOAuthProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
                    {
                        return Response.AsRedirect("/manage");
                    }

                    if (!_membership.HasLocalAccount(model.UserName))
                    {
                        var user = new User
                        {
                            UserName = model.UserName,
                            Id = Guid.NewGuid().ToString()
                        };
                        _oAuthProvider.CreateOAuthAccount(provider, providerUserId, user);
                        _oAuthProvider.OAuthLogin(provider, providerUserId, persistCookie: false);
                        CreateOrUpdatePersonRecord(user.UserName, user);
                        return Response.AsRedirect(Request.Query.returnUrl as string);
                    }
                    else
                    {
                        //not sure how to do model validation yet
                        //ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }

                    return View["/externalloginconfirmation", model];
                };

            Post["/login"] = p =>
                {
                    var model = this.Bind<LoginModel>();
                    var id = _membership.ValidateUser(model.UserName, model.Password);
                    if (id == null)
                    {
                        return Response.AsRedirect("/");
                    }

                    DateTime? expiry = null;

                    return this.LoginAndRedirect(id.Value, expiry, model.ReturnUrl);
                };

            Get["/logout"] = x => this.LogoutAndRedirect("~/");
        }

        private void CreateOrUpdatePersonRecord(string name, User user)
        {
            var person =
                Session.Query<Person>()
                            .FirstOrDefault(
                                p => p.EmailAddress.Equals(user.UserName, StringComparison.InvariantCultureIgnoreCase))
                ?? new Person
                {
                    Id = Guid.NewGuid().ToString(),
                    EmailAddress = user.UserName.ToLower(),
                    Name = name
                };

            person.UserId = user.Id;
            Session.Store(person);
        }
    }
}