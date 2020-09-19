using System.Collections;
using UnityEngine;

public class OneShotAudioSource : MonoBehaviour
{
    private AudioSource source;
    private bool ready = false;

    void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine(waitABit());
    }

    private void Update()
    {
        if (ready)
        {
            if (!source.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator waitABit()
    {
        yield return new WaitForSeconds(1);
        ready = true;
    }
}
