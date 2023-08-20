using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private int _capacity = 4;
    [SerializeField] private List<Item> _inventory = new List<Item>();

    private void Awake()
    {
        instance = this;
    }

    public void AddItem(Item newItem)
    {
        if (_inventory.Count < _capacity)
        {
            _inventory.Add(newItem);
        }
    }

    public void RemoveItem(Item item)
    {
        if (_inventory.Count > 0)
        {
            _inventory.Remove(item);
        }
    }
}
