using System.Linq;
using Nancy.Security;
using Raven.Client;
using Nancy;
using geeks_nancy.models;
using geeks_nancy.queries;

namespace geeks_nancy.Modules
{
    public class EventModule : RavenNancyModule
    {
        public EventModule(IDocumentSession session) : base(session, "/events")
        {
            this.RequiresAuthentication();

            Get["/"] = parameters => View["events"];
            
            Get["/list"] = parameters =>
                {
                    var person = Query(new PersonByUserId { UserId = GetCurrentUserId() });
                    var result = Query(new EventsDataForUser
                    {
                        Search = Request.Query.search,
                        PageIndex = Request.Query.pageIndex,
                        PageSize = Request.Query.pageSize,
                        CurrentPerson = person
                    });

                    return Response.AsJson(new
                    {
                        Events = from ev in result.List
                                 select EventModelFromEvent(ev, person),
                        NumberOfPages = result.TotalPages,
                        SearchTerm = Request.Query.search,
                        PageIndex = Request.Query.pageIndex
                    });
                };
            
            Get["/{id}"] = parameters => "a specific event " + parameters.id;
        }

        private EventModel EventModelFromEvent(Event ev, Person currentPerson = null)
        {
            var organiser = Session.Load<User>(ev.CreatedBy);
            var myInvitation = ev.Invitations.FirstOrDefault(invitation => invitation.PersonId == currentPerson.Id);

            return new EventModel
            {
                Id = ev.Id,
                CreatedByUserName = organiser.UserName,
                CreatedBy = organiser.Id,
                ReadOnly = organiser.Id != GetCurrentUserId(),
                Date = ev.Date,
                Time = ev.Time,
                Description = ev.Description,
                Latitude = ev.Latitude,
                Longitude = ev.Longitude,
                Venue = ev.Venue,
                Score = ev.PercentageScore(),
                Zoom = ev.Zoom,
                MyResponse = myInvitation == null ? InvitationResponse.No : myInvitation.Response,
                Invitations = (from i in ev.Invitations
                               let person = Session.Load<Person>(i.PersonId)
                               let friend = GetFriendFromPerson(currentPerson, i.PersonId)
                               select new InvitationModel
                               {
                                   IsCurrentUser = person.Id == currentPerson.Id,
                                   Email = person.EmailAddress,
                                   PersonId = person.Id,
                                   Rating = friend == null ? 0 : friend.Rating,
                                   EmailSent = i.EmailSent,
                                   Response = i.Response,
                                   EventId = ev.Id
                               }).ToList()
            };
        }

        private Friend GetFriendFromPerson(Person person, string friendPersonId)
        {
            if (person == null)
                return null;
            return person.Friends.SingleOrDefault(f => f.PersonId == friendPersonId);
        }
    }
}