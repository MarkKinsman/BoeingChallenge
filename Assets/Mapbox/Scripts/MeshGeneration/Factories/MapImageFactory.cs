using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox;
using Mapbox.Map;
using UnityEngine;
using UnityEngine.UI;
using Mapbox.MeshGeneration.Enums;
using Mapbox.MeshGeneration.Data;

namespace Mapbox.MeshGeneration.Factories
{
    [CreateAssetMenu(menuName = "Mapbox/Factories/Map Image Factory")]
    public class MapImageFactory : Factory
    {
        [SerializeField]
        public string _mapId;
        [SerializeField]
        public Material _baseMaterial;
        [SerializeField]
        private bool _createImagery = true;

        private Dictionary<Vector2, UnityTile> _tiles;

        public override void Initialize(MonoBehaviour mb, IFileSource fs)
        {
            base.Initialize(mb, fs);
            _tiles = new Dictionary<Vector2, UnityTile>();
        }

        public override void Register(UnityTile tile)
        {
            base.Register(tile);
            _tiles.Add(tile.TileCoordinate, tile);
            Run(tile);
        }

        private void Run(UnityTile tile)
        {
            if (!string.IsNullOrEmpty(_mapId))
            {
                var parameters = new Tile.Parameters();
                parameters.Fs = this.FileSource;
                parameters.Id = new CanonicalTileId(tile.Zoom, (int)tile.TileCoordinate.x, (int)tile.TileCoordinate.y);
                parameters.MapId = _mapId;

                tile.ImageDataState = TilePropertyState.Loading;
                var rasterTile = parameters.MapId.StartsWith("mapbox://") ? new RasterTile() : new ClassicRasterTile();
                rasterTile.Initialize(parameters, (Action)(() =>
                {
                    if (rasterTile.Error != null)
                    {
                        tile.ImageDataState = TilePropertyState.Error;
                        return;
                    }

                    var rend = tile.GetComponent<MeshRenderer>();
                    rend.material = _baseMaterial;
                    tile.ImageData = new Texture2D(256, 256, TextureFormat.RGB24, false);
                    tile.ImageData.wrapMode = TextureWrapMode.Clamp;
                    tile.ImageData.LoadImage(rasterTile.Data);
                    rend.material.mainTexture = tile.ImageData;
                    tile.ImageDataState = TilePropertyState.Loaded;

                }));
            }
            else
            {
                var rend = tile.GetComponent<MeshRenderer>();
                rend.material = _baseMaterial;
            }
        }
    }
}
