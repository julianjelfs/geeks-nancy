using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using geeks_nancy.models;

namespace geeks_nancy.queries
{
    public class Event_ByDescription : AbstractIndexCreationTask<Event>
    {
        public Event_ByDescription()
        {
            Map = events => from ev in events
                            select new
                                {
                                    ev.Description,
                                    ev.Venue,
                                    ev.CreatedBy,
                                    Invitations_PersonId = from i in ev.Invitations select i.PersonId
                                };

            Index(x => x.Description, FieldIndexing.Analyzed);
            Index(x => x.Venue, FieldIndexing.Analyzed);
        }
    }
}