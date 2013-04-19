﻿using Nancy;
using geeks_nancy.services;

namespace geeks_nancy.Modules
{
    public abstract class RavenNancyModule : NancyModule
    {
        protected RavenNancyModule(string baseRoute) : base(baseRoute)
        {
            Before += ctx => null;
            After += ctx => { };
        }

        protected RavenNancyModule() : this(null)
        {
            
        }
    }

    public class EventModule : RavenNancyModule
    {
        public EventModule() : base("/events")
        {
            Get["/"] = parameters => "the list of events";
            Get["/{id}"] = parameters => "a specific event " + parameters.id;
        }
    }

    public class HomeModule : RavenNancyModule
    {
        private readonly IEmailer _emailer;

        public HomeModule(IEmailer emailer)
        {
            _emailer = emailer;
            Get["/"] = parameters => "home page";
            Get["/about"] = parameters => _emailer.SayHello() + " from about page";
        }
    }
}