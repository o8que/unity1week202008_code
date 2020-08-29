using System.Collections.Generic;
using UnityEngine;

public sealed class GameBulletContainer : MonoBehaviour
{
    [SerializeField]
    private GameBullet bulletPrefab = default;

    private readonly List<GameBullet> activeBullets = new List<GameBullet>(1024);
    private readonly Stack<GameBullet> inactiveBullets = new Stack<GameBullet>(1024);

    public void Init() {
        for (int i = 0; i < 128; i++) {
            inactiveBullets.Push(Create());
        }
    }

    private GameBullet Create() {
        return Instantiate(bulletPrefab, new Vector3(100f, 100f), Quaternion.identity, transform).Init(this);
    }

    public void OnUpdate() {
        for (int i = activeBullets.Count - 1; i >= 0; i--) {
            activeBullets[i].OnUpdate();
        }
    }

    public void PutBarrage(int id, int ownerId, bool team, Vector3 origin, int lag, GamePlayerPerk perk) {
        float forward = (!team) ? 0f : 180f;

        Add(id, ownerId, team, origin, lag, perk.Speed, forward, perk.Pierce, perk.Bounce);
        for (int i = 1; i <= perk.CountLevel; i++) {
            float angle = i * perk.Spread / perk.CountLevel;
            id = (id + 1) % Config.MaxBulletId;
            Add(id, ownerId, team, origin, lag, perk.Speed, forward + angle, perk.Pierce, perk.Bounce);
            id = (id + 1) % Config.MaxBulletId;
            Add(id, ownerId, team, origin, lag, perk.Speed, forward - angle, perk.Pierce, perk.Bounce);
        }
    }

    private void Add(int id, int ownerId, bool team, Vector3 origin, int lag, float speed, float angle, int life, int bounce) {
        var bullet = (inactiveBullets.Count > 0) ? inactiveBullets.Pop() : Create();
        bullet.Activate(id, ownerId, team, origin, lag, speed, angle, life, bounce);
        activeBullets.Add(bullet);
    }

    public void Remove(int id, int ownerId) {
        foreach (var bullet in activeBullets) {
            if (bullet.Equals(id, ownerId)) {
                bullet.Deactivate();
                activeBullets.Remove(bullet);
                inactiveBullets.Push(bullet);
                return;
            }
        }
    }

    public void OnHit(int id, int ownerId) {
        foreach (var bullet in activeBullets) {
            if (bullet.Equals(id, ownerId)) {
                bullet.OnHit();
                return;
            }
        }
    }

    public void Clear() {
        foreach (var bullet in activeBullets) {
            bullet.Deactivate();
            inactiveBullets.Push(bullet);
        }
        activeBullets.Clear();
    }
}
