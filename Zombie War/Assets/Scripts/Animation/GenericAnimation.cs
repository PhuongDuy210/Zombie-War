using UnityEngine;
using System.Collections;

public enum AnimationType
{
    FlyTop,
    FlyBottom,
    FlyLeft,
    FlyRight,
    Scale,
    Fade
}

public class GenericAnimation : MonoBehaviour
{
    [SerializeField] private float animationDuration = 1.25f;

    [SerializeField] private AnimationType entryAnimationType = AnimationType.FlyTop;
    [SerializeField] private AnimationType exitAnimationType = AnimationType.FlyTop;

    [SerializeField] private Vector3 onscreenTarget = Vector3.zero;

    [SerializeField] private float initialOpacity = 1f; // 0–1 range

    [SerializeField] private Vector3 intialScale = Vector3.one;

    private Vector3 originalPos;
    private bool looping = false;
    private bool isShown = false;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        originalPos = transform.localPosition;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void AnimateEntry(System.Action onComplete = null)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();

        switch (entryAnimationType)
        {
            case AnimationType.FlyBottom:
                transform.localPosition = new Vector3(0, -Screen.height, 0);
                StartCoroutine(AnimatePosition(onscreenTarget, animationDuration, onComplete, true));
                break;

            case AnimationType.FlyTop:
                transform.localPosition = new Vector3(0, Screen.height, 0);
                StartCoroutine(AnimatePosition(onscreenTarget, animationDuration, onComplete, true));
                break;

            case AnimationType.FlyLeft:
                transform.localPosition = new Vector3(-Screen.width, 0, 0);
                StartCoroutine(AnimatePosition(onscreenTarget, animationDuration, onComplete, true));
                break;

            case AnimationType.FlyRight:
                transform.localPosition = new Vector3(Screen.width, 0, 0);
                StartCoroutine(AnimatePosition(onscreenTarget, animationDuration, onComplete, true));
                break;

            case AnimationType.Scale:
                transform.localScale = Vector3.zero;
                StartCoroutine(AnimateScale(intialScale, animationDuration, onComplete, true));
                break;

            case AnimationType.Fade:
                canvasGroup.alpha = 0f;
                StartCoroutine(AnimateFade(initialOpacity, animationDuration, onComplete, true));
                break;
        }

        isShown = true;
    }

    public void AnimateExit(System.Action onComplete = null)
    {
        StopAllCoroutines();

        switch (exitAnimationType)
        {
            case AnimationType.FlyBottom:
                StartCoroutine(AnimatePosition(new Vector3(0, -Screen.height, 0), animationDuration, () =>
                {
                    onComplete?.Invoke();
                    if (!looping) gameObject.SetActive(false);
                }, false));
                break;

            case AnimationType.FlyTop:
                StartCoroutine(AnimatePosition(new Vector3(0, Screen.height, 0), animationDuration, () =>
                {
                    onComplete?.Invoke();
                    if (!looping) gameObject.SetActive(false);
                }, false));
                break;

            case AnimationType.FlyLeft:
                StartCoroutine(AnimatePosition(new Vector3(-Screen.width, 0, 0), animationDuration, () =>
                {
                    onComplete?.Invoke();
                    if (!looping) gameObject.SetActive(false);
                }, false));
                break;

            case AnimationType.FlyRight:
                StartCoroutine(AnimatePosition(new Vector3(Screen.width, 0, 0), animationDuration, () =>
                {
                    onComplete?.Invoke();
                    if (!looping) gameObject.SetActive(false);
                }, false));
                break;

            case AnimationType.Scale:
                StartCoroutine(AnimateScale(Vector3.zero, animationDuration, () =>
                {
                    onComplete?.Invoke();
                    if (!looping) gameObject.SetActive(false);
                }, false));
                break;

            case AnimationType.Fade:
                StartCoroutine(AnimateFade(0f, animationDuration, () =>
                {
                    onComplete?.Invoke();
                    if (!looping) gameObject.SetActive(false);
                }, false));
                break;
        }

        isShown = false;
    }

    public void LoopAnimation()
    {
        looping = true;
        void RunCycle()
        {
            if (!looping) return;
            AnimateEntry(() =>
            {
                AnimateExit(() =>
                {
                    RunCycle();
                });
            });
        }
        RunCycle();
    }

    public void StopLoop()
    {
        looping = false;
        gameObject.SetActive(false);
    }

    // Helpers
    private IEnumerator AnimatePosition(Vector3 target, float duration, System.Action onComplete, bool easeOut)
    {
        Vector3 start = transform.localPosition;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            if (easeOut) progress = Mathf.Sin(progress * Mathf.PI * 0.5f); // easeOut
            else progress = 1f - Mathf.Cos(progress * Mathf.PI * 0.5f);    // easeIn
            transform.localPosition = Vector3.Lerp(start, target, progress);
            yield return null;
        }
        transform.localPosition = target;
        onComplete?.Invoke();
    }

    private IEnumerator AnimateScale(Vector3 target, float duration, System.Action onComplete, bool easeOut)
    {
        Vector3 start = transform.localScale;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            if (easeOut) progress = Mathf.Sin(progress * Mathf.PI * 0.5f);
            else progress = 1f - Mathf.Cos(progress * Mathf.PI * 0.5f);
            transform.localScale = Vector3.Lerp(start, target, progress);
            yield return null;
        }
        transform.localScale = target;
        onComplete?.Invoke();
    }

    private IEnumerator AnimateFade(float targetAlpha, float duration, System.Action onComplete, bool easeOut)
    {
        float start = canvasGroup.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            if (easeOut) progress = Mathf.Sin(progress * Mathf.PI * 0.5f);
            else progress = 1f - Mathf.Cos(progress * Mathf.PI * 0.5f);
            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha, progress);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        onComplete?.Invoke();
    }

    // Utility setters
    public void SetEntryAnimation(AnimationType type) => entryAnimationType = type;
    public void SetExitAnimation(AnimationType type) => exitAnimationType = type;
    public void SetOnscreenTarget(Vector3 target) => onscreenTarget = target;
    public void SetTargetAsOriginalPos() => onscreenTarget = originalPos;
    public void SetInitialOpacity(float opacity) => initialOpacity = opacity;
}
