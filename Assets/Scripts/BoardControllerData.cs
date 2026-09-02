using UnityEngine;
using System.Collections.Generic;
public partial class BoardController : MonoBehaviour
{
    private List<RectTransform> _cakeRects = new();
    private readonly List<Vector2> _slotPositions = new();
    private List<Cake> _cakes = new();  
    private Vector2 GetSlotPosition(int x,int y)
    {
        int index = GetIndex(x, y);
        return GetSlotPositionByIndex(index);
    }
    private Vector2 GetSlotPositionByIndex(int index)
    {
        if (index < 0 || index >= _slotPositions.Count)
        {
            Debug.LogError("Index out of range: " + index);
            return Vector2.zero;
        }
        return _slotPositions[index];
    }
    private void RemoveCakeAtIndex(int index)
    {
        if (index < 0 || index >= _cakes.Count)
        {
            Debug.LogError("Index out of range: " + index);
            return;
        }
        _cakes.RemoveAt(index);
        _cakeRects.RemoveAt(index);
    }

}
