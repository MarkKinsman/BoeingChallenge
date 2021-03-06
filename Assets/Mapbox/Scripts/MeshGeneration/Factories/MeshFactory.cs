﻿using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox;
using Mapbox.VectorTile;
using UnityEngine;
using UnityEngine.UI;
using Mapbox.MeshGeneration.Enums;
using Mapbox.MeshGeneration;
using Mapbox.MeshGeneration.Data;
using Mapbox.MeshGeneration.Interfaces;

namespace Mapbox.MeshGeneration.Factories
{
    [CreateAssetMenu(menuName = "Mapbox/Factories/Mesh Factory")]
    public class MeshFactory : Factory
    {
        [SerializeField] private string _mapId = "mapbox.mapbox-streets-v7";
        public List<LayerVisualizerBase> Visualizers;

        private Dictionary<Vector2, UnityTile> _tiles;
        private Dictionary<string, List<LayerVisualizerBase>> _layerBuilder;

        public override void Initialize(MonoBehaviour mb, IFileSource fs)
        {
            base.Initialize(mb, fs);
            _tiles = new Dictionary<Vector2, UnityTile>();
            _layerBuilder = new Dictionary<string, List<LayerVisualizerBase>>();
            foreach (LayerVisualizerBase factory in Visualizers)
            {
                if(_layerBuilder.ContainsKey(factory.Key))
                {
                    _layerBuilder[factory.Key].Add(factory);
                }
                else
                {
                    _layerBuilder.Add(factory.Key, new List<LayerVisualizerBase>() { factory });
                }
            }
        }

        public override void Register(UnityTile tile)
        {
            base.Register(tile);
            _tiles.Add(tile.TileCoordinate, tile);
            Run(tile);
        }

        private void Run(UnityTile tile)
        {
            //waiting for height data. is there a more elegant way to do this?
            if (tile.HeightDataState == TilePropertyState.Loading ||
                tile.ImageDataState == TilePropertyState.Loading)
            {
                tile.HeightDataChanged += (t, e) =>
                {
                    if (tile.ImageDataState != TilePropertyState.Loading)
                        CreateMeshes(t, e);
                };

                tile.SatelliteDataChanged += (t, e) =>
                {
                    if (tile.HeightDataState != TilePropertyState.Loading)
                        CreateMeshes(t, e);
                };
            }
            else
            {
                CreateMeshes(tile, null);
            }
        }

        private void CreateMeshes(UnityTile tile, object e)
        {
            tile.HeightDataChanged -= CreateMeshes;

            var parameters = new Mapbox.Map.Tile.Parameters
            {
                Fs = this.FileSource,
                Id = new Mapbox.Map.CanonicalTileId(tile.Zoom, (int)tile.TileCoordinate.x, (int)tile.TileCoordinate.y),
                MapId = _mapId
            };

            var vectorTile = new Mapbox.Map.VectorTile();
            vectorTile.Initialize(parameters, () =>
            {
                if (vectorTile.Error != null)
                {
                    Debug.Log(vectorTile.Error);
                    return;
                }

                foreach (var layerName in vectorTile.Data.LayerNames())
                {
                    if (_layerBuilder.ContainsKey(layerName))
                    {
                        foreach (var builder in _layerBuilder[layerName])
                        {
                            if (builder.Active)
                                builder.Create(vectorTile.Data.GetLayer(layerName), tile);
                        }                        
                    }
                }
            });
        }
    }
}
