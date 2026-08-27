using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
public partial class BoardController : MonoBehaviour
{
    private void Start()
    {
        // StartCoroutine(SpawnCakes());
    }
    public void StartGame()
    {
        StartCoroutine(GameLoop());
    }
    public void ShowMatchesResult()
    {
        List<MatchLine> matchLines = CheckAllMatch();
        List<MatchResult> matchResults = GetMatchResult(matchLines);
        for (int i = 0; i < matchResults.Count; i++)
        {
            foreach (var index in matchResults[i].destroyIndices)
            {
                _cakes[index].Hightlight(matchResults[i].specialCakeType);
            }
        }
    }
    private IEnumerator GameLoop()
    {
        while (true)
        {
            yield return SpawnCakes();
            List<MatchLine> matchLines = CheckAllMatch();
            // for (int i = 0; i < matchLines.Count; i++)
            // {
            //     MatchLine matchLine = matchLines[i];
            //     Debug.Log($"Match found: Direction={matchLine.direction}, Tiles={string.Join(", ", matchLine.tiles)}");
            //     foreach (int index in matchLine.tiles)
            //     {
            //         _cakes[index].Hightlight();
            //     }
            // }
            break;
        }
        Debug.Log("Game loop ended.");
    }   
}
