using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GenericPopup : MonoBehaviour
{
    //[SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject defaultButtonPrefab;
    [SerializeField] private Button nextPageBtn;
    [SerializeField] private Button prevPageBtn;

    private List<GenericAnimation> anims = new List<GenericAnimation>();
    private bool isShown = false;
    private int currentPageIndex = 0;
    private List<string> popupPages = new List<string>();

    private void Awake()
    {
        if (anims.Count == 0)
        {
            GetGenericAnimations(gameObject);
        }

        if (nextPageBtn != null)
        {
            nextPageBtn.onClick.AddListener(NextPage);
        }
        if (prevPageBtn != null)
        {
            prevPageBtn.onClick.AddListener(PrevPage);
        }

        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        GameEventHandler.PauseGame();
        if (anims.Count == 0)
        {
            GetGenericAnimations(gameObject);
        }

        foreach (var anim in anims)
        {
            anim.AnimateEntry();
        }

        isShown = true;
    }

    public void Show(string title, object content, List<PopupButtonData> buttons,
                     List<AnimationType> entryAnimations = null,
                     List<AnimationType> exitAnimations = null)
    {
        gameObject.SetActive(true);
        GameEventHandler.PauseGame();
        if (anims.Count == 0)
        {
            GetGenericAnimations(gameObject);
        }

        if (nextPageBtn != null)
            nextPageBtn.gameObject.SetActive(false);

        if (prevPageBtn != null)
            prevPageBtn.gameObject.SetActive(false);

        //titleText.text = title;
        popupPages.Clear();

        if (content is string strContent)
        {
            messageText.text = strContent;
        }
        else if (content is List<PopupPage> pages)
        {
            currentPageIndex = 0;
            foreach (var page in pages)
            {
                if (popupPages.Count <= page.PageNumber)
                {
                    popupPages.Add(page.Content);
                }
                else
                {
                    popupPages[page.PageNumber] = page.Content;
                }
            }

            messageText.text = popupPages[currentPageIndex];

            if (popupPages.Count > 1)
            {
                nextPageBtn.gameObject.SetActive(true);
                prevPageBtn.gameObject.SetActive(true);

                nextPageBtn.interactable = true;
                prevPageBtn.interactable = false;
            }
        }

        if (entryAnimations != null && entryAnimations.Count > 0)
        {
            for (int i = 0; i < anims.Count && i < entryAnimations.Count; i++)
            {
                anims[i].SetEntryAnimation(entryAnimations[i]);
            }
        }

        if (exitAnimations != null && exitAnimations.Count > 0)
        {
            for (int i = 0; i < anims.Count && i < exitAnimations.Count; i++)
            {
                anims[i].SetExitAnimation(exitAnimations[i]);
            }
        }

        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var btnData in buttons)
        {
            AddButton(btnData);
        }

        foreach (var anim in anims)
        {
            anim.AnimateEntry();
        }

        isShown = true;
    }

    public void SetPopupScale(Vector3 scale)
    {
        transform.localScale = scale;
    }

    public void Close()
    {
        foreach (var anim in anims)
        {
            anim.AnimateExit(() => { gameObject.SetActive(false); });
        }
        isShown = false;
        //GameEventHandler.ResumeGame();
    }

    private void GetGenericAnimations(GameObject root)
    {
        var anim = root.GetComponent<GenericAnimation>();
        if (anim != null)
        {
            anims.Add(anim);
        }

        foreach (Transform child in root.transform)
        {
            GetGenericAnimations(child.gameObject);
        }
    }

    public bool Shown() => isShown;

    private void NextPage()
    {
        currentPageIndex++;
        messageText.text = popupPages[currentPageIndex];

        prevPageBtn.interactable = false;
        nextPageBtn.interactable = false;

        if ((currentPageIndex + 1) < popupPages.Count)
        {
            nextPageBtn.interactable = true;
        }
        if (currentPageIndex > 0)
        {
            prevPageBtn.interactable = true;
        }

        // Play SFX here if you have an audio manager
    }

    private void PrevPage()
    {
        currentPageIndex--;
        messageText.text = popupPages[currentPageIndex];

        prevPageBtn.interactable = false;
        nextPageBtn.interactable = false;

        if (currentPageIndex > 0)
        {
            prevPageBtn.interactable = true;
        }
        if (popupPages.Count > 1 && currentPageIndex < popupPages.Count)
        {
            nextPageBtn.interactable = true;
        }

        // Play SFX here if you have an audio manager
    }

    private void AddButton(PopupButtonData btnData, int? index = null)
    {
        var prefab = btnData.ButtonPrefabOverride ?? defaultButtonPrefab;
        var btnObj = Instantiate(prefab);

        if (index.HasValue)
        {
            btnObj.transform.SetSiblingIndex(index.Value);
        }
        btnObj.transform.SetParent(buttonContainer, false);

        var label = btnObj.GetComponentInChildren<TMP_Text>();
        if (label != null) label.text = btnData.Label;

        var button = btnObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                btnData.Callback?.Invoke(); // Action invoked here
                foreach (var anim in anims)
                {
                    anim.AnimateExit();
                }
            });
        }
    }
}
