using Photon.Pun;

public sealed class PlayStateMachine
{
    private readonly ReadyState readyState;
    private readonly ActionState actionState;
    private readonly ResultState resultState;

    private IPlayState currentState;

    public PlayStateMachine(GamePlayerContainer playerContainer, GameBulletContainer bulletContainer, GameRoomDisplay display, Leaderboard leaderboard) {
        readyState = new ReadyState(this, display);
        actionState = new ActionState(this, playerContainer, display);
        resultState = new ResultState(this, playerContainer, bulletContainer, display, leaderboard);

        int startTime = PhotonNetwork.CurrentRoom.GetStartTime();
        int elapsedTime = unchecked(PhotonNetwork.ServerTimestamp - startTime);

        if (startTime == 0 || elapsedTime < 0) {
            currentState = resultState;
        } else if (elapsedTime < Config.ReadyTime) {
            currentState = readyState;
        } else {
            currentState = actionState;
        }

        currentState.Enter();
    }

    public void OnUpdate() {
        currentState.OnUpdate();
    }

    public void ChangeNextState() {
        currentState.Exit();

        switch (currentState) {
        case ResultState _:
            currentState = readyState;
            break;
        case ReadyState _:
            currentState = actionState;
            break;
        default:
            currentState = resultState;
            break;
        }

        currentState.Enter();
    }

    public void Dispose() {
        currentState.Dispose();
    }
}
