using UnityEngine;
using System.Collections;

namespace QPath
{
    public interface IQPathUnit
    {
        float CostToEnterHex(IQPathTile sourceTile, IQPathTile destinationTile);
    }
}