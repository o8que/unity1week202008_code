using UnityEngine;

public sealed class GamePlayerPerk
{
    public const int CountMaxLevel = 7;
    public const int FireRateMaxLevel = 7;
    public const int SpeedMaxLevel = 12;
    public const int SpreadMaxLevel = 12;
    public const int PierceMaxLevel = 3;
    public const int BounceMaxLevel = 3;

    public const int SpeedInitLevel = 6;
    public const int SpreadInitLevel = 6;

    public const int CountCost = 2;
    public const int FireRateCost = 2;
    public const int SpeedCost = 1;
    public const int SpreadCost = 1;
    public const int PierceCost = 4;
    public const int BounceCost = 8;

    private int countLevel;
    private int fireRateLevel;
    private int speedLevel;
    private int spreadLevel;
    private int pierceLevel;
    private int bounceLevel;

    public int CountLevel {
        get => countLevel;
        private set {
            countLevel = value;
            Count = 1 + 2 * value;
        }
    }

    public int FireRateLevel {
        get => fireRateLevel;
        private set {
            fireRateLevel = value;
            FireRate = 1f / (0.8f + 0.1f * value);
        }
    }

    public int SpeedLevel {
        get => speedLevel;
        private set {
            speedLevel = value;
            Speed = 6f + 1f * value;
        }
    }

    public int SpreadLevel {
        get => spreadLevel;
        private set {
            spreadLevel = value;
            Spread = 20f + 5f * value;
        }
    }

    public int PierceLevel {
        get => pierceLevel;
        private set {
            pierceLevel = value;
            Pierce = 1 + value;
        }
    }

    public int BounceLevel {
        get => bounceLevel;
        private set {
            bounceLevel = value;
            Bounce = value;
        }
    }

    public int Points { get; private set; }
    public int Count { get; private set; }
    public float FireRate { get; private set; }
    public float Speed { get; private set; }
    public float Spread { get; private set; }
    public int Pierce { get; private set; }
    public int Bounce { get; private set; }

    public GamePlayerPerk() {
        Init();
    }

    public void Init() {
        Points = 0;
        CountLevel = 0;
        FireRateLevel = 0;
        SpeedLevel = SpeedInitLevel;
        SpreadLevel = SpreadInitLevel;
        PierceLevel = 0;
        BounceLevel = 0;
    }

    public bool TryLevelUp(int id) {
        switch (id) {
        case 1:
            if (Points >= CountCost && CountLevel < CountMaxLevel) {
                Points -= CountCost;
                CountLevel++;
                return true;
            }
            break;
        case 2:
            if (Points >= FireRateCost && FireRateLevel < FireRateMaxLevel) {
                Points -= FireRateCost;
                FireRateLevel++;
                return true;
            }
            break;
        case 3:
            if (Points >= SpeedCost && SpeedLevel > 0) {
                Points -= SpeedCost;
                SpeedLevel--;
                return true;
            }
            break;
        case 4:
            if (Points >= SpeedCost && SpeedLevel < SpeedMaxLevel) {
                Points -= SpeedCost;
                SpeedLevel++;
                return true;
            }
            break;
        case 5:
            if (Points >= SpreadCost && SpreadLevel > 0) {
                Points -= SpreadCost;
                SpreadLevel--;
                return true;
            }
            break;
        case 6:
            if (Points >= SpreadCost && SpreadLevel < SpreadMaxLevel) {
                Points -= SpreadCost;
                SpreadLevel++;
                return true;
            }
            break;
        case 7:
            if (Points >= PierceCost && PierceLevel < PierceMaxLevel) {
                Points -= PierceCost;
                PierceLevel++;
                return true;
            }
            break;
        case 8:
            if (Points >= BounceCost && BounceLevel < BounceMaxLevel) {
                Points -= BounceCost;
                BounceLevel++;
                return true;
            }
            break;
        }

        return false;
    }

    public int GetRandomPerkId() {
        int perkId = 0;
        float rand = Random.value;

        if (rand < 0.3f && CountLevel < CountMaxLevel) {
            perkId = 1;
        } else if (rand < 0.7f && FireRateLevel < FireRateMaxLevel) {
            perkId = 2;
        } else if (rand < 0.8f) {
            if (Random.value < 0.5f && SpeedInitLevel <= SpeedLevel && SpeedLevel < SpeedMaxLevel) {
                perkId = 4;
            } else if (0 < SpeedLevel && SpeedLevel <= SpeedInitLevel) {
                perkId = 3;
            }
        } else if (rand < 0.9f) {
            if (Random.value < 0.5f && SpreadInitLevel <= SpreadLevel && SpreadLevel < SpreadMaxLevel) {
                perkId = 6;
            } else if (0 < SpreadLevel && SpreadLevel <= SpreadInitLevel) {
                perkId = 5;
            }
        } else if (BounceLevel < BounceMaxLevel) {
            perkId = 8;
        }

        return perkId;
    }

    public void ApplyLevel(int count, int fireRate, int speed, int spread, int pierce, int bounce) {
        CountLevel = count;
        FireRateLevel = fireRate;
        SpeedLevel = speed;
        SpreadLevel = spread;
        PierceLevel = pierce;
        BounceLevel = bounce;
    }

    public void AddPoint() {
        Points++;
    }

    public int GetScore() {
        return Mathf.Clamp(
            Points
            + CountLevel * CountCost
            + FireRateLevel * FireRateCost
            + Mathf.Abs(SpeedLevel - SpeedInitLevel) * SpeedCost
            + Mathf.Abs(SpreadLevel - SpreadInitLevel) * SpreadCost
            + PierceLevel * PierceCost
            + BounceLevel * BounceCost,
            0, 255
        );
    }
}
