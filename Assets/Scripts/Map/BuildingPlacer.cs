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
            Debug.Log("Ô này đã có công trình, không thể xây đè!");
            return;
        }

        if (buildingPrefab == null)
        {
            Debug.LogError("Chưa gán prefab nhà vào BuildingPlacer!");
            return;
        }

        GameObject newBuilding = Instantiate(buildingPrefab, clickedSlot.transform.position, Quaternion.identity);

        newBuilding.transform.SetParent(clickedSlot.transform);

        clickedSlot.SetOccupied(true);

        Debug.Log($"Thành công: Đã xây {buildingPrefab.name} tại {clickedSlot.gameObject.name}");
    }
}
