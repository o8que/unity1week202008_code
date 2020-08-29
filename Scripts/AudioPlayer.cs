using DG.Tweening;
using Ore2Lib;
using UnityEngine;

public sealed class AudioPlayer : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField]
    private AudioSource sourceBGM = default;
    [SerializeField]
    private AudioSource sourceSFX = default;
    [SerializeField]
    private AudioSourceChannel sourceSelfSFX = default;
    [SerializeField]
    private AudioSourceChannel sourceAttackSFX = default;
    [SerializeField]
    private AudioSourceChannel sourceDamageSFX = default;
    [SerializeField]
    private AudioSourceChannel sourceDeadSFX = default;
    [Header("AudioClip")]
    [SerializeField]
    private AudioClip clipButton = default;
    [SerializeField]
    private AudioClip clipCountdown = default;
    [SerializeField]
    private AudioClip clipStarter = default;
    [SerializeField]
    private AudioClip clipAttack = default;
    [SerializeField]
    private AudioClip clipHit = default;
    [SerializeField]
    private AudioClip clipDamage = default;
    [SerializeField]
    private AudioClip clipDead = default;
    [SerializeField]
    private AudioClip clipPowerup = default;

    private void Start() {
        sourceSelfSFX.SetInterval(60);
        sourceAttackSFX.SetInterval(60);
        sourceDamageSFX.SetInterval(60);
        sourceDeadSFX.SetInterval(60);
    }

    public void PlayBGM() {
        if (!sourceBGM.isPlaying) {
            sourceBGM.volume = 0.4f;
            sourceBGM.Play();
        }
    }

    public void StopBGM(bool immediately = false) {
        if (sourceBGM.isPlaying) {
            if (immediately) {
                sourceBGM.Stop();
            } else {
                sourceBGM.DOFade(0f, 2f).SetEase(Ease.OutCirc).OnComplete(() => sourceBGM.Stop()).Play();
            }
        }
    }

    public void PlayButtonSFX() {
        sourceSFX.PlayOneShot(clipButton);
    }

    public void PlayCountdownSFX() {
        sourceSFX.PlayOneShot(clipCountdown);
    }

    public void PlayStarterSFX() {
        sourceSFX.PlayOneShot(clipStarter);
    }

    public void PlayAttackSFX() {
        sourceAttackSFX.Play(clipAttack, Random.Range(0.95f, 1.05f));
    }

    public void PlayHitSFX() {
        sourceSelfSFX.Play(clipHit);
    }

    public void PlayDamageSFX() {
        sourceDamageSFX.Play(clipDamage, Random.Range(0.95f, 1.05f));
    }

    public void PlayDeadSFX() {
        sourceDeadSFX.Play(clipDead, Random.Range(0.95f, 1.05f));
    }

    public void PlayPowerupSFX() {
        sourceSFX.PlayOneShot(clipPowerup);
    }
}
