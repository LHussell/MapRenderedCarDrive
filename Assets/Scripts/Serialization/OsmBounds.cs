using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace Assets.Scripts.Serialization
{
    class OsmBounds : BaseOsm
    {
        // minlat="54.0889580" minlon="12.2487570" maxlat="54.0913900" maxlon="12.2524800"/>

        public float MinLat { get; private set; }
        public float MaxLat { get; private set; }
        public float MinLon { get; private set; }
        public float MaxLon { get; private set; }
        public Vector3 Centre { get; private set; }

        public OsmBounds(XmlNode node)
        {
            MinLat = GetAttribute<float>("minlat", node.Attributes);
            MaxLat = GetAttribute<float>("maxlat", node.Attributes);
            MinLon = GetAttribute<float>("minlon", node.Attributes);
            MaxLon = GetAttribute<float>("maxlon", node.Attributes);

            float xCentre = (float)((MercatorProjection.lonToX(MaxLon) + MercatorProjection.lonToX(MinLon)) / 2);
            float yCentre = (float)((MercatorProjection.latToY(MaxLat) + MercatorProjection.latToY(MinLat)) / 2);

            Centre = new Vector3(xCentre, 0, yCentre);
        }
    }
}
