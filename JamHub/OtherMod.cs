using UnityEngine;

namespace JamHub
{
    public class OtherMod
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string Author { get; private set; }
        public GameObject Planet { get; private set; }

        /**
         * Make a new OtherMod with the given name and author
         */
        public OtherMod(string id, string name, string author, GameObject planet)
        {
            ID = id;
            Name = name;
            Author = author;
            Planet = planet;
        }
    }
}
