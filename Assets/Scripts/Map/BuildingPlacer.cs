using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer Instance;

    public GameObject buildingPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlaceBuilding(GridSlot clickedSlot)
    {
        if (clickedSlot.isOccupied)
        {
            Debug.LogWarning("Slot already occupied!");
            return;
        }

        if (buildingPrefab == null)
        {
            Debug.LogError("Building prefab is securely missing in BuildingPlacer!");
            return;
        }

        GameObject newBuilding = Instantiate(buildingPrefab, clickedSlot.transform.position, Quaternion.identity);

        newBuilding.transform.SetParent(clickedSlot.transform);

        clickedSlot.SetOccupied(true);

        Debug.Log($"Successfully built {buildingPrefab.name} at {clickedSlot.gameObject.name}");
    }
}
