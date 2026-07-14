using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        GameEventHandler.OnPlayerHealthUpdate += UpdateHealth;
    }

    private void OnDisable()
    {
        GameEventHandler.OnPlayerHealthUpdate -= UpdateHealth;
    }

    private void UpdateHealth(float currentHealthPercentage)
    {
        healthSlider.value = currentHealthPercentage;
    }
}
