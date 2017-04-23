using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mapbox.VectorTile;
using Mapbox.VectorTile.ExtensionMethods;
using UnityEngine;
using Mapbox;
using Mapbox.Scripts.Utilities;
using System.Diagnostics;
using Mapbox.MeshGeneration;
using Mapbox.MeshGeneration.Filters;
using Mapbox.MeshGeneration.Data;
using Mapbox.MeshGeneration.Modifiers;

namespace Mapbox.MeshGeneration.Interfaces
{
    [Serializable]
    public class TypeVisualizerTuple
    {
        public string Type;
        public ModifierStack Stack;
    }

    [CreateAssetMenu(menuName = "Mapbox/Layer Visualizer/Vector Layer Visualizer")]
    public class VectorLayerVisualizer : LayerVisualizerBase
    {
        public bool SmoothMeshes = false;

        [SerializeField]
        private string _classificationKey;
        [SerializeField]
        private string _key;
        public override string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        [SerializeField]
        private List<FilterBase> Filters;

        [SerializeField]
        private ModifierStack _defaultStack;
        [SerializeField]
        private List<TypeVisualizerTuple> Stacks;

        private GameObject _container;

        public override void Create(VectorTileLayer layer, UnityTile tile)
        {
            _container = new GameObject(Key + " Container");
            _container.transform.SetParent(tile.transform, false);

            var fc = layer.FeatureCount();
            var filterOut = false;
            for (int i = 0; i < fc; i++)
            {
                filterOut = false;
                var feature = new VectorFeatureUnity(layer.GetFeature(i, 0), tile);
                foreach (var filter in Filters)
                {
                    if (!string.IsNullOrEmpty(filter.Key) && !feature.Properties.ContainsKey(filter.Key))
                        continue;

                    if (!filter.Try(feature))
                    {
                        filterOut = true;
                        break;
                    }
                }

                if (!filterOut)
                    Build(feature, tile, _container);
            }
        }

        private void Build(VectorFeatureUnity feature, UnityTile tile, GameObject parent)
        {
            if (feature.Properties.ContainsKey("extrude") && !bool.Parse(feature.Properties["extrude"].ToString()))
                return;

            foreach (var geometry in feature.Points)
            {
                var meshData = new MeshData();
                meshData.TileRect = tile.Rect;

                if (geometry.Count <= 1)
                    continue;

                System.Object sel = null;
                if (string.IsNullOrEmpty(_classificationKey))
                {
                    sel = feature.Properties.ContainsKey("type") ? feature.Properties["type"] : feature.Properties["class"];
                }
                else if (feature.Properties.ContainsKey(_classificationKey))
                {
                    sel = feature.Properties[_classificationKey].ToString().ToLowerInvariant();
                }
                else
                {
                    continue;
                }

                //we'll run all visualizers on MeshData here 
                var list = geometry.Select(x => Conversions.LatLonToMeters(x.Lat, x.Lng).ToVector3xz()).ToList();

                if (true)
                {
                    var verts = new List<Vector3>();
                    if (list.Count > 1)
                    {
                        for (int i = 0; i < list.Count - 1; i++)
                        {
                            verts.Add(list[i]);
                            var dist = Vector3.Distance(list[i], list[i + 1]);
                            var step = System.Math.Min(40f, dist / 10);
                            if (step > 1)
                            {
                                var counter = 1;
                                while (counter < step)
                                {
                                    var nv = Vector3.Lerp(list[i], list[i + 1], Mathf.Min(1, counter / step));
                                    verts.Add(nv);
                                    counter++;
                                }
                            }
                        }
                    }
                    verts.Add(list.Last());
                    list = verts;
                }

                meshData.Vertices = list.Select(vertex =>
                {
                    var cord = vertex - tile.Rect.center.ToVector3xz();
                    var rx = (vertex.x - tile.Rect.min.x) / tile.Rect.width;
                    var ry = 1 - (vertex.z - tile.Rect.min.y) / tile.Rect.height;

                    var h = tile.QueryHeightData(rx, ry);
                    cord.y += h;

                    //BRNKHY TODO height is broken
                    if (feature.Properties.ContainsKey("min_height"))
                    {
                        var min_height = Convert.ToSingle(feature.Properties["min_height"]);
                        cord.y += min_height;
                    }


                    return cord;
                }).ToList();

                var mod = Stacks.FirstOrDefault(x => x.Type.Contains(sel.ToString().ToLowerInvariant()));
                GameObject go;
                if (mod != null)
                {
                    go = mod.Stack.Execute(feature, meshData, parent, mod.Type);
                }
                else
                {
                    if(_defaultStack != null)
                        go = _defaultStack.Execute(feature, meshData, parent, _key);
                }
                //go.layer = LayerMask.NameToLayer(_key);
            }
        }
    }
}
