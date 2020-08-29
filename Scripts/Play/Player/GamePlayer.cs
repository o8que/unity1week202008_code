using BehaviorDesigner.Runtime;
using ExitGames.Client.Photon;
using Ore2Lib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public sealed class GamePlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    private GamePlayerContainer playerContainer;
    private GameBulletContainer bulletContainer;
    private GamePlayerView view;
    private BehaviorTree behaviorTree;

    private bool isActive = false;
    private float attackInterval = 0f;
    private int nextBulletId = 0;
    private int nextPerkId = 0;
    private Vector3 targetPosition;

    public GamePlayerPerk Perk { get; private set; }

    public int Id { get; private set; }
    public string NickName { get; private set; }
    public bool IsLocal { get; private set; }
    public bool IsRemote { get; private set; }
    public bool IsBot { get; private set; }
    public int Life { get; private set; }
    public bool IsAlive { get; private set; }
    public bool Team { get; private set; }
    public int Kill { get; private set; }
    public int Score { get; private set; }

    private void Awake() {
        playerContainer = GameObject.FindWithTag(TagName.PlayerContainer).GetComponent<GamePlayerContainer>();
        bulletContainer = GameObject.FindWithTag(TagName.BulletContainer).GetComponent<GameBulletContainer>();
        transform.SetParent(playerContainer.transform, false);

        view = GetComponent<GamePlayerView>();
        behaviorTree = GetComponent<BehaviorTree>();
        Perk = new GamePlayerPerk();

        if (photonView.IsSceneView) {
            int index = (int)photonView.InstantiationData[0];
            Id = -index;
            NickName = $"BOT-{index}";
            IsLocal = false;
            IsRemote = false;
            IsBot = true;
            Life = Config.PlayerLife;
            var props = PhotonNetwork.CurrentRoom.GetPlayerProperties(NickName);
            IsAlive = props.isAlive;
            Team = props.team;
            Kill = props.kill;
            Score = props.score;
        } else if (photonView.IsMine) {
            Id = photonView.OwnerActorNr;
            NickName = photonView.Owner.NickName;
            IsLocal = true;
            IsRemote = false;
            IsBot = false;
            Life = Config.PlayerLife;
            var props = photonView.Owner.GetPlayerProperties();
            IsAlive = props.isAlive;
            Team = props.team;
            Kill = props.kill;
            Score = props.score;
        } else {
            Id = photonView.OwnerActorNr;
            NickName = photonView.Owner.NickName;
            IsLocal = false;
            IsRemote = true;
            IsBot = false;
            Life = Config.PlayerLife;
            var props = photonView.Owner.GetPlayerProperties();
            IsAlive = props.isAlive;
            Team = props.team;
            Kill = props.kill;
            Score = props.score;
        }

        view.Init(NickName, IsLocal, IsRemote, IsAlive);
    }

    public void OnUpdate() {
        if (!isActive) { return; }
        if (photonView.IsMine) {
            if (IsLocal) {
                int perkId = 0;
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
                    perkId = 1;
                } else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) {
                    perkId = 2;
                } else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) {
                    perkId = 3;
                } else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) {
                    perkId = 4;
                } else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) {
                    perkId = 5;
                } else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) {
                    perkId = 6;
                } else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) {
                    perkId = 7;
                } else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) {
                    perkId = 8;
                }

                if (perkId > 0 && Perk.TryLevelUp(perkId)) {
                    GameManager.Audio.PlayPowerupSFX();
                }

                var input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                ProcessInput(input);
            } else if (IsBot && photonView.IsMine) {
                if (nextPerkId <= 0 || Perk.TryLevelUp(nextPerkId)) {
                    nextPerkId = Perk.GetRandomPerkId();
                }

                var input = ((SharedVector3)behaviorTree.GetVariable("Input")).Value;
                ProcessInput(input);
            }
        } else {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Config.PlayerSpeed * Time.deltaTime);
        }
    }

    private void ProcessInput(Vector3 input) {
        if (input.sqrMagnitude > 0f) {
            attackInterval = 0f;
            var velocity = Config.PlayerSpeed * Time.deltaTime * input.normalized;
            transform.position = Config.ClampPosition(transform.position + velocity, Team);
        } else {
            attackInterval += Time.deltaTime;
            if (attackInterval >= Perk.FireRate) {
                attackInterval -= Perk.FireRate;
                long bytes = GameProtocol.SerializeRPCAttack(nextBulletId, Team, transform.position, Perk);
                photonView.RPC(nameof(Attack), RpcTarget.All, bytes);
            }
        }
    }

    public void OnClickPerkButton(int perkId) {
        if (IsLocal && isActive && Perk.TryLevelUp(perkId)) {
            GameManager.Audio.PlayPowerupSFX();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!isActive || !photonView.IsMine) { return; }
        if (!other.TryGetComponent<GameBullet>(out var bullet)) { return; }
        if (bullet.Team == Team) { return; }

        bool andDead = ((--Life) <= 0);
        view.SetCurrentLife(Life);
        photonView.RPC(nameof(TakeDamage), RpcTarget.All, bullet.OwnerId, GameProtocol.SerializeRPCTakeDamage(bullet.Id, andDead));

        if (andDead) {
            isActive = false;
            view.OnDead();
            GameManager.Audio.PlayDeadSFX();

            IsAlive = false;
            Score = Perk.GetScore();

            if (IsBot) {
                PhotonNetwork.CurrentRoom.SetPlayerProperties(NickName, IsAlive, Team, Kill, Score);
            } else {
                photonView.Owner.SetPlayerProperties(IsAlive, Team, Kill, Score);
            }
        }

        if (IsLocal) {
            GameManager.Screen.Shake();
        }
    }

    [PunRPC]
    private void Attack(long bytes, PhotonMessageInfo info) {
        var p = GameProtocol.DeserializeRPCAttack(bytes);
        Perk.ApplyLevel(p.count, p.fireRate, p.speed, p.spread, p.pierce, p.bounce);

        int lag = Mathf.Clamp(unchecked(PhotonNetwork.ServerTimestamp - info.SentServerTimestamp), 0, 100);
        bulletContainer.PutBarrage(p.bulletId, Id, p.team, p.origin, lag, Perk);
        nextBulletId = (p.bulletId + Perk.Count) % Config.MaxBulletId;
        GameManager.Audio.PlayAttackSFX();
    }

    [PunRPC]
    private void TakeDamage(int bulletOwnerId, short bytes) {
        var p = GameProtocol.DeserializeRPCTakeDamage(bytes);
        playerContainer.NotifyDealDamage(bulletOwnerId, p.andDead);
        bulletContainer.OnHit(p.bulletId, bulletOwnerId);
        view.OnTakeDamage();
        GameManager.Audio.PlayDamageSFX();
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(GameProtocol.SerializePlayerView(transform.position, Life));
        } else {
            var data = GameProtocol.DeserializePlayerView((int)stream.ReceiveNext());

            Life = data.life;
            view.SetCurrentLife(Life);

            if (isActive) {
                targetPosition = data.position;
            } else {
                transform.position = data.position;
                targetPosition = data.position;
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if (targetPlayer.ActorNumber != photonView.OwnerActorNr) { return; }
        var props = changedProps.GetPlayerProperties();

        if (!IsAlive && props.isAlive) {
            view.OnRespawn();
        } else if (IsAlive && !props.isAlive) {
            isActive = false;
            view.OnDead();
            GameManager.Audio.PlayDeadSFX();
        }

        IsAlive = props.isAlive;
        Team = props.team;
        Kill = props.kill;
        Score = props.score;
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
        if (!propertiesThatChanged.TryGetPlayerProperties(NickName, out var props)) { return; }

        if (!IsAlive && props.isAlive) {
            view.OnRespawn();
            behaviorTree.EnableBehavior();
        } else if (IsAlive && !props.isAlive) {
            isActive = false;
            view.OnDead();
            behaviorTree.DisableBehavior();
            GameManager.Audio.PlayDeadSFX();
        }

        IsAlive = props.isAlive;
        Team = props.team;
        Kill = props.kill;
        Score = props.score;
    }

    public void Respawn(bool nextTeam) {
        Perk.Init();
        attackInterval = 0;
        nextBulletId = 0;

        Life = Config.PlayerLife;

        if (photonView.IsMine) {
            transform.position = Config.RandomSpawnPosition(nextTeam);
            view.OnRespawn();

            IsAlive = true;
            Team = nextTeam;
            Kill = 0;
            Score = 0;

            if (IsBot) {
                PhotonNetwork.CurrentRoom.SetPlayerProperties(NickName, true, nextTeam, 0, 0);
            } else {
                photonView.Owner.SetPlayerProperties(true, nextTeam, 0, 0);
            }
        }
    }

    public void Activate() {
        if (IsAlive) {
            isActive = true;
            if (IsBot) {
                behaviorTree.EnableBehavior();
            }
        }
    }

    public void Deactivate() {
        isActive = false;
        if (IsBot) {
            behaviorTree.DisableBehavior();
        }

        if (IsAlive && photonView.IsMine) {
            Score = Perk.GetScore();

            if (IsBot) {
                PhotonNetwork.CurrentRoom.SetPlayerProperties(NickName, IsAlive, Team, Kill, Score);
            } else {
                photonView.Owner.SetPlayerProperties(IsAlive, Team, Kill, Score);
            }
        }
    }

    public void AddPoint() {
        if (IsAlive && photonView.IsMine) {
            Perk.AddPoint();
        }
    }

    public void OnDealDamage(bool andDead) {
        if (isActive && photonView.IsMine) {
            Perk.AddPoint();

            if (andDead) {
                Kill++;
            }

            if (IsLocal) {
                GameManager.Audio.PlayHitSFX();
            }
        }
    }
}
