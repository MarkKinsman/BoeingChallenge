//-----------------------------------------------------------------------
// <copyright file="MapboxConvenience.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Scripts.Utilities
{
	using UnityEngine;
	using System;
	using Mapbox.Unity;
	using Mapbox.Directions;
	using Mapbox.Geocoding;
	using Mapbox.Map;

	public class MapboxConvenience : SingletonBehaviour<MapboxConvenience>
	{
		// HACK: this is a bad place to store this.
		public float TileScale;
		
		[SerializeField]
		string _token;
		public string Token
		{
			get
			{
				return _token;
			}
		}

		FileSource _fileSource;
		public FileSource FileSource
		{
			get
			{
				if(_fileSource == null)
				{
					_fileSource = new FileSource(this);
					_fileSource.AccessToken = _token;
				}

				return _fileSource;
			}
		}

		/// <summary>
		/// Lazy geocoder.
		/// </summary>
		Geocoder _geocoder;
		public Geocoder Geocoder
		{
			get
			{
				if (_geocoder == null)
				{
					_geocoder = new Geocoder(FileSource);
				}
				return _geocoder;
			}
		}

		/// <summary>
		/// Lazy Directions.
		/// </summary>
		Directions _directions;
		public Directions Directions
		{
			get
			{
				if (_directions == null)
				{
					_directions = new Directions(FileSource);
				}
				return _directions;
			}
		}

		public override void Awake()
		{
			if (s_instance != null)
			{
				Destroy(gameObject);
				return;
			}
			base.Awake();
			if (string.IsNullOrEmpty(_token))
			{
				throw new InvalidTokenException("Please get a token from mapbox.com");
			}
			_fileSource = new FileSource(this);
			_fileSource.AccessToken = _token;
		}

		public static UnwrappedTileId LatitudeLongitudeToTileId(float latitude, float longitude, int zoom)
		{
			// See: http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
			var x = (int)Math.Floor((longitude + 180.0) / 360.0 * Math.Pow(2.0, zoom));
			var y = (int)Math.Floor((1.0 - Math.Log(Math.Tan(latitude * Math.PI / 180.0)
					+ 1.0 / Math.Cos(latitude * Math.PI / 180.0)) / Math.PI) / 2.0 * Math.Pow(2.0, zoom));

			return new UnwrappedTileId(zoom, x, y);
		}

		public static Vector2 TileIdToLatitudeLongitude(int x, int y, int zoom)
		{
			var n = Math.Pow(2.0, zoom);
			var lon_deg = (x + .5f) / n * 360.0 - 180.0;
			var lat_rad = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * (y + .5f) / n)));
			var lat_deg = lat_rad * 180.0 / Math.PI;
			return new Vector2((float)lat_deg, (float)lon_deg);
		}

		public static float GetTileScaleInMeters(float latitude, int zoom)
		{
			return 40075000 * Mathf.Cos(Mathf.Deg2Rad * latitude) / (Mathf.Pow(2f, zoom + 8)) * 256;
		}

		public static float GetRelativeHeightFromColor(Color color, float relativeScale)
		{
			return GetAbsoluteHeightFromColor(color) / relativeScale;
		}

		public static float GetAbsoluteHeightFromColor(Color color)
		{
			return (float)(-10000 + ((color.r * 256 * 256 * 256 + color.g * 256 * 256 + color.b * 256) * 0.1));
		}

		class InvalidTokenException : Exception
		{
			public InvalidTokenException(string message) : base(message)
			{
			}
		}
	}
}