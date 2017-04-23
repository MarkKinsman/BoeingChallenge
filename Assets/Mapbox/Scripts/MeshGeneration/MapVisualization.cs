using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Mapbox.MeshGeneration;
using Mapbox;
using Mapbox.Unity;
using Mapbox.MeshGeneration.Data;
using Mapbox.MeshGeneration.Factories;

namespace Mapbox.MeshGeneration
{
    [CreateAssetMenu(menuName = "Mapbox/MapVisualization")]
    public class MapVisualization : ScriptableObject
    {
        public List<Factory> Factories;

        public void Initialize(MonoBehaviour runner, IFileSource fs)
        {
            foreach (Factory fac in Factories.Where(x => x != null))
            {
                fac.Initialize(runner, fs);
            }
        }

        public void ShowTile(UnityTile tile)
        {
            foreach (var fac in Factories.Where(x => x != null))
            {
                fac.Register(tile);
            }
        }
    }
}
