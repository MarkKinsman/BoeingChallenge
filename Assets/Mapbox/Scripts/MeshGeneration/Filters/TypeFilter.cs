using UnityEngine;
using System.Collections;
using Mapbox.VectorTile;
using Mapbox.MeshGeneration.Data;

namespace Mapbox.MeshGeneration.Filters
{
    [CreateAssetMenu(menuName = "Mapbox/Filters/Type Filter")]
    public class TypeFilter : FilterBase
    {
        public override string Key { get { return "type"; } }
        [SerializeField]
        private string _type;
        [SerializeField]
        private TypeFilterType _behaviour;

        public override bool Try(VectorFeatureUnity feature)
        {
            var check = _type.ToLowerInvariant().Contains(feature.Properties["type"].ToString().ToLowerInvariant());
            return _behaviour == TypeFilterType.Include ? check : !check;
        }

        public enum TypeFilterType
        {
            Include,
            Exclude
        }
    }
}