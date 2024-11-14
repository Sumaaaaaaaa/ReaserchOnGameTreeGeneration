using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public partial class Planet
    {
        private abstract class Organ
        {
            public Vector3 Direction;
            public float Biomass;
            public HashSet<Organ> StoragePointer;
        }

        private class Flower : Organ
        {
            
        }
    }

}

