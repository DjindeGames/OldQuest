using UnityEngine;

namespace Djinde.Quest
{
    public class Archives : MonoBehaviour
    {
        void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ScreenManager.Instance.switchScreen(EScreenType.Archives);
            }
        }
    }
}