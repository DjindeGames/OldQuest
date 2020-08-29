using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickering : MonoBehaviour
{
    [Header("Flickering parameters")]
    //How far the light's position can go from its base position
    [SerializeField]
    [Range(0, 1)]
    private float positionRange;
    //The maximum quantity of movement per frame, should be lower than range
    [SerializeField]
    [Range(0, 1)]
    private float positionStep;
    //How bright can the light shine from its base intensity
    [SerializeField]
    [Range(0, 5)]
    private float intensityRange;
    //The maximum quantity of intensity added  or removed per frame
    [SerializeField]
    [Range(0, 1f)]
    private float intensityStep;
    private Light flame;
    Vector3 basePosition;
    float baseIntensity;

    void Awake()
    {
        flame = GetComponent<Light>();
        basePosition = transform.position;
        baseIntensity = flame.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = basePosition;
        float newIntensity = baseIntensity;

        //Security to avoid infinite loops
        if (positionStep < positionRange)
        {
            do
            {
                newPosition = new Vector3(transform.position.x + Random.Range(-positionStep, positionStep), basePosition.y, transform.position.z + Random.Range(-positionStep, positionStep));
            } while (Vector3.Distance(newPosition, basePosition) > positionRange);
        }

        //Security to avoid infinite loops
        if (intensityStep < intensityRange)
        {
            do
            {
                newIntensity = flame.intensity + Random.Range(-intensityStep, intensityStep);
            } while (System.Math.Abs(newIntensity - baseIntensity) > intensityRange);
        }

        flame.intensity = newIntensity;
        transform.position = newPosition;
    }
}
