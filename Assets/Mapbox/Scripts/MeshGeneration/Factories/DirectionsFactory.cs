using UnityEngine;
using System.Collections;
using Mapbox;
using Mapbox.Directions;
using System.Collections.Generic;
using System.Linq;
using Mapbox.Scripts.Utilities;
using Mapbox.MeshGeneration;
using Mapbox.MeshGeneration.Data;
using Mapbox.MeshGeneration.Modifiers;

namespace Mapbox.MeshGeneration.Factories
{

    [CreateAssetMenu(menuName = "Mapbox/Factories/Direction Factory")]
    public class DirectionsFactory : Factory
    {
        [SerializeField]
        private Material _material;
        private Directions.Directions _directions;
        public List<MeshModifier> MeshModifiers;

        public override void Initialize(MonoBehaviour mb, IFileSource fileSource)
        {
            base.Initialize(mb, fileSource);
            _directions = new Directions.Directions(fileSource);
        }

        public void Query(List<GeoCoordinate> waypoints)
        {


            var _directionResource = new DirectionResource(waypoints.ToArray(), RoutingProfile.Driving);
            _directionResource.Steps = true;
            _directions.Query(_directionResource, HandleDirectionsResponse);
        }

        void HandleDirectionsResponse(DirectionsResponse response)
        {
            var meshData = new MeshData();

            foreach (var leg in response.Routes[0].Legs)
            {
                foreach (var step in leg.Steps)
                {
                    var pos = Conversions.LatLonToMeters(step.Maneuver.Location.Latitude, step.Maneuver.Location.Longitude) - MapController.ReferenceTileRect.center;
                    meshData.Vertices.Add(pos.ToVector3xz() * MapController.WorldScaleFactor);

                }
            }

            foreach (MeshModifier mod in MeshModifiers.Where(x => x.Active))
            {
                mod.Run(null, meshData);
            }

            CreateGameObject(meshData);
        }

        private GameObject CreateGameObject(MeshData data)
        {
            var go = new GameObject("direction waypoint " + " entity");
            var mesh = go.AddComponent<MeshFilter>().mesh;
            mesh.subMeshCount = data.Triangles.Count;

            mesh.SetVertices(data.Vertices);
            for (int i = 0; i < data.Triangles.Count; i++)
            {
                var triangle = data.Triangles[i];
                mesh.SetTriangles(triangle, i);
            }

            for (int i = 0; i < data.UV.Count; i++)
            {
                var uv = data.UV[i];
                mesh.SetUVs(i, uv);
            }

            mesh.RecalculateNormals();
            go.AddComponent<MeshRenderer>().material = _material;
            return go;
        }
    }

}