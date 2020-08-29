using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameRoomDisplayPerk : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI costLabel = default;
    [SerializeField]
    private RectTransform levelRectTransform = default;
    [SerializeField]
    private Image levelImage = default;

    private readonly List<Image> imageList = new List<Image>(12);
    private int currentLevel = 0;

    public void Init(int cost, int maxLevel) {
        costLabel.SetText("コスト: {0}", cost);

        imageList.Add(levelImage);
        for (int i = 1; i < maxLevel; i++) {
            imageList.Add(Instantiate(levelImage, levelRectTransform));
        }
    }

    public void SetLevel(int level) {
        if (currentLevel == level) { return; }
        currentLevel = level;

        for (int i = 0; i < imageList.Count; i++) {
            imageList[i].color = (i < level) ? Color.yellow : new Color(0.25f, 0.25f, 0f, 1f);
        }
    }
}
