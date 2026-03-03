using UnityEngine;

public class GridSlot : MonoBehaviour
{
    public bool isOccupied = false;
    private SpriteRenderer spriteRenderer;

    [Header("Visual Colors")]
    public Color emptyColor = Color.green;
    public Color occupiedColor = Color.gray;
    public Color hoverColor = Color.yellow; 

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }


    private void OnMouseEnter()
    {
        if (!isOccupied && spriteRenderer != null)
        {
            spriteRenderer.color = hoverColor;
        }
    }

    private void OnMouseExit()
    {
        UpdateVisual();
    }

    private void OnMouseDown()
    {
        if (!isOccupied)
        {
            Debug.Log($"Đã click vào ô trống: {gameObject.name}");

           
            SetOccupied(true);
        }
        else
        {
            Debug.Log($"Ô {gameObject.name} đã có nhà rồi!");
        }
    }


    public void SetOccupied(bool status)
    {
        isOccupied = status;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isOccupied ? occupiedColor : emptyColor;
        }
    }
}

