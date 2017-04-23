using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mapbox;
using Mapbox.MeshGeneration.Factories;
using Mapbox.Scripts.Utilities;

public class DirectionsHelper : MonoBehaviour
{
    public DirectionsFactory Directions;
    public List<Transform> Waypoints;

	void Start ()
	{
		// draw directions path at start
		Query ();
	}

    public void Query()
    {
        var waypoints = new List<GeoCoordinate>();
        foreach (var wp in Waypoints)
        {
            waypoints.Add(wp.transform.GetGeoPosition());
        }

        Directions.Query(waypoints);
    }


}
