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
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int randomIndex = Random.Range(0, banhPrefabs.Length);

                GameObject cake = Instantiate(
                    banhPrefabs[randomIndex]
                );

                cake.transform.SetParent(transform, false);
                cake.transform.localScale = Vector3.zero;

                cake.transform
                    .DOScale(Vector3.one, 0.5f)
                    .SetEase(Ease.OutBack);

                RectTransform cakeRect = cake.GetComponent<RectTransform>();

                _cakeRects.Add(cakeRect);

                Cake cakeComponent = cake.GetComponent<Cake>();

                cakeComponent.index = GetIndex(x, y);

                _cakes.Add(cakeComponent);
            }

            yield return null;
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayoutGroup.GetComponent<RectTransform>());

        gridLayoutGroup.enabled = false;

        _slotPositions.Clear();
        for (int i = 0; i < _cakeRects.Count; i++)
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
        HashSet<int> uniqueDestroyIndices = new(destroyIndices);
        foreach (int index in uniqueDestroyIndices)
        {
            if (index < 0 || index >= _cakes.Count || _cakes[index] == null)
            {
                continue;
            }

            RectTransform rect = _cakeRects[index];
            rect.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack);
        }

        yield return new WaitForSeconds(0.7f);

        foreach (int index in uniqueDestroyIndices)
        {
            if (index >= 0 && index < _cakes.Count && _cakes[index] != null)
            {
                Destroy(_cakes[index].gameObject);
            }
        }
    }
    private int GetIndex(int x, int y)
    {
        return y * width + x;
    }
    private IEnumerator RefillAndApplyGravity(List<int> destroyIndices)
    {
        HashSet<int> destroyed = new(destroyIndices);
        List<Cake> nextCakes = new(new Cake[width * height]);
        List<RectTransform> nextCakeRects = new(new RectTransform[width * height]);
        Sequence fallSequence = DOTween.Sequence().Pause();
        const float fallDurationPerCell = 0.12f;

        for (int x = 0; x < width; x++)
        {
            List<int> columnIndices = new();
            for (int y = 0; y < height; y++)
            {
                columnIndices.Add(GetIndex(x, y));
            }

            columnIndices.Sort((left, right) =>
                _slotPositions[left].y.CompareTo(_slotPositions[right].y));

            float cellSize = 1f;
            if (columnIndices.Count > 1)
            {
                Vector2 cellStep = _slotPositions[columnIndices[columnIndices.Count - 1]]
                    - _slotPositions[columnIndices[columnIndices.Count - 2]];
                cellSize = cellStep.magnitude;
                if (cellSize <= 0f)
                {
                    cellSize = 1f;
                }
            }

            List<Cake> oldCakesInColumn = new();
            for (int position = 0; position < columnIndices.Count; position++)
            {
                int sourceIndex = columnIndices[position];
                if (destroyed.Contains(sourceIndex))
                {
                    continue;
                }

                Cake cake = _cakes[sourceIndex];
                if (cake != null)
                {
                    oldCakesInColumn.Add(cake);
                }
            }

            int destroyedCount = 0;
            foreach (int sourceIndex in columnIndices)
            {
                if (destroyed.Contains(sourceIndex))
                {
                    destroyedCount++;
                }
            }

            List<Cake> newCakesInColumn = new();
            int topIndex = columnIndices[columnIndices.Count - 1];
            for (int stackIndex = 0; stackIndex < destroyedCount; stackIndex++)
            {
                GameObject cakeObject = Instantiate(
                    banhPrefabs[Random.Range(0, banhPrefabs.Length)],
                    transform,
                    false);
                RectTransform cakeRect = cakeObject.GetComponent<RectTransform>();
                cakeRect.sizeDelta = gridLayoutGroup.cellSize;
                cakeRect.anchoredPosition = new Vector2(
                    _slotPositions[topIndex].x,
                    _slotPositions[topIndex].y + cellSize * (stackIndex + 1));

                Cake newCake = cakeObject.GetComponent<Cake>();
                newCakesInColumn.Add(newCake);
            }

            int targetPosition = 0;
            foreach (Cake cake in oldCakesInColumn)
            {
                int targetIndex = columnIndices[targetPosition++];
                RectTransform cakeRect = cake.GetComponent<RectTransform>();
                nextCakes[targetIndex] = cake;
                nextCakeRects[targetIndex] = cakeRect;
                float distance = Vector2.Distance(
                    cakeRect.anchoredPosition,
                    _slotPositions[targetIndex]);
                if (distance > 0f)
                {
                    fallSequence.Join(cakeRect
                    .DOAnchorPos(_slotPositions[targetIndex], distance / cellSize * fallDurationPerCell)
                    .SetEase(Ease.Linear)
                    .Pause());
                }
            }

            foreach (Cake newCake in newCakesInColumn)
            {
                int targetIndex = columnIndices[targetPosition++];
                RectTransform newCakeRect = newCake.GetComponent<RectTransform>();
                nextCakes[targetIndex] = newCake;
                nextCakeRects[targetIndex] = newCakeRect;
                float distance = Vector2.Distance(
                    newCakeRect.anchoredPosition,
                    _slotPositions[targetIndex]);
                fallSequence.Join(newCakeRect
                    .DOAnchorPos(_slotPositions[targetIndex], distance / cellSize * fallDurationPerCell)
                    .SetEase(Ease.Linear)
                    .Pause());
            }
        }

        _cakes = nextCakes;
        _cakeRects = nextCakeRects;

        for (int index = 0; index < _cakes.Count; index++)
        {
            if (_cakes[index] != null)
            {
                _cakes[index].index = index;
            }
        }

        if (fallSequence.Duration() > 0f)
        {
            fallSequence.Play();
            yield return fallSequence.WaitForCompletion();
        }

        for (int index = 0; index < _cakes.Count; index++)
        {
            if (_cakes[index] == null)
            {
                continue;
            }

            _cakes[index].index = index;
            _cakeRects[index].anchoredPosition = _slotPositions[index];
        }

    }
}
