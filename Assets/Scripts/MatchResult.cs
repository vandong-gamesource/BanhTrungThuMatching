using UnityEngine;
using System.Collections.Generic;
public enum MatchDirection
{
    Horizontal,
    Vertical
}
public enum SpecialCakeType
{
    None,
    RowClear,
    ColumnClear,
    Bomb,
    Special
}
public struct MatchLine
{
    public MatchDirection direction;
    public List<int> tiles;
    public MatchLine(MatchDirection direction, List<int> tiles)
    {
        this.direction = direction;
        this.tiles = tiles;
    }
}
public class MatchResult
{
    public List<int> destroyIndices;
    public int spawnIndex=-1;
    public SpecialCakeType specialCakeType;
}
