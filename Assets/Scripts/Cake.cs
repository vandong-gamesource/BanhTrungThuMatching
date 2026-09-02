using UnityEngine;
using UnityEngine.UI;
public enum CakeType
{
    Red,
    Green,
    Blue,
    Yellow,
    Purple
}
public class Cake : MonoBehaviour
{
    public CakeType cakeType;
    public int index;
    public void Hightlight(SpecialCakeType specialType = SpecialCakeType.None)
    {
       if (TryGetComponent<Image>(out var image))
        {
            image.color = specialType switch
            {
                SpecialCakeType.RowClear => Color.cyan,// Highlight for Row Clear
                SpecialCakeType.ColumnClear => Color.magenta,// Highlight for Column Clear
                SpecialCakeType.Bomb => Color.yellow,// Highlight for Bomb
                SpecialCakeType.Special => Color.white,// Highlight for Special
                _ => Color.gray,// Default highlight color
            };
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f); // Set transparency
        }
    }
    public void DestroyCake()
    {
        // Add any destruction animation or effects here
        Destroy(gameObject);
    }
}
