using System.Collections.Generic;
using AimBurst.Core.Contracts;
using AimBurst.LevelLayout.Contracts;

namespace AimBurst.LevelLayout.Runtime.Abstractions {
    internal struct LayoutLookups {
        public Dictionary<int, Dictionary<int, Queue<ICube>>> byColumnQueue;
        public Dictionary<int, Dictionary<CubeColor, Queue<ICube>>> byColorQueue;
        public LayoutLookups(Dictionary<int, Dictionary<int, Queue<ICube>>> byColumn, Dictionary<int, Dictionary<CubeColor, Queue<ICube>>> byColor) {
            this.byColumnQueue = byColumn;
            this.byColorQueue = byColor;
        }
    }
}
