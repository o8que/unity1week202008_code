using UnityEngine;

public static class GameProtocol
{
    public static int SerializePlayerView(Vector3 position, int life) {
        int px = Mathf.RoundToInt((position.x + Config.WorldHalfWidth) * Config.PixelPerUnit);
        int py = Mathf.RoundToInt((position.y + Config.WorldHalfHeight) * Config.PixelPerUnit);

        return (life << 21) | (px << 10) | py;
    }

    public static (Vector3 position, int life) DeserializePlayerView(int bytes) {
        float px = ((bytes >> 10) & 0b_11111_11111_1) / Config.PixelPerUnit - Config.WorldHalfWidth;
        float py = (bytes & 0b_11111_11111) / Config.PixelPerUnit - Config.WorldHalfHeight;
        int life = (bytes >> 21);

        return (new Vector3(px, py), life);
    }

    public static long SerializeRPCAttack(int bulletId, bool team, Vector3 origin, GamePlayerPerk perk) {
        long bulletIdLong = bulletId;
        long teamLong = (team) ? 1 : 0;
        long px = Mathf.RoundToInt((origin.x + Config.WorldHalfWidth) * Config.PixelPerUnit);
        long py = Mathf.RoundToInt((origin.y + Config.WorldHalfHeight) * Config.PixelPerUnit);

        long count = perk.CountLevel;
        long fireRate = perk.FireRateLevel;
        long speed = perk.SpeedLevel;
        long spread = perk.SpreadLevel;
        long pierce = perk.PierceLevel;
        long bounce = perk.BounceLevel;

        return (count << 47) | (fireRate << 44) | (speed << 40) | (spread << 36) | (pierce << 34) | (bounce << 32)
            | (bulletIdLong << 22) | (teamLong << 21) | (px << 10) | py;
    }

    public static (int bulletId, bool team, Vector3 origin, int count, int fireRate, int speed, int spread, int pierce, int bounce) DeserializeRPCAttack(long bytes) {
        int bulletId = (int)(bytes >> 22) & 0b_11111_11111;
        bool team = ((bytes >> 21) & 0b_1) == 1;
        float px = ((bytes >> 10) & 0b_11111_11111_1) / Config.PixelPerUnit - Config.WorldHalfWidth;
        float py = (bytes & 0b_11111_11111) / Config.PixelPerUnit - Config.WorldHalfHeight;

        int count = (int)(bytes >> 47) & 0b_111;
        int fireRate = (int)(bytes >> 44) & 0b_111;
        int speed = (int)(bytes >> 40) & 0b_1111;
        int spread = (int)(bytes >> 36) & 0b_1111;
        int pierce = (int)(bytes >> 34) & 0b_11;
        int bounce = (int)(bytes >> 32) & 0b_11;

        return (bulletId, team, new Vector3(px, py), count, fireRate, speed, spread, pierce, bounce);
    }

    public static short SerializeRPCTakeDamage(int bulletId, bool andDead) {
        int andDeadInt = (andDead) ? 1 : 0;

        return (short)((bulletId << 1) | andDeadInt);
    }

    public static (int bulletId, bool andDead) DeserializeRPCTakeDamage(int bytes) {
        int bulletId = (bytes >> 1) & 0b_11111_11111;
        bool andDead = (bytes & 0b_1) == 1;

        return (bulletId, andDead);
    }

    public static short SerializePlayerProperties(bool isAlive, bool team, int kill, int score) {
        int isAliveInt = (isAlive) ? 1 : 0;
        int teamInt = (team) ? 1 : 0;

        return (short)((isAliveInt << 15) | (teamInt << 14) | (kill << 8) | score);
    }

    public static (bool isAlive, bool team, int kill, int score) DeserializePlayerProperties(int bytes) {
        bool isAlive = ((bytes >> 15) & 0b_1) == 1;
        bool team = ((bytes >> 14) & 0b_1) == 1;
        int kill = (bytes >> 8) & 0b_11111_1;
        int score = bytes & 0b_11111_111;

        return (isAlive, team, kill, score);
    }
}
