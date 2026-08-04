using TMPro;
using UnityEngine;

public class SurvivalTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private float elapsedTime;

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        timerText.text = $"Time: {elapsedTime:F1}";
    }
}