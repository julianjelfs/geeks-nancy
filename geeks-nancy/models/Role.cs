using System.Collections.Generic;
using System.Collections.ObjectModel;
using FlexProviders.Roles;

namespace geeks_nancy.models
{
    public class Role : IFlexRole<string>
    {
        public Role()
        {
            Users = new Collection<string>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<string> Users { get; set; }
    }
}