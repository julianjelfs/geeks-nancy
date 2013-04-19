using System;
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

        public LoginModule(IFlexMembershipProvider membership,
            IDocumentSession session)
            : base(session)
        {
            _membership = membership;
            Get["/login"] = p => View["login", new LoginModel
                {
                    ReturnUrl = Request.Query.returnUrl
                }];

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
    }
}