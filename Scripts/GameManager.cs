using DG.Tweening;
using Ore2Lib;
using Ore2Lib.Scene;
using Photon.Pun;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    [SerializeField]
    private SceneNavigator sceneNavigator = default;
    [SerializeField]
    private AudioPlayer audioPlayer = default;

    public static SceneNavigator Scene { get; private set; }
    public static AudioPlayer Audio { get; private set; }
    public static GameScreenCamera Screen { get; private set; }

    private void Awake() {
        Application.targetFrameRate = 60;
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 5;
        DOTween.Init();

        Scene = sceneNavigator;
        Audio = audioPlayer;
        Screen = GameObject.FindWithTag(TagName.MainCamera).GetComponent<GameScreenCamera>();
    }

    private void Start() {
        Scene.LaunchFrom(new TitleScene.With());
    }
}
