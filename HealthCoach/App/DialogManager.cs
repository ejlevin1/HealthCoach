using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCoach.App
{
    public class DialogManager
    {
        private static DialogManager _Instance = new DialogManager();

        public static DialogManager Instance
        {
            get { return _Instance; }
        }

        private Dictionary<string, Dictionary<string, string>> _manager = new Dictionary<string, Dictionary<string, string>>();

        public void SetCurrentConversationContext(string provider, string user, Dictionary<string,string> metadata)
        {
            if (HasConversationContext(provider, user))
                throw new Exception("The provided key is already in a conversation");

            _manager.Add(GetKey(provider, user), metadata);
        }

        public bool HasConversationContext(string provider, string user)
        {
            return _manager.ContainsKey(GetKey(provider, user));
        }

        public void HandleMessage(string provider, string user, string text, Action<string> callback, string photoUrl = null)
        {
            if(HasConversationContext(provider, user))
            {
                var context = GetConversationContext(provider, user);
                if (context["type"] == "photo")
                {
                    if (text == "stool" || text == "meal")
                    {
                        callback("Thanks! I've logged that photo for you.");
                        _manager.Remove(GetKey(provider, user));
                    }
                    else
                    {
                        callback("I dont' know how to handle that response.  What type of photo is that? (stool/meal)");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(photoUrl))
                {
                    SetCurrentConversationContext(provider, user, new Dictionary<string, string>
                        {
                            { "type", "photo" },
                            { "image", photoUrl }
                        });
                    callback("Is that a photo of a meal or stool?");
                }
            }
        }

        public Dictionary<string,string> GetConversationContext(string provider, string user)
        {
            return _manager[GetKey(provider, user)];
        }

        private string GetKey(string provider, string user)
        {
            return string.Format("{0}-{1}", provider, user);
        }
    }
}
