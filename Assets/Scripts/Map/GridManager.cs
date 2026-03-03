using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 5;
    public int height = 4;
    public float cellSize = 1.1f; // Cell gap

    [Header("References")]
    public GameObject slotPrefab;

    // 2D array store Gridslot
    private GridSlot[,] gridArray;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        gridArray = new GridSlot[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate spawn position
                Vector2 spawnPosition = new Vector2(x * cellSize, y * cellSize);

                GameObject spawnedSlotObj = Instantiate(slotPrefab, spawnPosition, Quaternion.identity);

                spawnedSlotObj.name = $"Slot_{x}_{y}";

                spawnedSlotObj.transform.SetParent(this.transform);

                GridSlot slotScript = spawnedSlotObj.GetComponent<GridSlot>();
                gridArray[x, y] = slotScript;
            }
        }
    }

    //Get information gridslot
    public GridSlot GetSlotAt(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return gridArray[x, y];
        }
        return null;
    }
}
