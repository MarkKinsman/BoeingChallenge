﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mapbox.VectorTile;
using TriangleNet;
using TriangleNet.Geometry;
using UnityEngine;
using Mapbox.MeshGeneration;
using Mapbox.MeshGeneration.Data;

namespace Mapbox.MeshGeneration.Modifiers
{
    public enum ExtrusionType
    {
        Wall,
        FirstMidFloor,
        FirstMidTopFloor
    }

    [CreateAssetMenu(menuName = "Mapbox/Modifiers/Height Modifier")]
    public class HeightModifier : MeshModifier
    {
        [SerializeField]
        private float _height;
        [SerializeField]
        private bool _forceHeight;

        public override ModifierType Type { get { return ModifierType.Preprocess; } }

        public override void Run(VectorFeatureUnity feature, MeshData md)
        {
            if (md.Vertices.Count == 0)
                return;



            //brnkhy there has to be a better way to do this?
            float hf = _height;
            if (feature != null)
            {
                if (!_forceHeight)
                {
                    if (feature.Properties.ContainsKey("height"))
                    {
                        if (float.TryParse(feature.Properties["height"].ToString(), out hf))
                        {
                            if (feature.Properties.ContainsKey("min_height"))
                            {
                                hf -= float.Parse(feature.Properties["min_height"].ToString());
                            }
                        }
                    }
                    if (feature.Properties.ContainsKey("ele"))
                    {
                        if (float.TryParse(feature.Properties["ele"].ToString(), out hf))
                        {
                        }
                    }
                }
            }


            for (int i = 0; i < md.Vertices.Count; i++)
            {
                md.Vertices[i] = new Vector3(md.Vertices[i].x, md.Vertices[i].y + hf, md.Vertices[i].z);
            }

            var vertsStartCount = 0;
            var count = md.Vertices.Count;
            float d = 0f;
            Vector3 v1;
            Vector3 v2;
            int ind = 0;

            var wallTri = new List<int>();
            var wallUv = new List<Vector2>();

            for (int i = 1; i < count; i++)
            {
                v1 = md.Vertices[vertsStartCount + i - 1];
                v2 = md.Vertices[vertsStartCount + i];
                ind = md.Vertices.Count;
                md.Vertices.Add(v1);
                md.Vertices.Add(v2);
                md.Vertices.Add(new Vector3(v1.x, md.Vertices[i].y - hf, v1.z));
                md.Vertices.Add(new Vector3(v2.x, md.Vertices[i].y - hf, v2.z));

                d = (v2 - v1).magnitude;

                wallUv.Add(new Vector2(0, 0));
                wallUv.Add(new Vector2(d, 0));
                wallUv.Add(new Vector2(0, -hf));
                wallUv.Add(new Vector2(d, -hf));

                wallTri.Add(ind);
                wallTri.Add(ind + 2);
                wallTri.Add(ind + 1);

                wallTri.Add(ind + 1);
                wallTri.Add(ind + 2);
                wallTri.Add(ind + 3);
            }


            v1 = md.Vertices[vertsStartCount];
            v2 = md.Vertices[vertsStartCount + count - 1];
            ind = md.Vertices.Count;
            md.Vertices.Add(v1);
            md.Vertices.Add(v2);
            md.Vertices.Add(new Vector3(v1.x, md.Vertices[ind].y - hf, v1.z));
            md.Vertices.Add(new Vector3(v2.x, md.Vertices[ind].y - hf, v2.z));

            d = (v2 - v1).magnitude;

            wallUv.Add(new Vector2(0, 0));
            wallUv.Add(new Vector2(d, 0));
            wallUv.Add(new Vector2(0, -hf));
            wallUv.Add(new Vector2(d, -hf));

            wallTri.Add(ind);
            wallTri.Add(ind + 1);
            wallTri.Add(ind + 2);

            wallTri.Add(ind + 1);
            wallTri.Add(ind + 3);
            wallTri.Add(ind + 2);


            md.Triangles.Add(wallTri);

            md.UV[0].AddRange(wallUv);
        }
    }
}
