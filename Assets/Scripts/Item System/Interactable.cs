using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] protected float interactRadius = 0.75f;
    [SerializeField] protected bool hasInteracted = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < interactRadius)
        {
            Interact();
            hasInteracted = true;
        }
    }

    protected virtual void Interact()
    {
        Debug.Log("Interacted with" + transform.name);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactRadius);  
    }
}
