using System.Collections.Generic;
using Ore2Lib;
using TMPro;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

public sealed class Leaderboard : BaseDialog
{
    [SerializeField]
    private TextMeshProUGUI messageLabel = default;
    [SerializeField]
    private ScrollRect team1 = default;
    [SerializeField]
    private ScrollRect team2 = default;

    private Canvas canvas;
    private GamePlayerContainer playerContainer;

    private readonly List<LeaderboardEntry> team1List = new List<LeaderboardEntry>(50);
    private readonly List<LeaderboardEntry> team2List = new List<LeaderboardEntry>(50);

    public void Init(GamePlayerContainer playerContainer) {
        canvas = GetComponent<Canvas>();
        this.playerContainer = playerContainer;

        foreach (Transform child in team1.content) { team1List.Add(child.GetComponent<LeaderboardEntry>()); }
        foreach (Transform child in team2.content) { team2List.Add(child.GetComponent<LeaderboardEntry>()); }

        canvas.enabled = false;
    }

    public void Open(int winningTeam) {
        if (winningTeam < 0) {
            messageLabel.text = "<color=#ff4000>チーム1</color>の勝利です！";
        } else if (winningTeam > 0) {
            messageLabel.text = "<color=#00c0ff>チーム2</color>の勝利です！";
        } else {
            messageLabel.text = "他のプレイヤーの参加を待っています...";
        }

        playerContainer.Sort();
        int i1 = 0, i2 = 0;
        for (int i = 0; i < playerContainer.Count; i++) {
            var player = playerContainer[i];
            if (!player.Team) {
                team1List[i1++].Show(player.NickName, player.IsLocal, player.Kill, player.Score);
            } else {
                team2List[i2++].Show(player.NickName, player.IsLocal, player.Kill, player.Score);
            }
        }
        for (int i = i1; i < team1List.Count; i++) { team1List[i].Hide(); }
        for (int i = i2; i < team2List.Count; i++) { team2List[i].Hide(); }

        if (!canvas.enabled) {
            team1.verticalNormalizedPosition = 1f;
            team2.verticalNormalizedPosition = 1f;
            canvas.enabled = true;
            Show().Forget();
        }
    }

    public void Close() {
        if (canvas.enabled) {
            CloseAsync().Forget();
        }

        async UniTaskVoid CloseAsync() {
            await Hide(false);
            canvas.enabled = false;
        }
    }
}
