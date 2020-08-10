namespace QPath
{
    public interface IQPathTile
    {
        IQPathTile[] GetNeighbours();
        float AggregateCostToEnter(float costSoFar, IQPathTile sourceTile, IQPathUnit theUnit);
    }
}