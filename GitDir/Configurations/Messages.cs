using System.Linq;

namespace GitDir.Data
{
    public class Messages
    {
        public Message[] Available { get; set; }

        public Message GetMessage(string Identifier)
        {
            return Available?.SingleOrDefault(m => m.Identifier == Identifier);
        }
    }
}
