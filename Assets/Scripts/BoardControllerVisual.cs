using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public partial class BoardController : MonoBehaviour
{
    [Header("Board Settings")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GameObject[] banhPrefabs;
    [SerializeField] GridLayoutGroup gridLayoutGroup;
    private IEnumerator SpawnCakes()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                int randomIndex = Random.Range(0, banhPrefabs.Length);
                GameObject cake = Instantiate(banhPrefabs[randomIndex], new Vector3(x, y, 0), Quaternion.identity);
                cake.transform.SetParent(transform);
                cake.transform.localScale = Vector3.zero;
                cake.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
                RectTransform cakeRect = cake.GetComponent<RectTransform>();
                _cakeRects.Add(cakeRect);
                Cake cakeComponent = cake.GetComponent<Cake>();
                cakeComponent.index = GetIndex(x, y);
                _cakes.Add(cakeComponent);
                }
            yield return null;
        }
        gridLayoutGroup.enabled = false;
        for(int i = 0; i < _cakeRects.Count; i++)
        {
            _slotPositions.Add(_cakeRects[i].anchoredPosition);
        }
        yield return null;
    }

    private void SwapCakes(int indexA, int indexB)
    {
        RectTransform rectA = _cakeRects[indexA];
        RectTransform rectB = _cakeRects[indexB];
        Vector2 tempPosition = rectA.anchoredPosition;
        rectA.DOAnchorPos(rectB.anchoredPosition, 0.5f).SetEase(Ease.InOutQuad);
        rectB.DOAnchorPos(tempPosition, 0.5f).SetEase(Ease.InOutQuad);
        // Swap the cakes in the list
        Cake tempCake = _cakes[indexA];
        _cakes[indexA] = _cakes[indexB];
        _cakes[indexB] = tempCake;
    }
    private IEnumerator DestroyCakes(List<int> destroyIndices)
    {
        foreach (var index in destroyIndices)
        {
            RectTransform rect = _cakeRects[index];
            rect.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
            Destroy(_cakes[index].gameObject, 0.5f);
        }
        yield return null;
    }
    private int GetIndex(int x, int y)
    {
        return x * width + y;
    }
}
