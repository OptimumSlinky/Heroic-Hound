using UnityEngine;

public class Item : Interactable
{
    protected override void Interact()
    {
        PickUpItem();
    }

    private void PickUpItem()
    {
        Debug.Log("Picked up " + transform.name);

        // Add item to inventory
        Inventory.instance.AddItem(this);

        Destroy(this.gameObject);
    }
}
