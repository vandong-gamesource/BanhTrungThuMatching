using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
public partial class BoardController : MonoBehaviour
{
    private List<MatchLine> CheckAllMatch()
    {
        List<MatchLine> matchLines = new();
        // Check horizontal matches
        for (int y = 0; y < height; y++)
        {
            List<int> horizontalMatch = new();
            for (int x = 0; x < width; x++)
            {
                int index = GetIndex(x, y);
                if (_cakes[index] == null)
                {
                    if (horizontalMatch.Count >= 3)
                    {
                        matchLines.Add(new MatchLine(MatchDirection.Horizontal, new List<int>(horizontalMatch)));
                    }
                    horizontalMatch.Clear();
                }
                else if (horizontalMatch.Count == 0)
                {
                    horizontalMatch.Add(index);
                }
                else
                {
                    if (_cakes[index].cakeType == _cakes[horizontalMatch[0]].cakeType)
                    {
                        horizontalMatch.Add(index);
                    }
                    else
                    {
                        if (horizontalMatch.Count >= 3)
                        {
                            matchLines.Add(new MatchLine(MatchDirection.Horizontal, new List<int>(horizontalMatch)));
                        }
                        horizontalMatch.Clear();
                        horizontalMatch.Add(index);
                    }
                }
            }
            if (horizontalMatch.Count >= 3)
            {
                matchLines.Add(new MatchLine(MatchDirection.Horizontal, new List<int>(horizontalMatch)));

            }
            horizontalMatch.Clear();
        }
        Debug.Log("Total horizontal matches found: " + matchLines.Count);
        // Check vertical matches
        for (int x = 0; x < width; x++)
        {
            List<int> verticalMatch = new();
            for (int y = 0; y < height; y++)
            {
                int index = GetIndex(x, y);
                if (_cakes[index] == null)
                {
                    if (verticalMatch.Count >= 3)
                    {
                        matchLines.Add(new MatchLine(MatchDirection.Vertical, new List<int>(verticalMatch)));
                    }
                    verticalMatch.Clear();
                }
                else if (verticalMatch.Count == 0)
                {
                    verticalMatch.Add(index);
                }
                else
                {
                    if (_cakes[index].cakeType == _cakes[verticalMatch[0]].cakeType)
                    {
                        verticalMatch.Add(index);
                    }
                    else
                    {
                        if (verticalMatch.Count >= 3)
                        {
                            matchLines.Add(new MatchLine(MatchDirection.Vertical, new List<int>(verticalMatch)));
                        }
                        verticalMatch.Clear();
                        verticalMatch.Add(index);
                    }
                }
            }
            if (verticalMatch.Count >= 3)
            {
                matchLines.Add(new MatchLine(MatchDirection.Vertical, new List<int>(verticalMatch)));
            }
            verticalMatch.Clear();
        }
        Debug.Log("Total matches found: " + matchLines.Count);
        return matchLines;
    }
    private List<MatchLine> CheckMatchForIndex(int index)
    {
        List<MatchLine> matchLines = new();
        if (index < 0 || index >= _cakes.Count || _cakes[index] == null)
        {
            return matchLines;
        }

        int x = index % width;
        int y = index / width;
        CakeType targetType = _cakes[index].cakeType;

        // Check horizontal match
        List<int> horizontalMatch = new();
        for (int currentX = 0; currentX < width; currentX++)
        {
            int currentIndex = GetIndex(currentX, y);
            if (_cakes[currentIndex] != null && _cakes[currentIndex].cakeType == targetType)
            {
                horizontalMatch.Add(currentIndex);
            }
            else
            {
                if (horizontalMatch.Count >= 3 && horizontalMatch.Contains(index))
                {
                    matchLines.Add(new MatchLine(MatchDirection.Horizontal, new List<int>(horizontalMatch)));
                }
                horizontalMatch.Clear();
            }
        }
        if (horizontalMatch.Count >= 3 && horizontalMatch.Contains(index))
        {
            matchLines.Add(new MatchLine(MatchDirection.Horizontal, new List<int>(horizontalMatch)));
        }

        // Check vertical match
        List<int> verticalMatch = new();
        for (int currentY = 0; currentY < height; currentY++)
        {
            int currentIndex = GetIndex(x, currentY);
            if (_cakes[currentIndex] != null && _cakes[currentIndex].cakeType == targetType)
            {
                verticalMatch.Add(currentIndex);
            }
            else
            {
                if (verticalMatch.Count >= 3 && verticalMatch.Contains(index))
                {
                    matchLines.Add(new MatchLine(MatchDirection.Vertical, new List<int>(verticalMatch)));
                }
                verticalMatch.Clear();
            }
        }
        if (verticalMatch.Count >= 3 && verticalMatch.Contains(index))
        {
            matchLines.Add(new MatchLine(MatchDirection.Vertical, new List<int>(verticalMatch)));
        }
        return matchLines;
    }
    private List<MatchResult> GetMatchResult(List<MatchLine> matchLines,int playerMoveIndex=-1)
    {
        List<MatchResult> matchResults = new();
        List<int> blackListLines = new(); // which indices have been processed to avoid duplicates
        //Check for special matches (4 or more in a line, or intersecting lines)
        for(int i = 0; i < matchLines.Count-1; i++)
        {
            for(int j=i+1;j<matchLines.Count;j++)
            {
                if (blackListLines.Contains(i) || blackListLines.Contains(j))
                {
                    continue; // Skip if either match line has already been processed
                }
                List<int> intersection = new(matchLines[i].tiles);
                bool hasIntersection = intersection
                .Intersect(matchLines[j].tiles)
                .Any();
                if(hasIntersection)
                {
                    List<int> combinedTiles = matchLines[i].tiles.Union(matchLines[j].tiles).ToList();
                    if(matchLines[i].direction==matchLines[j].direction)
                    {
                        if(combinedTiles.Count>=4)
                        {
                            if (combinedTiles.Count == 4)
                            {
                               MatchResult matchResult = new()
                                {
                                    destroyIndices = combinedTiles,
                                    spawnIndex = playerMoveIndex != -1 ? playerMoveIndex : intersection[0],
                                    specialCakeType = matchLines[i].direction == MatchDirection.Horizontal ? SpecialCakeType.RowClear : SpecialCakeType.ColumnClear
                                };
                                matchResults.Add(matchResult);
                            }
                            else
                            {
                                combinedTiles.RemoveRange(4, combinedTiles.Count - 4);
                                MatchResult matchResult = new()
                                {
                                    destroyIndices = combinedTiles,
                                    spawnIndex = playerMoveIndex != -1 ? playerMoveIndex : intersection[0],
                                    specialCakeType = SpecialCakeType.Special
                                };
                                matchResults.Add(matchResult);
                            }
                        }

                    }
                    else
                    {
                        MatchResult matchResult = new()
                        {
                            destroyIndices = combinedTiles,
                            spawnIndex = playerMoveIndex != -1 ? playerMoveIndex : intersection[0],
                            specialCakeType = SpecialCakeType.Bomb
                        };
                        matchResults.Add(matchResult);
                    }
                    blackListLines.Add(i);
                    blackListLines.Add(j);
                }
            }
        }
        // Process remaining match lines that were not part of any special match
        for (int i = 0; i < matchLines.Count; i++)
        {
            if (blackListLines.Contains(i))
            {
                continue; // Skip if this match line has already been processed
            }
            if(matchLines[i].tiles.Count>=4)
            {
                if(matchLines[i].tiles.Count==4)
                {
                    MatchResult matchResult = new()
                    {
                        destroyIndices = matchLines[i].tiles,
                        spawnIndex = playerMoveIndex != -1 ? playerMoveIndex : matchLines[i].tiles[0],
                        specialCakeType = matchLines[i].direction == MatchDirection.Horizontal ? SpecialCakeType.RowClear : SpecialCakeType.ColumnClear
                    };
                    matchResults.Add(matchResult);
                }
                else
                {
                    List<int> limitedTiles = new(matchLines[i].tiles);
                    limitedTiles.RemoveRange(4, limitedTiles.Count - 4);
                    MatchResult matchResult = new()
                    {
                        destroyIndices = limitedTiles,
                        spawnIndex = playerMoveIndex != -1 ? playerMoveIndex : matchLines[i].tiles[0],
                        specialCakeType = SpecialCakeType.Special
                    };
                    matchResults.Add(matchResult);
                }
            }else
            {
                MatchResult matchResult = new()
                {
                    destroyIndices = matchLines[i].tiles,
                    spawnIndex = playerMoveIndex != -1 ? playerMoveIndex : matchLines[i].tiles[0],
                    specialCakeType = SpecialCakeType.None
                };
                matchResults.Add(matchResult);
            }
        }
        return matchResults;
    }
    private List<int> GetDestroyIndices(List<MatchResult> matchResults)
    {
        List<int> destroyIndices = new();
        for (int i = 0; i < matchResults.Count; i++)
        {
            destroyIndices.AddRange(matchResults[i].destroyIndices);
        }
        return destroyIndices.Except(GetSpecialIndices(matchResults)).ToList();
    }
    private List<int> GetSpecialIndices(List<MatchResult> matchResults)
    {
        List<int> specialIndices = new();
        for (int i = 0; i < matchResults.Count; i++)
        {
            if (matchResults[i].specialCakeType != SpecialCakeType.None)
            {
                specialIndices.Add(matchResults[i].spawnIndex);
            }
        }
        return specialIndices;
    }
}
