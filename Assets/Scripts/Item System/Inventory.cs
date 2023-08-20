using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    [SerializeField] private List<Item> _inventory = new List<Item>();

    private void Awake()
    {
        instance = this;
    }

    public void AddItem(Item newItem)
    {
        _inventory.Add(newItem);
    }
    
    public void RemoveItem(Item item) 
    { 
        if (_inventory.Count > 0) 
        { 
            _inventory.Remove(item);
        }
    }
}
