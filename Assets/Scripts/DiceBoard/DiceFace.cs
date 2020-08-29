using UnityEngine;

public class DiceFace : MonoBehaviour
{
    [SerializeField]
    [Header("Settings")]
    private DiceValue value;

    private Dice dice;

    void Awake()
    {
        dice = transform.parent.GetComponent<Dice>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Constants.DiceBoardTag)
        {
            dice.notifyFaceDown(value);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Constants.DiceBoardTag)
        {
            dice.notifyFaceUp(value);
        }
    }
}
