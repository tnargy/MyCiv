using UnityEngine;

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
        public static T[] FindPath<T>(
            IQPathWorld world,
            IQPathUnit unit,
            T startTile,
            T endTile,
            CostEstimateDelegate costEstimateFunc
            ) where T : IQPathTile
        {
            // Debug.Log("QPath::FindPath");
            if (world == null || unit == null || startTile == null || endTile == null)
            {
                Debug.LogError("Null values passed to QPath::FindPath");
                return null;
            }

            // Call on our actual path solver
            QPath_AStar<T> resolver = new QPath_AStar<T>(world, unit, startTile, endTile, costEstimateFunc);

            resolver.DoWork();

            return resolver.GetList();
        }
    }

    public delegate float CostEstimateDelegate(IQPathTile a, IQPathTile b);
}