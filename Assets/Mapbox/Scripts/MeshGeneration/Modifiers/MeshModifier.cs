using System.Collections;
using Mapbox.VectorTile;
using UnityEngine;
using Mapbox.MeshGeneration;
using Mapbox.MeshGeneration.Data;

namespace Mapbox.MeshGeneration.Modifiers
{
    public enum ModifierType
    {
        Preprocess,
        Postprocess
    }

    public class MeshModifier : ModifierBase
    {
        public virtual ModifierType Type { get { return ModifierType.Preprocess; } }

        public virtual void Run(VectorFeatureUnity feature, MeshData md)
        {

        }
    }
}