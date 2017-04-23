using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mapbox.VectorTile;
using UnityEngine;
using Mapbox.MeshGeneration;
using Mapbox.MeshGeneration.Data;

namespace Mapbox.MeshGeneration.Interfaces
{
    public abstract class LayerVisualizerBase : ScriptableObject
    {
        public bool Active = true;
        public abstract string Key { get; set; }
        public abstract void Create(VectorTileLayer layer, UnityTile tile);
    }
}
