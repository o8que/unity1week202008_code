// #define TEST

using UnityEngine;

public static class Config
{
    public const string Version = "ver1.3";
    public const string RoomName = "u1w200813";

#if TEST
    public const int ReadyPeriod = 4000;
    public const int ActionPeriod = 60000;
    public const int ResultPeriod = 3000;
    public const int MinPlayerCount = -1;
    public const int BotCount = 100;
#else
    public const int ReadyPeriod = 5000;
    public const int ActionPeriod = 60000;
    public const int ResultPeriod = 15000;
    public const int MinPlayerCount = 0;
    public const int BotCount = 10;
#endif

    public const int ReadyTime = ReadyPeriod;
    public const int ActionTime = ReadyTime + ActionPeriod;
    public const int AddPointInterval = 3000;

    public const int PlayerLife = 5;
    public const float PlayerSpeed = 6f;
    public const int MaxBulletId = 1000;

    public const float WorldHalfWidth = 32f;
    public const float WorldHalfHeight = 18f;
    public const float PixelPerUnit = 20f;
    public const float BoundTop = 18f;
    public const float BoundBottom = -14f;
    public const float BoundLeft = -32.5f;
    public const float BoundRight = 32.5f;
    public const float Team1Top = 15.5f;
    public const float Team1Bottom = -11.5f;
    public const float Team1Left = -29.5f;
    public const float Team1Right = -2.5f;
    public const float Team2Top = 15.5f;
    public const float Team2Bottom = -11.5f;
    public const float Team2Left = 2.5f;
    public const float Team2Right = 29.5f;

    public static Vector3 RandomInitialPosition() {
        return new Vector3(Random.Range(Team1Left, Team2Right), BoundBottom);
    }

    public static Vector3 RandomSpawnPosition(bool team) {
        if (!team) {
            return new Vector3(Random.Range(Team1Left, Team1Right), Random.Range(Team1Bottom, Team1Top));
        } else {
            return new Vector3(Random.Range(Team2Left, Team2Right), Random.Range(Team2Bottom, Team2Top));
        }
    }

    public static Vector3 ClampPosition(Vector3 position, bool team) {
        if (!team) {
            position.x = Mathf.Clamp(position.x, Team1Left, Team1Right);
            position.y = Mathf.Clamp(position.y, Team1Bottom, Team1Top);
        } else {
            position.x = Mathf.Clamp(position.x, Team2Left, Team2Right);
            position.y = Mathf.Clamp(position.y, Team2Bottom, Team2Top);
        }
        return position;
    }
}
