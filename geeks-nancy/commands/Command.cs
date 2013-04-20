using Raven.Client;
using geeks_nancy.queries;

namespace geeks_nancy.commands
{
    public abstract class Command
    {
        public IDocumentSession Session { get; set; }
        public string CurrentUserId { get; set; }

        public abstract void Execute();
        protected TResult Query<TResult>(Query<TResult> query)
        {
            query.Session = Session;
            return query.Execute();
        }
    }
}