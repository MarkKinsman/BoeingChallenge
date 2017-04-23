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
    [CreateAssetMenu(menuName = "Mapbox/Modifiers/Chamfer Modifier")]
    public class ChamferModifier : MeshModifier
    {
        [SerializeField]
        private float _size;

        public override void Run(VectorFeatureUnity feature, MeshData md)
        {
            if (md.Vertices.Count == 0)
                return;

            var final = new List<Vector3>();

            for (int i = 0; i < md.Vertices.Count; i++)
            {
                if (i > 0)
                {
                    var dif = (md.Vertices[i - 1] - md.Vertices[i]);
                    if (dif.magnitude > _size * 2)
                    {
                        dif = dif.normalized * _size;
                        final.Add(md.Vertices[i] + dif);
                    }
                    else
                    {
                        final.Add(md.Vertices[i]);
                    }
                }

                if (i < md.Vertices.Count - 1)
                {
                    var dif = (md.Vertices[i + 1] - md.Vertices[i]);
                    if (dif.magnitude > _size * 2)
                    {
                        dif = dif.normalized * _size;
                        final.Add(md.Vertices[i] + dif);
                    }
                    else
                    {
                        final.Add(md.Vertices[i]);
                    }
                }
            }

            md.Vertices = final;
        }
    }
}
