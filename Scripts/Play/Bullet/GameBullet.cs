using UnityEngine;

public sealed class GameBullet : MonoBehaviour
{
    private GameBulletContainer bulletContainer;
    private SpriteRenderer body;
    private CircleCollider2D circleCollider;
    private Vector3 velocity;
    private int life;
    private int bounce;

    public int Id { get; private set; }
    public int OwnerId { get; private set; }
    public bool Team { get; private set; }
    public bool Equals(int id, int ownerId) => Id == id && OwnerId == ownerId;

    public GameBullet Init(GameBulletContainer bulletContainer) {
        this.bulletContainer = bulletContainer;
        body = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        Deactivate();
        return this;
    }

    public void OnUpdate() {
        var position = transform.position + velocity * Time.deltaTime;
        if (position.x < Config.BoundLeft || Config.BoundRight < position.x) {
            bulletContainer.Remove(Id, OwnerId);
        } else if (position.y < Config.BoundBottom) {
            if (bounce > 0) {
                bounce--;
                velocity.y = -velocity.y;
                position.y = Config.BoundBottom - (position.y - Config.BoundBottom);
                transform.position = position;
            } else {
                bulletContainer.Remove(Id, OwnerId);
            }
        } else if (Config.BoundTop < position.y) {
            if (bounce > 0) {
                bounce--;
                velocity.y = -velocity.y;
                position.y = Config.BoundTop - (position.y - Config.BoundTop);
                transform.position = position;
            } else {
                bulletContainer.Remove(Id, OwnerId);
            }
        } else {
            transform.position = position;
        }
    }

    public void Activate(int id, int ownerId, bool team, Vector3 origin, int lag, float speed, float angle, int life, int bounce) {
        body.color = (!team) ? new Color32(255, 64, 0, 255) : new Color32(0, 192, 255, 255);
        body.enabled = true;
        circleCollider.enabled = true;

        velocity = new Vector3(speed * Mathf.Cos(angle * Mathf.Deg2Rad), speed * Mathf.Sin(angle * Mathf.Deg2Rad));
        this.life = life;
        this.bounce = bounce;

        Id = id;
        OwnerId = ownerId;
        Team = team;

        transform.position = origin + velocity * Mathf.Clamp(lag / 1000f, 0f, 0.1f);
    }

    public void Deactivate() {
        body.enabled = false;
        circleCollider.enabled = false;
    }

    public void OnHit() {
        if (--life <= 0) {
            bulletContainer.Remove(Id, OwnerId);
        }
    }
}
