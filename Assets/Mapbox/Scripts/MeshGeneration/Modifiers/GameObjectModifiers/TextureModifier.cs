using System.Collections.Generic;
using System.Linq;
using Mapbox.VectorTile;
using TriangleNet;
using TriangleNet.Geometry;
using UnityEngine;
using Mapbox.MeshGeneration.Components;

namespace Mapbox.MeshGeneration.Modifiers
{
    [CreateAssetMenu(menuName = "Mapbox/Modifiers/Texture Modifier")]
    public class TextureModifier : GameObjectModifier
    {
        [SerializeField]
        private bool _textureTop;
        [SerializeField]
        private bool _useSatelliteTexture;
        [SerializeField]
        private Material[] _topMaterials;

        [SerializeField]
        private bool _textureSides;
        [SerializeField]
        private Material[] _sideMaterials;

        public override void Run(FeatureBehaviour fb)
        {
            var ts = fb.GameObject.AddComponent<TextureSelector>();
            ts.Initialize(fb, _textureTop, _useSatelliteTexture, _topMaterials, _textureSides, _sideMaterials);
        }
    }
}
