using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JamHub
{
    public class OtherMod
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string Author { get; private set; }
        public GameObject planet = null;

        /**
         * Make a new OtherMod with the given name and author
         */
        public OtherMod(string id, string name, string author)
        {
            ID = id;
            Name = name;
            Author = author;
        }
    }
}
