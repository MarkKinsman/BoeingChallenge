namespace Mapbox.Examples.Voxels
{
	using Mapbox.Map;
	using Mapbox.Unity;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Collections;
	using System.Linq;
	using Mapbox.Scripts.UI;
	using Mapbox.Scripts.Utilities;
	using System;

	class VoxelTile : MonoBehaviour, IObserver<RasterTile>, IObserver<RawPngRasterTile>
	{
		[SerializeField]
		ForwardGeocodeUserInput _geocodeInput;

		[SerializeField]
		int _zoom = 17;

		[SerializeField]
		float _elevationMultiplier = 1f;

		[SerializeField]
		int _voxelDepthPadding = 1;

		[SerializeField]
		int _tileWidthInVoxels;

		[SerializeField]
		VoxelFetcher _voxelFetcher;

		[SerializeField]
		GameObject _player;

		[SerializeField]
		int _voxelBatchCount = 100;

		[SerializeField]
		string _styleUrl;

		Map<RasterTile> _raster;
		Map<RawPngRasterTile> _elevation;

		Texture2D _rasterTexture;
		Texture2D _elevationTexture;

		FileSource _fileSource;

		List<VoxelData> _voxels = new List<VoxelData>();

		List<GameObject> _instantiatedVoxels = new List<GameObject>();

		float _tileScale;

		void Awake()
		{
			_geocodeInput.OnGeocoderResponse += GeocodeInput_OnGeocoderResponse;
		}

		void OnDestroy()
		{
			if (_geocodeInput)
			{
				_geocodeInput.OnGeocoderResponse -= GeocodeInput_OnGeocoderResponse;
			}
		}

		void Start()
		{
			_fileSource = MapboxConvenience.Instance.FileSource;

			_raster = new Map<RasterTile>(_fileSource);
			_elevation = new Map<RawPngRasterTile>(_fileSource);

			if (!string.IsNullOrEmpty(_styleUrl))
			{
				_raster.MapId = _styleUrl;
			}

			_elevation.Subscribe(this);
			_raster.Subscribe(this);

			// Torres Del Paine
			FetchWorldData(new GeoCoordinate(-50.98306, -72.96639));
		}

		void GeocodeInput_OnGeocoderResponse(object sender, EventArgs e)
		{
			Debug.Log("VoxelTile Search Location: " + _geocodeInput.Coordinate);
			Cleanup();
			FetchWorldData(_geocodeInput.Coordinate);
		}

		void Cleanup()
		{
			StopAllCoroutines();
			_rasterTexture = null;
			_elevationTexture = null;
			_voxels.Clear();
			foreach (var voxel in _instantiatedVoxels)
			{
				Destroy(voxel);
			}
		}

		void FetchWorldData(GeoCoordinate coordinates)
		{
			_tileScale = _tileWidthInVoxels / MapboxConvenience.GetTileScaleInMeters((float)coordinates.Latitude, _zoom);
			var bounds = new GeoCoordinateBounds();
			bounds.Center = coordinates;
			_raster.SetGeoCoordinateBoundsZoom(bounds, _zoom);
			_elevation.SetGeoCoordinateBoundsZoom(bounds, _zoom);
		}

		public void OnNext(RasterTile tile)
		{
			if (tile.CurrentState == Tile.State.Loaded && tile.Error == null)
			{
				_rasterTexture = new Texture2D(2, 2);
				_rasterTexture.LoadImage(tile.Data);
				TextureScale.Point(_rasterTexture, _tileWidthInVoxels, _tileWidthInVoxels);

				if (ShouldBuildWorld())
				{
					BuildVoxelWorld();
				}
			}
		}

		public void OnNext(RawPngRasterTile tile)
		{
			if (tile.CurrentState == Tile.State.Loaded && tile.Error == null)
			{
				_elevationTexture = new Texture2D(2, 2);
				_elevationTexture.LoadImage(tile.Data);
				TextureScale.Point(_elevationTexture, _tileWidthInVoxels, _tileWidthInVoxels);

				if (ShouldBuildWorld())
				{
					BuildVoxelWorld();
				}
			}
		}

		bool ShouldBuildWorld()
		{
			return _rasterTexture != null && _elevationTexture != null;
		}

		void BuildVoxelWorld()
		{
			var baseHeight = (int)MapboxConvenience.GetRelativeHeightFromColor((_elevationTexture.GetPixel(_elevationTexture.width / 2, _elevationTexture.height / 2)), 1 / _elevationMultiplier);
			for (int x = 0; x < _rasterTexture.width; x++)
			{
				for (int z = 0; z < _rasterTexture.height; z++)
				{
					var height = (int)MapboxConvenience.GetRelativeHeightFromColor(_elevationTexture.GetPixel(x, z), 1 / _elevationMultiplier) - baseHeight;

					height = (int)(height * _tileScale);
					var startHeight = height - _voxelDepthPadding - 1;
					var color = _rasterTexture.GetPixel(x, z);

					for (int y = startHeight; y < height; y++)
					{
						_voxels.Add(new VoxelData() { Position = new Vector3(x, y, z), Prefab = _voxelFetcher.GetVoxelFromColor(color) });
					}
				}
			}

			// TODO: decouple me!
			_player.transform.position = new Vector3(_tileWidthInVoxels * .5f, 2f, _tileWidthInVoxels * .5f);
			StartCoroutine(BuildRoutine());
			var cc = _player.GetComponent<CharacterController>();
			if (cc)
			{
				cc.enabled = true;
			}
		}

		IEnumerator BuildRoutine()
		{
			var distanceOrderedVoxels = _voxels.OrderBy(x => (_player.transform.position - x.Position).magnitude).ToList();

			for (int i = 0; i < distanceOrderedVoxels.Count; i += _voxelBatchCount)
			{
				for (int j = 0; j < _voxelBatchCount; j++)
				{
					var index = i + j;
					if (index < distanceOrderedVoxels.Count)
					{
						var voxel = distanceOrderedVoxels[index];
						_instantiatedVoxels.Add(Instantiate(voxel.Prefab, voxel.Position, Quaternion.identity, transform) as GameObject);
					}
				}
				yield return null;
			}
		}
	}
}