using Assets.Scripts.Serialization;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

class MapReader : MonoBehaviour
{
    [HideInInspector]
    internal Dictionary<ulong, OsmNode> nodes;

    [HideInInspector]
    internal List<OsmWay> ways;

    [HideInInspector]
    internal OsmBounds bounds;

    [Tooltip("The resource file that contains the OSM map data")]
    public string resourceFile;

    public bool IsReady { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        nodes = new Dictionary<ulong, OsmNode>();
        ways = new List<OsmWay>();

        var txtAsset = Resources.Load<TextAsset>(resourceFile);

        // parse xml map data
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(txtAsset.text);

        // parse all the xml nodes - see https://wiki.openstreetmap.org/wiki/OSM_XML
        SetBounds(doc.SelectSingleNode("/osm/bounds"));
        GetNodes(doc.SelectNodes("/osm/node"));
        GetWays(doc.SelectNodes("/osm/way"));

        // data is loaded
        IsReady = true;
    }

    private void Update()
    {
        foreach(OsmWay way in ways)
        {
            if (way.Visible)
            {
                Color color = Color.cyan; // cyan for buildings
                if (!way.IsBoundary) color = Color.red; // red for roads

                for (int i = 1; i < way.NodeIDs.Count; i++)
                {
                    OsmNode p1 = nodes[way.NodeIDs[i - 1]];
                    OsmNode p2 = nodes[way.NodeIDs[i]];

                    Vector3 v1 = p1 - bounds.Centre;
                    Vector3 v2 = p2 - bounds.Centre;

                    Debug.DrawLine(v1, v2, color);
                }
            }
        }
    }

    private void GetWays(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode xmlNode in xmlNodeList)
        {
            OsmWay osmWay = new OsmWay(xmlNode);
            ways.Add(osmWay);
        }
    }

    private void GetNodes(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode xmlNode in xmlNodeList)
        {
            OsmNode osmNode = new OsmNode(xmlNode);
            nodes[osmNode.ID] = osmNode;
        }
    }

    private void SetBounds(XmlNode xmlNode)
    {
        bounds = new OsmBounds(xmlNode);
    }
}
