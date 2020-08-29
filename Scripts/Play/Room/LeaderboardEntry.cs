using TMPro;
using UnityEngine;

public sealed class LeaderboardEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameLabel = default;
    [SerializeField]
    private TextMeshProUGUI killLabel = default;
    [SerializeField]
    private TextMeshProUGUI scoreLabel = default;

    public void Show(string nickName, bool isLocal, int kill, int score) {
        nameLabel.text = nickName;
        killLabel.SetText("{0}", kill);
        scoreLabel.SetText("{0}", score);

        var color = (isLocal) ? Color.yellow : Color.white;
        nameLabel.color = color;
        killLabel.color = color;
        scoreLabel.color = color;

        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
        }
    }

    public void Hide() {
        if (gameObject.activeSelf) {
            gameObject.SetActive(false);
        }
    }
}
