using System;
using System.Collections.Generic;
using System.Configuration;
using FlexProviders.Membership;
using FlexProviders.Raven;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Conventions;
using Nancy.Cryptography;
using Nancy.Security;
using Nancy.TinyIoc;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using geeks_nancy.models;

namespace geeks_nancy
{
    public class CustomConventionsBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            
            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("views/", viewName));

            Conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("assets", @"assets")
            );

            container.Register<IFlexUserStore, FlexMembershipUserStore<User, Role>>();
            container.Register(typeof(IDocumentStore), InitDocStore());
            container.Register(typeof(IDocumentSession), (c, overloads) =>
                c.Resolve<IDocumentStore>().OpenSession());

            var cryptographyConfiguration = new CryptographyConfiguration(
                new RijndaelEncryptionProvider(new PassphraseKeyGenerator(Configuration.EncryptionKey, new byte[] { 8, 2, 10, 4, 68, 120, 7, 14 })),
                new DefaultHmacProvider(new PassphraseKeyGenerator(Configuration.HmacKey, new byte[] { 1, 20, 73, 49, 25, 106, 78, 86 })));

            var authenticationConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    CryptographyConfiguration = cryptographyConfiguration,
                    RedirectUrl = "/login",
                    UserMapper = container.Resolve<IUserMapper>(),
                };

            FormsAuthentication.Enable(pipelines, authenticationConfiguration);
        }

        private IDocumentStore InitDocStore()
        {
            var store = new DocumentStore
            {
                ConnectionStringName = "LocalRavenDB"
            };
            store.Initialize();
            //IndexCreation.CreateIndexes(typeof(Event_ByDescription).Assembly, store);
            return store;
        }
    }
}