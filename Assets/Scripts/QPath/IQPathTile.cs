using UnityEngine;
using System.Collections;

namespace QPath
{
    public interface IQPathTile
    {
        IQPathTile[] GetNeighbors();
        float AggregateCostToEnter(float costSoFar, IQPathTile sourceTile, IQPathUnit theUnit);
    }
}