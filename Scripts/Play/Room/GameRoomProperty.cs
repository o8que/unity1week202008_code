using ExitGames.Client.Photon;
using Photon.Realtime;

public static class GameRoomProperty
{
    private const string StartTime = "t";

    private static readonly Hashtable propsToSet = new Hashtable();

    public static void SendCustomProperties(this Room room) {
        if (propsToSet.Count > 0) {
            room.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }
    }

    public static int GetStartTime(this Room room) {
        if (room.CustomProperties[StartTime] is int startTime) {
            return startTime;
        }
        return 0;
    }

    public static void SetStartTime(this Room room, int startTime) {
        propsToSet[StartTime] = (startTime != 0) ? startTime : 1;
    }

    public static bool TryGetPlayerProperties(this Hashtable hashtable, string nickName, out (bool isAlive, bool team, int kill, int score) props) {
        if (hashtable[nickName] is short bytes) {
            props = GameProtocol.DeserializePlayerProperties(bytes);
            return true;
        }

        props = (false, false, 0, 0);
        return false;
    }

    public static (bool isAlive, bool team, int kill, int score) GetPlayerProperties(this Room room, string nickName) {
        if (room.CustomProperties[nickName] is short bytes) {
            return GameProtocol.DeserializePlayerProperties(bytes);
        }
        return (false, false, 0, 0);
    }

    public static void SetPlayerProperties(this Room room, string nickName, bool isAlive, bool team, int kill, int score) {
        propsToSet[nickName] = GameProtocol.SerializePlayerProperties(isAlive, team, kill, score);
    }

    public static void RemovePlayerProperties(this Room room, string nickName) {
        propsToSet[nickName] = null;
    }
}
