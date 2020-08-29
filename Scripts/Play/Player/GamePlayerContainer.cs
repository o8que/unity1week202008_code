using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public sealed class GamePlayerContainer : MonoBehaviour
{
    private readonly List<GamePlayer> playerList = new List<GamePlayer>(100);

    public GamePlayer this[int index] => playerList[index];
    public int Count => playerList.Count;

    private void OnTransformChildrenChanged() {
        playerList.Clear();
        foreach (Transform child in transform) {
            playerList.Add(child.GetComponent<GamePlayer>());
        }
    }

    public void Sort() {
        playerList.Sort(
            (p1, p2) => {
                int diff = p2.Score - p1.Score;
                if (diff != 0) { return diff; }
                if (!p1.IsBot || !p2.IsBot) {
                    if (p1.IsBot) { return 1; }
                    if (p2.IsBot) { return -1; }
                }
                return p1.Id - p2.Id;
            }
        );
    }

    public void OnUpdate() {
        foreach (var player in playerList) {
            player.OnUpdate();
        }
    }

    public void RespawnAll() {
        Sort();

        int humanCount = 0;
        int botCount = 0;
        for (int i = 0; i < playerList.Count; i++) {
            var player = playerList[i];
            if (player.IsBot) {
                player.Respawn(botCount % 2 == 1);
                botCount++;
            } else {
                player.Respawn(humanCount % 2 == 0);
                humanCount++;
            }
        }
    }

    public void ActivateAll() {
        foreach (var player in playerList) {
            player.Activate();
        }
    }

    public void DeactivateAll() {
        foreach (var player in playerList) {
            player.Deactivate();
        }
    }

    public void AddPoint() {
        foreach (var player in playerList) {
            player.AddPoint();
        }
    }

    public void NotifyDealDamage(int bulletOwnerId, bool andDead) {
        foreach (var player in playerList) {
            if (bulletOwnerId == player.Id) {
                player.OnDealDamage(andDead);
                return;
            }
        }
    }

    public (int team1, int team2) CountAlivePlayers() {
        int count1 = 0, count2 = 0;
        foreach (var player in playerList) {
            if (!player.Team) {
                if (player.IsAlive) {
                    count1++;
                }
            } else {
                if (player.IsAlive) {
                    count2++;
                }
            }
        }
        return (count1, count2);
    }

    public void MakeUpWithBots(int count) {
        int humanCount = 0;
        foreach (var player in playerList) {
            if (!player.IsBot) {
                humanCount++;
            }
        }

        int currentBotCount = playerList.Count - humanCount;
        int nextBotCount = Mathf.Max(humanCount % 2, count - humanCount);
        if (currentBotCount < nextBotCount) {
            // ボットを増やす
            for (int i = currentBotCount + 1; i <= nextBotCount; i++) {
                var position = Config.RandomInitialPosition();
                var data = new object[] { i };
                var bot = PhotonNetwork.InstantiateRoomObject("GamePlayer", position, Quaternion.identity, 0, data).GetComponent<GamePlayer>();
                PhotonNetwork.CurrentRoom.SetPlayerProperties(bot.NickName, false, false, 0, 0);
            }
        } else if (currentBotCount > nextBotCount) {
            // ボットを減らす
            for (int i = playerList.Count - 1; i >= 0; i--) {
                var player = playerList[i];
                if (player.IsBot && -player.Id > nextBotCount) {
                    PhotonNetwork.Destroy(player.photonView);
                    PhotonNetwork.CurrentRoom.RemovePlayerProperties(player.NickName);

                    playerList.Remove(player);
                }
            }
        }
    }
}
