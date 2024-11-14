using System.Collections.Generic;
using UnityEngine;

namespace UnlimitedGreen
{
    internal abstract class EntityOrgan<T>
    {
        public Vector3 Direction;
        public float Biomass;
        public HashSet<T> StoragePointer;
    }

    internal class EntityFlower : EntityOrgan<EntityFlower>{}

    internal class EntityFruit : EntityOrgan<EntityFruit>{}
    
    internal class EntityLeaf : EntityOrgan<EntityLeaf>{}

}