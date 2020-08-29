using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyMaterial { Iron, Bronze, Gold }

[CreateAssetMenu]
public class Key : Item
{
    public KeyMaterial material;
}
