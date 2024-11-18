using System;
using System.Collections.Generic;

namespace UnlimitedGreen
{
    internal class PhytomerCohort
    {
        private struct PhytomerCohortData
        {
            public HashSet<EntityPhytomer> Phytomers;
            public int BirthCycle;
        }

        private Queue<PhytomerCohortData>[] _data;
        private PhytomerData _phytomerData;

        public PhytomerCohort(PhytomerData phytomerData)
        {
            _phytomerData = phytomerData;
            
            //TODO: ...NOW...
        }
    }
}