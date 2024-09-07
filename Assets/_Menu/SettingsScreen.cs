using UnityEngine;

public class SettingsScreen : DefaultMenuScreen
{
    private bool isMusicEnabled = true;
    private bool isPoorGraphic = false;

    [SerializeField] private ToggleButton switchMusic, switchGraphic;

    [SerializeField] private AudioSource musicSource;

    public override void OpenScreenLazy()
    {
        switchGraphic.onClick = SwitchGraphic;
        switchMusic.onClick = SwitchMusic;

        base.OpenScreenLazy();
    }


    private void SwitchMusic()
    {
        isMusicEnabled = !isMusicEnabled;
        switchMusic.SwitchButton();

        if (isMusicEnabled)
            musicSource.enabled = true;
        else
        {
            musicSource.enabled = false;
        }
    }

    private void SwitchGraphic()
    {
        isPoorGraphic = !isPoorGraphic;
        switchGraphic.SwitchButton();

        if (isPoorGraphic)
        {
            QualitySettings.masterTextureLimit = 3;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        }
        else
        {
            QualitySettings.masterTextureLimit = 1;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        }
    }
}