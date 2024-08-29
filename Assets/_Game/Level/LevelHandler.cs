using System.Collections;
using TMPro;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    [SerializeField] private LoadingScreen _loadingScreen;

    private const int healthRefreshTime = 3;
    [SerializeField] private Player player;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private HUD hud;

    [SerializeField] private TextMeshProUGUI FPS_text;
    private float deltaTime;

    int totalDamage = 0;

    public float cameraShakeDuration, cameraShakeAmplitude;

    private Coroutine healthRefresher;

    void OnEnable()
    {
        _loadingScreen.instantOpen = true;
        // _loadingScreen.OpenScreen();

        player.onGetDamage += (x) => CameraShake();
        player.onGetDamage += AddDamage;

        Application.targetFrameRate = 60;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        FPS_text.text = $"{1.0f / deltaTime}";
    }

    public void Play()
    {
        player.StartRun();
    }

    private void CameraShake()
    {
        cameraMovement.ShakeCamera(cameraShakeDuration, cameraShakeAmplitude);
        Debug.Log("Shake");
    }

    private void AddDamage(int amaount)
    {
        if (totalDamage + amaount < 2)
        {
            totalDamage++;

            if (healthRefresher != null)
            {
                StopCoroutine(healthRefresher);
                healthRefresher = null;
            }

            player.SetDamageFX(true);

            healthRefresher = StartCoroutine(HealthRefresh());
        }
        else
        {
            player.onDeath?.Invoke(hud.ShowTryAgainScreen);
        }
    }

    IEnumerator HealthRefresh()
    {
        yield return new WaitForSeconds(healthRefreshTime);

        totalDamage = 0;
        player.SetDamageFX(false);

        yield break;
    }
}