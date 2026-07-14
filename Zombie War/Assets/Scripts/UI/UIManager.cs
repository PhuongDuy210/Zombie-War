using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private GenericPopup genericPopup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        GameEventHandler.OnPlayerHealthUpdate += UpdateHealth;
        GameEventHandler.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        GameEventHandler.OnPlayerHealthUpdate -= UpdateHealth;
        GameEventHandler.OnGameOver -= GameOver;
    }

    private void UpdateHealth(float currentHealthPercentage)
    {
        healthSlider.value = currentHealthPercentage;
    }

    private void GameOver(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Win:
                OpenWinPopup(); break;

            case GameState.Lose:
                OpenLosePopup(); break;

            default: break;
        }
    }

    private void OpenWinPopup()
    {
        if (genericPopup != null && !genericPopup.Shown())
        {
            string content = "MISSION ACCOMPLISHED";
            List<PopupButtonData> popupButtonDatas = new List<PopupButtonData>();
            popupButtonDatas.Add(new PopupButtonData("NEXT", () => { GameEventHandler.NextLevel(); }));
            genericPopup.Show("", content, popupButtonDatas);
        }
    }

    private void OpenLosePopup()
    {
        if (genericPopup != null && !genericPopup.Shown())
        {
            string content = "MISSION FAILED";
            List<PopupButtonData> popupButtonDatas = new List<PopupButtonData>();
            popupButtonDatas.Add(new PopupButtonData("RETRY", () => { GameEventHandler.RetryLevel(); }));
            genericPopup.Show("", content, popupButtonDatas);
        }
    }
}
