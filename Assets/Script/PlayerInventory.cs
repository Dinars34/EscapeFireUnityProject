using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Status")]
    public bool hasTowel = false;
    
    [Header("Keys Inventory")]
    public List<string> keys = new List<string>(); // Store collected keys
    
    [Header("UI References")]
    public GameObject towelIcon; // UI icon to show when player has towel
    public GameObject keyUIPanel; // Panel to show collected keys
    
    void Start()
    {
        // Hide towel icon at start
        UpdateTowelUI();
        
        // Initialize keys list
        if (keys == null)
        {
            keys = new List<string>();
        }
    }
    
    // ========== TOWEL METHODS ==========
    
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
    
    // ========== KEY METHODS ==========
    
    public void AddKey(string keyType)
    {
        if (!keys.Contains(keyType))
        {
            keys.Add(keyType);
            Debug.Log("Added " + keyType + " to inventory. Total keys: " + keys.Count);
            UpdateKeyUI();
        }
        else
        {
            Debug.Log("Already have " + keyType);
        }
    }
    
    public bool HasKey(string keyType)
    {
        return keys.Contains(keyType);
    }
    
    public void RemoveKey(string keyType)
    {
        if (keys.Contains(keyType))
        {
            keys.Remove(keyType);
            Debug.Log("Used " + keyType + ". Removed from inventory.");
            UpdateKeyUI();
        }
    }
    
    public int GetKeyCount()
    {
        return keys.Count;
    }
    
    public List<string> GetAllKeys()
    {
        return new List<string>(keys); // Return a copy
    }
    
    void UpdateKeyUI()
    {
        // This is a basic implementation
        // You can expand this to show key icons in the UI
        if (keyUIPanel != null)
        {
            // Show panel if player has any keys
            keyUIPanel.SetActive(keys.Count > 0);
        }
    }
}