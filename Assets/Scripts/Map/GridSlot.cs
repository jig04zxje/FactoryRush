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
        if (GameStateManager.Instance != null && GameStateManager.Instance.State != GameState.Playing) return;

        if (!isOccupied && spriteRenderer != null)
        {
            spriteRenderer.color = hoverColor;
        }
    }

    private void OnMouseExit()
    {
        UpdateVisual();
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

