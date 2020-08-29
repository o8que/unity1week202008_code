using Photon.Pun;
using UnityEngine;

public sealed class ResultState : IPlayState
{
    private readonly PlayStateMachine stateMachine;
    private readonly GamePlayerContainer playerContainer;
    private readonly GameBulletContainer bulletContainer;
    private readonly GameRoomDisplay display;
    private readonly Leaderboard leaderboard;

    private float timer;
    private int winningTeam;

    public ResultState(PlayStateMachine stateMachine, GamePlayerContainer playerContainer, GameBulletContainer bulletContainer, GameRoomDisplay display, Leaderboard leaderboard) {
        this.stateMachine = stateMachine;
        this.playerContainer = playerContainer;
        this.bulletContainer = bulletContainer;
        this.display = display;
        this.leaderboard = leaderboard;
    }

    void IPlayState.Enter() {
        timer = 0f;
        (int team1, int team2) = playerContainer.CountAlivePlayers();
        winningTeam = team2 - team1;
    }

    void IPlayState.OnUpdate() {
        int timestamp = PhotonNetwork.ServerTimestamp;
        int startTime = PhotonNetwork.CurrentRoom.GetStartTime();
        int remainingTime = unchecked(startTime - timestamp);

        bool hasStartTime = (startTime != 0);
        bool hasSetNextTime = (remainingTime > -Config.ReadyTime);
        bool canStart = (hasStartTime && hasSetNextTime);

        timer += Time.deltaTime;
        if (timer >= 2f) {
            timer = 0f;

            leaderboard.Open(winningTeam);

            if (PhotonNetwork.IsMasterClient && !canStart) {
                PhotonNetwork.CurrentRoom.SetStartTime(unchecked(timestamp + Config.ResultPeriod));
            }
        }

        display.SetTime((canStart) ? remainingTime : 0);

        if (canStart && remainingTime <= 0) {
            stateMachine.ChangeNextState();
        }
    }

    void IPlayState.Exit() {
        display.SetTime(0);
        leaderboard.Close();

        if (PhotonNetwork.IsMasterClient) {
            playerContainer.MakeUpWithBots(Config.BotCount);
        }

        playerContainer.RespawnAll();
        bulletContainer.Clear();
        display.ResetStatus();
    }

    void IPlayState.Dispose() {}
}
