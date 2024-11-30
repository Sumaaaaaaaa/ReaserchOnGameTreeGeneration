using System.Collections.Generic;
using UnityEngine;

namespace UnlimitedGreen
{
    internal class EntityPhytomer
    {
        public HashSet<EntityPhytomer> StoragePointer;
        public float Radius;
        public Vector3 Direction;
        public Vector3 SubDirection;
        public float Length;
        public Vector3 Position;
        public EntityFlower[] AxillaryFlowers;
        public EntityFruit[] AxillaryFruits;
        public EntityLeaf[] AxillaryLeaves;
        public Axis[] AxillaryAxis;
    }
}