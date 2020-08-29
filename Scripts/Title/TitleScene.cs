using Ore2Lib.Scene;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

public sealed class TitleScene : PunScene<TitleScene.With>
{
    public sealed class With : BaseSceneParameter
    {
        public string Message { get; }

        public With(string message = "") : base("Title") {
            Message = message;
        }
    }

    [SerializeField]
    private TMP_InputField nameInput = default;
    [SerializeField]
    private TextMeshProUGUI errorLabel = default;
    [SerializeField]
    private Button playButton = default;
    [SerializeField]
    private TextMeshProUGUI versionLabel = default;
    [SerializeField]
    private Canvas canvas = default;
    [SerializeField]
    private ManualDialog manualDialogPrefab = default;

    protected override async UniTask Enter() {
        nameInput.text = PlayerPrefs.GetString("name", string.Empty);
        errorLabel.text = Parameter.Message;
        versionLabel.text = Config.Version;

        await UniTask.Yield();
    }

    public void OnClickManualButton() {
        Instantiate(manualDialogPrefab, canvas.transform).Open();
        GameManager.Audio.PlayButtonSFX();
    }

    public void OnClickPlayButton() {
        nameInput.interactable = false;
        errorLabel.text = string.Empty;
        playButton.interactable = false;
        GameManager.Audio.PlayButtonSFX();

        PlayerPrefs.SetString("name", nameInput.text);
        PlayerPrefs.Save();

        PhotonNetwork.NickName = string.IsNullOrWhiteSpace(nameInput.text) ? "unknown" : nameInput.text;
        PhotonNetwork.LocalPlayer.SetPlayerProperties(false, (Random.value < 0.5f), 0, 0);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinOrCreateRoom(Config.RoomName, new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom() {
        PhotonNetwork.IsMessageQueueRunning = false;
        GameManager.Scene.NavigateTo(new PlayScene.With());
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        nameInput.interactable = true;
        errorLabel.text = "サーバーへの接続に失敗しました";
        playButton.interactable = true;
    }
}
