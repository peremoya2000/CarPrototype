using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SpeedometerUI : MonoBehaviour
{
    [SerializeField] private FloatProvider speedGetter;
    [SerializeField] private TMP_Text tmp;

    void Update()
    {
        tmp.text = $"{speedGetter.GetData().ToString("000")} kph";
    }
}
