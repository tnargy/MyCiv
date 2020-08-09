using UnityEngine;
using System.Collections;

namespace QPath
{
    /// <summary>
    /// 
    /// Tile will be generic
    /// Tile[] path = QPath.FindPath( ourWorld, theUnit, startTile, endTile );
    /// 
    /// theUnits is an obj that is trying to path between tiels and might
    /// have special logic based on movement type and the type of tiles
    /// moved through
    /// 
    /// Our tiles need to be able to return:
    ///     1: List of neighbors
    ///     2: The aggregate cost to enter this tile from another
    /// 
    /// </summary>
    
    public static class QPath
    {
        public static IEnumerable[] FindPath(IQPathWorld world, IQPathUnit unit, 
            IQPathTile startTile, IQPathTile endTile)
        {
            if (world == null || unit == null || startTile == null || endTile == null)
            {
                Debug.LogError("Null values passed to FindPath");
                return null;
            }

            return null;
        }
    }
}