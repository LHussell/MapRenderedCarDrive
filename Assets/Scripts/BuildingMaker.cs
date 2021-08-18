using Assets.Scripts.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MapReader))]
    class BuildingMaker : MonoBehaviour
    {
        MapReader map;

        public Material building;
        IEnumerator Start()
        {
            map = GetComponent<MapReader>();
            while (!map.IsReady)
            {
                yield return null;
            }

            // Process map data to create buildings
            foreach (var way in map.ways.FindAll((way) => { return way.IsBuilding && way.NodeIDs.Count > 1; }))
            {
                // construct the walls of the building
                GameObject gameObject = new GameObject();
                Vector3 localOrigin = GetCentre(way);
                gameObject.transform.position = localOrigin - map.bounds.Centre;

                // generate the mesh and render it
                MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

                meshRenderer.material = building;

                List<Vector3> vectors = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<int> indices = new List<int>();

                // we construct the walls
                for(int i = 1; i < way.NodeIDs.Count; i++)
                {
                    OsmNode p1 = map.nodes[way.NodeIDs[i - 1]];
                    OsmNode p2 = map.nodes[way.NodeIDs[i]];

                    // define vertices of the wall
                    Vector3 v1 = p1 - localOrigin; // bottom left point, data from osm
                    Vector3 v2 = p2 - localOrigin; // bottom right point, data from osm
                    Vector3 v3 = v1 + new Vector3(0, way.Height, 0); // top left point
                    Vector3 v4 = v2 + new Vector3(0, way.Height, 0); // top right point

                    vectors.Add(v1);
                    vectors.Add(v2);
                    vectors.Add(v3);
                    vectors.Add(v4);

                    normals.Add(-Vector3.forward);
                    normals.Add(-Vector3.forward);
                    normals.Add(-Vector3.forward);
                    normals.Add(-Vector3.forward);

                    int idx1, idx2, idx3, idx4;
                    idx4 = vectors.Count - 1;
                    idx3 = vectors.Count - 2;
                    idx2 = vectors.Count - 3;
                    idx1 = vectors.Count - 4;

                    // define triangles using their indices in the correct order (CW,CCW)

                    // CW

                    // first triangle: v1, v3, v2
                    indices.Add(idx1);
                    indices.Add(idx3);
                    indices.Add(idx2);

                    // second triangle: v3, v4, v2
                    indices.Add(idx3);
                    indices.Add(idx4);
                    indices.Add(idx2);

                    // CCW

                    // third triangle: v2, v3, v1
                    indices.Add(idx2);
                    indices.Add(idx3);
                    indices.Add(idx1);

                    // fourth triangle: v2, v4, v3
                    indices.Add(idx2);
                    indices.Add(idx4);
                    indices.Add(idx3);
                }

                meshFilter.mesh.vertices = vectors.ToArray();
                meshFilter.mesh.normals = normals.ToArray();
                meshFilter.mesh.triangles = indices.ToArray();

                yield return null;
            }
        }

        Vector3 GetCentre(OsmWay way)
        {
            Vector3 total = Vector3.zero;

            foreach (var id in way.NodeIDs)
            {
                total += map.nodes[id];
            }

            return total / way.NodeIDs.Count;
        }
    }
}
