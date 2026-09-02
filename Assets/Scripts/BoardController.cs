using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public IEnumerator ClearMatchesResult(List<int> destroyIndices)
    {
        // List<int> destroyIndices = new();
        // for (int i = 0; i < matchResults.Count; i++)
        // {
        //     destroyIndices.AddRange(matchResults[i].destroyIndices);
        // }
        // List<int> specialIndices = new();
        // for (int i = 0; i < matchResults.Count; i++)
        // {
        //     if (matchResults[i].specialCakeType != SpecialCakeType.None)
        //     {
        //         specialIndices.Add(matchResults[i].spawnIndex);
        //     }
        // }
        Debug.Log($"Destroy Indices: {string.Join(", ", destroyIndices)}");
        Debug.Log($"Final Destroy Indices after removing special indices: {string.Join(", ", destroyIndices)}");
        destroyIndices = destroyIndices.Distinct().OrderByDescending(index => index).ToList();
        destroyIndices.ForEach(index => {
            _cakes[index].DestroyCake();
            _cakes.RemoveAt(index);
        });
        yield return null;
    }
    private IEnumerator GameLoop()
    {
        yield return SpawnCakes();
        while (true)
        {
            List<MatchLine> matchLines = CheckAllMatch();
            if (matchLines.Count > 0)
            {
                List<MatchResult> matchResults = GetMatchResult(matchLines);
                List<int> destroyIndices = GetDestroyIndices(matchResults);
                List<int> specialIndices = GetSpecialIndices(matchResults);
                yield return ClearMatchesResult(destroyIndices);
                yield return Gravity(destroyIndices);
            }
            else
            {
                Debug.Log("No matches found. Ending game loop.");
                break;
            }
            break;
        }
        Debug.Log("Game loop ended.");
    }   
}
