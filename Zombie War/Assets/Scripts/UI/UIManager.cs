using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private GenericPopup genericPopup;

    [SerializeField]
    private TMP_Text killCountText;
    private int killCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        GameEventHandler.OnPlayerHealthUpdate += UpdateHealth;
        GameEventHandler.OnEnemyKilled += UpdateKillCount;
        GameEventHandler.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        GameEventHandler.OnPlayerHealthUpdate -= UpdateHealth;
        GameEventHandler.OnEnemyKilled -= UpdateKillCount;
        GameEventHandler.OnGameOver -= GameOver;
    }

    private void UpdateKillCount()
    {
        killCount++;
        killCountText.text = "Kill Count: " + killCount.ToString();
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
