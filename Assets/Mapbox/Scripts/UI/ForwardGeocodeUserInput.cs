﻿//-----------------------------------------------------------------------
// <copyright file="ForwardGeocodeUserInput.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Scripts.UI 
{
	using UnityEngine;
	using UnityEngine.UI;
	using System;
	using Mapbox.Geocoding;
	using Mapbox.Scripts.Utilities;

	[RequireComponent(typeof(InputField))]
	public class ForwardGeocodeUserInput : MonoBehaviour
	{
		InputField _inputField;

		ForwardGeocodeResource _resource;

		GeoCoordinate _coordinate;
		public GeoCoordinate Coordinate
		{
			get
			{
				return _coordinate;
			}
		}

		bool _hasResponse;
		public bool HasResponse
		{
			get
			{
				return _hasResponse;
			}
		}

		public ForwardGeocodeResponse Response { get; private set;}

		public event EventHandler<EventArgs> OnGeocoderResponse;

		void Awake()
		{
			_inputField = GetComponent<InputField>();
			_inputField.onEndEdit.AddListener(HandleUserInput);
			_resource = new ForwardGeocodeResource("");
		}

		void HandleUserInput(string searchString)
		{
			_hasResponse = false;
			if (!string.IsNullOrEmpty(searchString))
			{
				_resource.Query = searchString;
				MapboxConvenience.Instance.Geocoder.Geocode(_resource, HandleGeocoderResponse);
			}
		}

		void HandleGeocoderResponse(ForwardGeocodeResponse res)
		{
			_hasResponse = true;
			_coordinate = res.Features[0].Center;
			Response = res;
			if (OnGeocoderResponse != null)
			{
				OnGeocoderResponse(this, EventArgs.Empty);
			}
		}
	}
}