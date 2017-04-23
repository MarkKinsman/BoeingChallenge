//-----------------------------------------------------------------------
// <copyright file="RasterTileExample.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Linq;

namespace Mapbox.Examples.Playground 
{
	using System;
	using Mapbox.Map;
	using Mapbox.Scripts.UI;
	using Mapbox.Scripts.Utilities;
	using UnityEngine;
	using UnityEngine.UI;

	public class RasterTileExample : MonoBehaviour, IObserver<RasterTile>
	{
		[SerializeField]
		ForwardGeocodeUserInput _searchLocation;

		[SerializeField]
		Slider _zoomSlider;

		[SerializeField]
		Dropdown _stylesDropdown;

		[SerializeField]
		RawImage _imageContainer;

		Map<RasterTile> _map;

		// initialize _mapboxStyles
		string[] _mapboxStyles = new string[]		{
			"mapbox://styles/mapbox/satellite-v9",
			"mapbox://styles/mapbox/streets-v9",			"mapbox://styles/mapbox/dark-v9",
			"mapbox://styles/mapbox/light-v9"
		};

		// start location - San Francisco		GeoCoordinate _startLoc = new GeoCoordinate(37.76480, -122.46300);

		int _mapstyle = 0;

		void Awake()
		{
			_searchLocation.OnGeocoderResponse += SearchLocation_OnGeocoderResponse;
			_stylesDropdown.ClearOptions();
			_stylesDropdown.AddOptions(_mapboxStyles.ToList());
			_stylesDropdown.onValueChanged.AddListener(ToggleDropdownStyles);
			_zoomSlider.onValueChanged.AddListener(AdjustZoom);
		}

		void OnDestroy()
		{
			if (_searchLocation != null)
			{
				_searchLocation.OnGeocoderResponse -= SearchLocation_OnGeocoderResponse;
			}
		}

		void Start()
		{
			_map = new Map<RasterTile>(MapboxConvenience.Instance.FileSource);
			_map.MapId = _mapboxStyles[_mapstyle];
			_map.Center = _startLoc;
			_map.Zoom = (int)_zoomSlider.value;
			_map.Subscribe(this);
		}

		/// <summary>
		/// New search location has become available, begin a new _map query.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void SearchLocation_OnGeocoderResponse(object sender, EventArgs e)
		{
			_map.Center = _searchLocation.Coordinate;
		}

		/// <summary>
		/// Zoom was modified by the slider, begin a new _map query.
		/// </summary>
		/// <param name="value">Value.</param>
		void AdjustZoom(float value)
		{
			_map.Zoom = (int)_zoomSlider.value;
		}

		/// <summary>
		/// Style dropdown updated, begin a new _map query.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		void ToggleDropdownStyles(int target)		{
			_mapstyle = target;
			_map.MapId = _mapboxStyles[target];
		}
		/// <summary>
		/// Update the texture with new data.
		/// </summary>
		/// <param name="tile">Tile.</param>
		public void OnNext(RasterTile tile)
		{
			if (tile.CurrentState != Tile.State.Loaded || tile.Error != null)
			{
				return;
			}

			// Can we utility this? Should users have to know source size?
			var texture = new Texture2D(256, 256);
			texture.LoadImage(tile.Data);
			_imageContainer.texture = texture;
		}
	}
}