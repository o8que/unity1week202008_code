using Photon.Pun;

public sealed class ReadyState : IPlayState
{
    private readonly PlayStateMachine stateMachine;
    private readonly GameRoomDisplay display;

    public ReadyState(PlayStateMachine stateMachine, GameRoomDisplay display) {
        this.stateMachine = stateMachine;
        this.display = display;
    }

    void IPlayState.Enter() {}

    void IPlayState.OnUpdate() {
        int elapsedTime = unchecked(PhotonNetwork.ServerTimestamp - PhotonNetwork.CurrentRoom.GetStartTime());
        int remainingTime = Config.ReadyTime - elapsedTime;

        display.SetCountdown(remainingTime);

        if (remainingTime <= 0) {
            stateMachine.ChangeNextState();
        }
    }

    void IPlayState.Exit() {
        display.SetCountdown(0);
    }

    void IPlayState.Dispose() {}
}
