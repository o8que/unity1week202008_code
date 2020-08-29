using DG.Tweening;
using Ore2Lib.Extensions;
using TMPro;
using UnityEngine;

public sealed class GameRoomDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timeLabel = default;
    [SerializeField]
    private TextMeshProUGUI countdownLabel = default;
    [SerializeField]
    private RectTransform currentLifeBarRectTransform = default;
    [SerializeField]
    private TextMeshProUGUI pointsCounter = default;
    [Header("Perk")]
    [SerializeField]
    private GameRoomDisplayPerk perk1 = default;
    [SerializeField]
    private GameRoomDisplayPerk perk2 = default;
    [SerializeField]
    private GameRoomDisplayPerk perk3 = default;
    [SerializeField]
    private GameRoomDisplayPerk perk4 = default;
    [SerializeField]
    private GameRoomDisplayPerk perk5 = default;
    [SerializeField]
    private GameRoomDisplayPerk perk6 = default;

    private GamePlayer player;
    private int timeValue = 0;
    private int countdownValue = 0;
    private Tween countdownTween;

    public void Init(GamePlayer player) {
        timeLabel.enabled = false;
        countdownLabel.enabled = false;
        perk1.Init(GamePlayerPerk.CountCost, GamePlayerPerk.CountMaxLevel);
        perk2.Init(GamePlayerPerk.FireRateCost, GamePlayerPerk.FireRateMaxLevel);
        perk3.Init(GamePlayerPerk.SpeedCost, GamePlayerPerk.SpeedMaxLevel);
        perk4.Init(GamePlayerPerk.SpreadCost, GamePlayerPerk.SpreadMaxLevel);
        perk5.Init(GamePlayerPerk.PierceCost, GamePlayerPerk.PierceMaxLevel);
        perk6.Init(GamePlayerPerk.BounceCost, GamePlayerPerk.BounceMaxLevel);
        this.player = player;
        countdownTween = countdownLabel.DOScale(1f, 0.5f).SetEase(Ease.InQuad).SetAutoKill(false);

        ResetStatus();
    }

    public void SetTime(int milliseconds) {
        int value = Mathf.CeilToInt(milliseconds / 1000f);

        if (timeValue == value) { return; }
        timeValue = value;

        if (value != 0) {
            timeLabel.enabled = true;
            timeLabel.SetText("{0}", value);
        } else {
            timeLabel.enabled = false;
        }
    }

    public void SetCountdown(int milliseconds) {
        int value = Mathf.CeilToInt(milliseconds / 1000f);
        if (value < 1 || 3 < value) { value = 0; }

        if (countdownValue == value) { return; }
        countdownValue = value;

        if (value != 0) {
            countdownLabel.enabled = true;
            countdownLabel.SetText("{0}", value);

            countdownLabel.rectTransform.SetLocalScale(1.5f, 1.5f, 1.5f);
            countdownTween.Restart();
            GameManager.Audio.PlayCountdownSFX();
        } else {
            countdownLabel.enabled = false;
            GameManager.Audio.PlayStarterSFX();
        }
    }

    public void ResetStatus() {
        currentLifeBarRectTransform.SetLocalScaleX(1f);
        pointsCounter.SetText("{0}", 0);

        perk1.SetLevel(0);
        perk2.SetLevel(0);
        perk3.SetLevel(GamePlayerPerk.SpeedInitLevel);
        perk4.SetLevel(GamePlayerPerk.SpreadInitLevel);
        perk5.SetLevel(0);
        perk6.SetLevel(0);
    }

    public void UpdateStatus() {
        currentLifeBarRectTransform.SetLocalScaleX((float)player.Life / Config.PlayerLife);
        pointsCounter.SetText("{0}", player.Perk.Points);

        perk1.SetLevel(player.Perk.CountLevel);
        perk2.SetLevel(player.Perk.FireRateLevel);
        perk3.SetLevel(player.Perk.SpeedLevel);
        perk4.SetLevel(player.Perk.SpreadLevel);
        perk5.SetLevel(player.Perk.PierceLevel);
        perk6.SetLevel(player.Perk.BounceLevel);
    }

    public void OnClickPerkButton(int perkId) {
        player.OnClickPerkButton(perkId);
    }
}
