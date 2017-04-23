﻿using UnityEngine;
using System.Collections;
using Mapbox.MeshGeneration;
using Mapbox.Scripts.Utilities;
using Mapbox;
using Mapbox.Map;

public class Slippy : MonoBehaviour
{
    private MapController _mapController;
    private Camera _camera;
    [SerializeField]
    private int _range = 2;

    Ray _ray;
    float _hitDistance;
    Plane _yPlane;
    Vector3 _cameraTarget;
    Vector2 _cachedTile;
    Vector2  _currentTile;

    void Start()
    {
        _mapController = GetComponent<MapController>();
        _camera = Camera.main;
        _yPlane = new Plane(Vector3.up, Vector3.zero);
    }

    
    void Update()
    {
        _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (_yPlane.Raycast(_ray, out _hitDistance))
        {
            _cameraTarget = _ray.GetPoint(_hitDistance) / MapController.WorldScaleFactor;
            _currentTile = Conversions.MetersToTile(_cameraTarget.ToVector2xz() + MapController.ReferenceTileRect.center, _mapController.Zoom);
            if(_currentTile != _cachedTile)
            {
                for (int i = -_range; i <= _range; i++)
                {
                    for (int j = -_range; j <= _range; j++)
                    {
                        _mapController.Request(new Vector2(_currentTile.x + i, _currentTile.y + j), _mapController.Zoom);
                    }
                }
                _cachedTile = _currentTile;
            }
        }
    }
}
