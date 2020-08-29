using UnityEngine;

public class Fishnet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ItemPhysics itemPhysics = other.gameObject.GetComponent<ItemPhysics>();
        Dice dice = other.gameObject.GetComponent<Dice>();
        if (itemPhysics != null)
        {
            itemPhysics.restorePosition();
        }
        if (dice != null)
        {
            dice.restorePosition();
        }
    }
}
