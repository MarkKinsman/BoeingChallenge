//-----------------------------------------------------------------------
// <copyright file="Rotate.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Scripts.Utilities
{

	using UnityEngine;

	public class Rotate : MonoBehaviour
	{
	    public void Update()
	    {
	        transform.Rotate(0, 20 * Time.deltaTime, 0);
	    }
	}
}
