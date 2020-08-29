using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public static class GamePlayerProperty
{
    private const string Properties = "p";

    private static readonly Hashtable propsToSet = new Hashtable();

    public static (bool isAlive, bool team, int kill, int score) GetPlayerProperties(this Hashtable hashtable) {
        if (hashtable[Properties] is short bytes) {
            return GameProtocol.DeserializePlayerProperties(bytes);
        }
        return (false, false, 0, 0);
    }

    public static (bool isAlive, bool team, int kill, int score) GetPlayerProperties(this Player player) {
        return player.CustomProperties.GetPlayerProperties();
    }

    public static void SetPlayerProperties(this Player player, bool isAlive, bool team, int kill, int score) {
        propsToSet[Properties] = GameProtocol.SerializePlayerProperties(isAlive, team, kill, score);
        PhotonNetwork.SetPlayerCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}
