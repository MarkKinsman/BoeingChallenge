using System;
using Mapbox;
using UnityEngine;
using Mapbox.MeshGeneration;
using Mapbox.MeshGeneration.Data;

namespace Mapbox.MeshGeneration.Factories
{
    public class Factory : ScriptableObject
    {
        protected IFileSource FileSource;

        public virtual void Initialize(MonoBehaviour mb, IFileSource fileSource)
        {
            FileSource = fileSource;
        }

        public virtual void Register(UnityTile tile)
        {
            
        }
    }
}
