using Ore2Lib.Extensions;
using TMPro;
using UnityEngine;

public sealed class GamePlayerView : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro nameLabel = default;
    [SerializeField]
    private SpriteRenderer lifeBar = default;
    [SerializeField]
    private SpriteRenderer currentLifeBar = default;
    [SerializeField]
    private ParticleSystem damageEffect = default;

    private SpriteRenderer body;
    private bool isLocal;

    public void Init(string nickName, bool isLocal, bool isRemote, bool isAlive) {
        nameLabel.text = nickName;
        body = GetComponent<SpriteRenderer>();
        this.isLocal = isLocal;

        var circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = !isRemote;

        if (isLocal) {
            nameLabel.sortingOrder += 10;
        }

        if (isAlive) {
            OnRespawn();
        } else {
            OnDead();
        }
    }

    public void SetCurrentLife(int life) {
        currentLifeBar.transform.SetLocalScaleX((float)life / Config.PlayerLife);
    }

    public void OnRespawn() {
        nameLabel.color = (isLocal) ? Color.yellow : Color.white;
        nameLabel.alpha = 1f;
        lifeBar.enabled = true;
        currentLifeBar.enabled = true;
        body.color = Color.white;
        body.sortingOrder = (isLocal) ? 20 : 10;
        SetCurrentLife(Config.PlayerLife);
    }

    public void OnTakeDamage() {
        damageEffect.Play();
    }

    public void OnDead() {
        nameLabel.color = Color.gray;
        nameLabel.alpha = 0.5f;
        lifeBar.enabled = false;
        currentLifeBar.enabled = false;
        body.color = Color.gray;
        body.sortingOrder = 0;
    }
}
