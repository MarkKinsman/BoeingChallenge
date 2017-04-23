using UnityEngine;
using System.Collections;
using Mapbox.VectorTile;
using System.Collections.Generic;
using System;
using Mapbox.MeshGeneration.Data;

namespace Mapbox.MeshGeneration.Filters
{
    public class FilterBase : ScriptableObject
    {
        public virtual string Key { get { return ""; } }

        public virtual bool Try(VectorFeatureUnity feature)
        {
            return true;
        }
    }
}