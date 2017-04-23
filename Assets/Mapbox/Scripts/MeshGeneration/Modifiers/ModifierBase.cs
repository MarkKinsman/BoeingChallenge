using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mapbox.MeshGeneration.Modifiers
{
    public class ModifierBase : ScriptableObject
    {
        [SerializeField]
        public bool Active = true;
    }
}
