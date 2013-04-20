using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using geeks_nancy.models;

namespace geeks_nancy.queries
{
    public class EventsDataForUser : ListQuery<Event>
    {
        public string Search { get; set; }
        public Person CurrentPerson { get; set; }

        public override ListResult<Event> Execute()
        {
            var query = Session.Query<Event, Event_ByDescription>()
                                            .Include<Event>(e => e.Invitations)
                                          .Where(e => e.CreatedBy == CurrentUserId
                                                || e.Invitations.Any(p => p.PersonId == CurrentPerson.Id)
                                          );
            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Search(e => e.Description, Search)
                             .Search(e => e.Venue, Search);
            }
            return PageFrom(query.ToList());
        }
    }
}