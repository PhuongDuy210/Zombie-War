using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GrenadeButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private float cooldown = 30f; // seconds
    private bool isOnCooldown = false;

    [SerializeField]
    private Slider cooldownLayer;

    private float cooldownTime;

    private void Start()
    {
        cooldownLayer.gameObject.SetActive(false);
        cooldownLayer.maxValue = cooldown;
    }

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownLayer.value = cooldownTime;
            cooldownTime -= Time.deltaTime;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Grenade clicked");
        if (!isOnCooldown)
        {
            GameEventHandler.ThrowGrenade();
            GameEventHandler.PlaySFX(SFXID.GrenadeThrow);
            StartCoroutine(CooldownRoutine());
        }
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        cooldownTime = cooldown;
        cooldownLayer.gameObject.SetActive(true);

        yield return new WaitForSeconds(cooldown);

        cooldownLayer.gameObject.SetActive(false);
        isOnCooldown = false;
    }
}