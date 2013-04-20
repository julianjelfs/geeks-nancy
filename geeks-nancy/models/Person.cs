using System.Collections.Generic;

namespace geeks_nancy.models
{
    /// <summary>
    /// we need to be able to represent a person in the system before they 
    /// have a user account, so we need a separate class
    /// </summary>
    public class Person
    {
        public Person()
        {
            Friends = new List<Friend>();
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public List<Friend> Friends { get; set; }
    }
}