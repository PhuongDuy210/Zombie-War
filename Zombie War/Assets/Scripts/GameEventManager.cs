using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventHandler : MonoBehaviour
{
    public static event Action OnGameStart;

	public static event Action OnGamePause;

	public static event Action<GameState> OnGameOver;

    public static event Action<SFXID> OnSFXPlay;

    public static event Action OnShootButtonDown;

    public static event Action OnShootButtonUp;

    public static event Action OnSwitchButtonDown;

    public static event Action OnGrenadeButtonDown;

    public static event Action<float> OnPlayerHealthUpdate;

    public static event Action OnEnemyKilled;

    public static void StartGame() => OnGameStart?.Invoke();
    public static void PauseGame() => OnGamePause?.Invoke();
    public static void EndGame(GameState state) => OnGameOver?.Invoke(state);    
    public static void PlaySFX(SFXID sfxId) =>
        OnSFXPlay?.Invoke(sfxId);
    public static void StartShooting() => OnShootButtonDown?.Invoke();
    public static void StopShooting() => OnShootButtonUp?.Invoke();
    public static void SwitchWeapon() => OnSwitchButtonDown?.Invoke();
    public static void ThrowGrenade() => OnGrenadeButtonDown?.Invoke();
    public static void UpdatePlayerHealth(float percentage) => OnPlayerHealthUpdate?.Invoke(percentage);
    public static void EnemyKilled() => OnEnemyKilled?.Invoke();
}
