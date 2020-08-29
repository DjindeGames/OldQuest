using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Ceiling : MonoBehaviour
{
    void Awake()
    {
        GetComponent<BoxCollider>().enabled = true;
        Destroy(this);
    }
}
