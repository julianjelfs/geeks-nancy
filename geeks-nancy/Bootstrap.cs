using Nancy;
using Nancy.Conventions;
using Nancy.TinyIoc;

namespace geeks_nancy
{
    public class CustomConventionsBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("views/", viewName));
            
            Conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("assets", @"assets")
            );
        }
    }
}