using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace geeks_nancy.services
{
    public interface IEmailer
    {
        string SayHello();
    }

    public class Emailer : IEmailer
    {
        public string SayHello()
        {
            return "Hello";
        }
    }
}