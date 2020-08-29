using Ore2Lib.Scene;
using Photon.Pun;
using UniRx.Async;
using UnityEngine;

public sealed class PlayScene : PunScene<PlayScene.With>
{
    public sealed class With : BaseSceneParameter
    {
        public With() : base("Play") {}
    }

    [SerializeField]
    private GamePlayerContainer playerContainer = default;
    [SerializeField]
    private GameBulletContainer bulletContainer = default;
    [SerializeField]
    private GameRoomDisplay display = default;
    [SerializeField]
    private Leaderboard leaderboard = default;

    private PlayStateMachine stateMachine;

    protected override async UniTask Enter() {
        PhotonNetwork.IsMessageQueueRunning = true;
        var player = PhotonNetwork.Instantiate("GamePlayer", Config.RandomInitialPosition(), Quaternion.identity).GetComponent<GamePlayer>();
        await UniTask.Yield();

        bulletContainer.Init();
        display.Init(player);
        leaderboard.Init(playerContainer);

        stateMachine = new PlayStateMachine(playerContainer, bulletContainer, display, leaderboard);
    }

    protected override void OnUpdate() {
        stateMachine.OnUpdate();
        bulletContainer.OnUpdate();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            GameManager.Scene.NavigateTo(new TitleScene.With());
        } else if (Time.smoothDeltaTime >= 0.4f) {
            GameManager.Scene.NavigateTo(new TitleScene.With("フレームレートが低下しています"));
        } else if (!PhotonNetwork.InRoom) {
            GameManager.Scene.NavigateTo(new TitleScene.With("通信が不安定です"));
        }
    }

    protected override void OnLateUpdate() {
        PhotonNetwork.CurrentRoom.SendCustomProperties();
    }

    protected override async UniTask Exit() {
        stateMachine.Dispose();

        PhotonNetwork.Disconnect();
        await UniTask.WaitWhile(() => PhotonNetwork.IsConnected);
    }
}
