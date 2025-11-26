using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Status")]
    public bool hasTowel = false;
    
    [Header("UI References")]
    public GameObject towelIcon; // UI icon to show when player has towel
    
    void Start()
    {
        // Hide towel icon at start
        UpdateTowelUI();
    }
    
    public void PickupTowel()
    {
        hasTowel = true;
        UpdateTowelUI();
        Debug.Log("Picked up towel! CO2 damage will be reduced.");
    }
    
    public void RemoveTowel()
    {
        hasTowel = false;
        UpdateTowelUI();
        Debug.Log("Towel removed from inventory.");
    }
    
    public bool HasTowel()
    {
        return hasTowel;
    }
    
    void UpdateTowelUI()
    {
        if (towelIcon != null)
        {
            towelIcon.SetActive(hasTowel);
        }
    }
}