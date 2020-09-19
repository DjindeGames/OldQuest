using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrbGauge : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Image gauge;
    [SerializeField]
    private TMP_Text value;
    [SerializeField]
    private TMP_Text max;

    private int gaugeCurrentValue = -1;
    private int gaugeMaxValue = -1;

    public void setValue(int newValue)
    {
        gaugeCurrentValue = newValue;
        value.text = gaugeCurrentValue.ToString();
        refreshGauge();
    }

    public void setMaxValue(int newMaxValue)
    {
        gaugeMaxValue = newMaxValue;
        max.text = gaugeMaxValue.ToString();
        refreshGauge();
    }

    private void refreshGauge()
    {
        gauge.fillAmount = ((1f / gaugeMaxValue) * gaugeCurrentValue);
    }
}
