//-----------------------------------------------------------------------
// <copyright file="MouseDrag.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Scripts.Utilities
{

	using UnityEngine;

	public class MouseDrag : MonoBehaviour
	{
		[SerializeField]
		LayerMask _layerMask;

		void OnMouseDrag()
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
			{
				var position = hit.point;
				position.y = 100f;
				transform.position = position;
			}
		}
	}
}