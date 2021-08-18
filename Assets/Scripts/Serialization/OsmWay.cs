using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Assets.Scripts.Serialization
{
    class OsmWay : BaseOsm
    {
        public ulong ID { get; private set; }
        public bool Visible { get; private set; }
        public List<ulong> NodeIDs { get; private set; }
        public bool IsBoundary { get; private set; }
        public bool IsBuilding { get; private set; }
        public float Height { get; private set; } // height of the building in meters

        public OsmWay(XmlNode node)
        {
            NodeIDs = new List<ulong>();

            ID = GetAttribute<ulong>("id", node.Attributes);
            Visible = GetAttribute<bool>("visible", node.Attributes);

            XmlNodeList nds = node.SelectNodes("nd");
            foreach(XmlNode nd in nds)
            {
                ulong refNo = GetAttribute<ulong>("ref", nd.Attributes);
                NodeIDs.Add(refNo);
            }

            // determine what type of way this is: road / boundary
            if (NodeIDs.Count > 1)
            {
                IsBoundary = NodeIDs[0] == NodeIDs[NodeIDs.Count - 1];
            }

            // go through any tags (buildings etc)
            XmlNodeList tags = node.SelectNodes("tag");
            foreach (XmlNode tag in tags)
            {
                string key = GetAttribute<string>("k", tag.Attributes);
                switch(key)
                {
                    case "building:levels":
                        Height = 3.0f * GetAttribute<float>("v", tag.Attributes); // each story is 3 meters high
                        break;
                    case "height":
                        Height = 0.3048f * GetAttribute<float>("v", tag.Attributes); // feet to meters
                        break;
                    case "building":
                        IsBuilding = GetAttribute<string>("v", tag.Attributes) == "yes";
                        break;
                    default:
                        //Console.WriteLine("Unsupported tag: " + key);
                        break;
                }
            }



        }

    }
}
