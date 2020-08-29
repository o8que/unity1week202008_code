using BehaviorDesigner.Runtime;
using Photon.Pun;

public sealed class ActionState : IPlayState
{
    private readonly PlayStateMachine stateMachine;
    private readonly GamePlayerContainer playerContainer;
    private readonly GameRoomDisplay display;

    private int previousAddPointTime;

    public ActionState(PlayStateMachine stateMachine, GamePlayerContainer playerContainer, GameRoomDisplay display) {
        this.stateMachine = stateMachine;
        this.playerContainer = playerContainer;
        this.display = display;
    }

    void IPlayState.Enter() {
        previousAddPointTime = Config.AddPointInterval;

        playerContainer.ActivateAll();
        GameManager.Audio.PlayBGM();
    }

    void IPlayState.OnUpdate() {
        int elapsedTime = unchecked(PhotonNetwork.ServerTimestamp - PhotonNetwork.CurrentRoom.GetStartTime());
        int remainingTime = Config.ActionTime - elapsedTime;

        // 時間経過でポイント付与
        int addPointTime = remainingTime % Config.AddPointInterval;
        if (addPointTime > previousAddPointTime) {
            playerContainer.AddPoint();
        }
        previousAddPointTime = addPointTime;

        // ボットAIを更新
        if (PhotonNetwork.IsMasterClient) {
            BehaviorManager.instance.Tick();
        }

        playerContainer.OnUpdate();
        display.SetTime(remainingTime);
        display.UpdateStatus();

        (int team1, int team2) = playerContainer.CountAlivePlayers();
        if (remainingTime <= 0 || team1 <= Config.MinPlayerCount || team2 <= Config.MinPlayerCount) {
            stateMachine.ChangeNextState();
        }
    }

    void IPlayState.Exit() {
        playerContainer.DeactivateAll();
        display.SetTime(0);
        GameManager.Audio.StopBGM();
    }

    void IPlayState.Dispose() {
        GameManager.Audio.StopBGM(true);
    }
}
