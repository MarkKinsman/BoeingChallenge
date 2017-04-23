using UnityEngine;
using System.Collections;
using System.Linq;
using Mapbox;
using Mapbox.VectorTile;
using Mapbox.MeshGeneration.Data;

namespace Mapbox.MeshGeneration.Components
{
    public class FeatureBehaviour : MonoBehaviour
    {
        public Transform Transform { get; set; }
        public GameObject GameObject { get; set; }
        public VectorFeatureUnity Data;

        [Multiline(10)]
        public string DataString;

        public void Init(VectorFeatureUnity feature)
        {
            Transform = transform;
            GameObject = gameObject;
            Data = feature;
            DataString = string.Join(" \r\n ", Data.Properties.Select(x => x.Key + " - " + x.Value.ToString()).ToArray());
        }
    }
}