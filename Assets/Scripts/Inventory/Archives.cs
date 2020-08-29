using UnityEngine;

public class Archives : MonoBehaviour
{
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ScreenManager.Instance.switchScreen(ScreenType.Archives);
        }
    }
}
