//-----------------------------------------------------------------------
// <copyright file="VectorExtensions.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Scripts.Utilities
{
    using MeshGeneration;
    using UnityEngine;

    public static class VectorExtensions
    {
        public static Vector3 ToVector3xz(this Vector2 v)
        {
            return new Vector3(v.x, 0, v.y);
        }

        public static Vector2 ToVector2xz(this Vector3 v)
        {
            return new Vector3(v.x, v.z);
        }

        public static void MoveToGeocoordinate(this Transform t, double lat, double lng)
        {
            var pos = Conversions.LatLonToMeters(lat, lng) - MapController.ReferenceTileRect.center;
            t.position = pos.ToVector3xz() * MapController.WorldScaleFactor;
        }

        public static void MoveToGeocoordinate(this Transform t, Vector2 latLon)
        {
            var pos = Conversions.LatLonToMeters(latLon.x, latLon.y) - MapController.ReferenceTileRect.center;
            t.position = pos.ToVector3xz() * MapController.WorldScaleFactor;
        }

        // TODO: add ability to get geo position from a vector2 or vector 3, as well (not just transform).
        public static Vector3 AsUnityPosition(this Vector2 latLon)
        {
            var pos = Conversions.LatLonToMeters(latLon.x, latLon.y) - MapController.ReferenceTileRect.center;
            return new Vector3(pos.x, 0, pos.y) * MapController.WorldScaleFactor;
        }

        public static GeoCoordinate GetGeoPosition(this Transform t)
        {
            // HACK: prevent NaN in case of temporal coupling.
            if (MapController.WorldScaleFactor <= 0)
            {
                return new GeoCoordinate();
            }

            var pos = MapController.ReferenceTileRect.center.ToVector3xz() + (t.position / MapController.WorldScaleFactor);
            return Conversions.MetersToLatLon(pos.ToVector2xz());
        }

    }
}
