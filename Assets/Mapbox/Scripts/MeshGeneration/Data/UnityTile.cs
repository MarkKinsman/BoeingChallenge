﻿using System;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using Mapbox.MeshGeneration.Enums;

namespace Mapbox.MeshGeneration.Data
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class UnityTile : MonoBehaviour, INotifyPropertyChanged
    {
        #region basic properties //move to a base class?
        [SerializeField] private Texture2D _heightData;
        [SerializeField] private Texture2D _imageData;
        [SerializeField] private string _vectorData;

        public Texture2D ImageData
        {
            get { return _imageData; }
            set
            {
                _imageData = value;
                OnSatelliteDataChanged();
            }
        }
        public Texture2D HeightData
        {
            get { return _heightData; }
            set
            {
                _heightData = value;
                OnHeightDataChanged();
            }
        }
        public string VectorData
        {
            get { return _vectorData; }
            set
            {
                _vectorData = value;
                OnVectorDataChanged();
            }
        }

        public TilePropertyState ImageDataState { get; set; }
        public TilePropertyState HeightDataState { get; set; }
        public TilePropertyState VectorDataState { get; set; }
        #endregion

        public Vector2 TileCoordinate { get; set; }
        public int Zoom { get; set; }
        public Rect Rect { get; set; }
		public float RelativeScale { get; set;}
        
        public float QueryHeightData(float x, float y)
        {
            if(HeightData != null)
            {
                return Conversions.GetRelativeHeightFromColor(HeightData.GetPixel(
                        (int)Mathf.Clamp((x * 256), 0, 255),
                        (int)Mathf.Clamp((y * 256), 0, 255)), RelativeScale);
            }

            return 0;
        }

        #region Events //again move to base class?
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void TileEventArgs(UnityTile sender, object param);
        public event TileEventArgs HeightDataChanged;
        public event TileEventArgs SatelliteDataChanged;
        public event TileEventArgs VectorDataChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnHeightDataChanged()
        {
            var handler = HeightDataChanged;
            if (handler != null) handler(this, null);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnSatelliteDataChanged()
        {
            var handler = SatelliteDataChanged;
            if (handler != null) handler(this, null);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnVectorDataChanged()
        {
            var handler = VectorDataChanged;
            if (handler != null) handler(this, null);
        } 
        #endregion
    }
}
