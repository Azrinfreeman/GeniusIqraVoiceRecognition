using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClawbearGames
{

    #region --------------------Ingame enums
    public enum IngameState
    {
        Ingame_Playing = 0,
        Ingame_Revive = 1,
        Ingame_GameOver = 2,
        Ingame_CompleteLevel = 3,
    }

    public enum PlayerState
    {
        Player_Prepare = 0,
        Player_Living = 1,
        Player_Died = 2,
        Player_CompletedLevel = 3,
    }


    public enum ItemType
    {
        COIN = 0,
        MAGNET = 1,
        SHIELD = 2,
    }


    public enum PlatformType
    {
        GREEN = 0,
        BLUE = 1,
        GRAY = 2,
        RED = 3,
        YELLOW = 4,
        WHITE = 5,
        CYAN = 6,
        PURPLE = 7,
        PINK = 8,
        BLACK = 9,
    }


    public enum PlatformCreationType
    {
        RANDOM = 0,
        SYMMETRY_LEFT = 1,
        SYMMETRY_RIGHT = 2,
        WAVE = 3,
    }

    public enum PlatformSize
    {
        SMALL = 0,
        NORMAL = 1,
        MEDIUM = 2,
        BIG = 3,
        HUGE = 4,
    }


    public enum ObstacleType
    {
        SPIKE_OBSTACLE = 0,
        SPINNER_OBSTACLE = 1,
        FIRE_OBSTACLE = 2,
        ICE_OBSTACLE = 3,
    }


    public enum DayType
    {
        DAY_1 = 0,
        DAY_2 = 1,
        DAY_3 = 2,
        DAY_4 = 3,
        DAY_5 = 4,
        DAY_6 = 5,
        DAY_7 = 6,
        DAY_8 = 7,
        DAY_9 = 8,
    }

    #endregion



    #region --------------------Ads enums
    public enum BannerAdType
    {
        NONE = 0,
        ADMOB = 1,
        UNITY = 2,
    }

    public enum InterstitialAdType
    {
        UNITY = 0,
        ADMOB = 1,
    }


    public enum RewardedAdType
    {
        UNITY = 0,
        ADMOB = 1,
    }

    public enum RewardedAdTarget
    {
        GET_FREE_COINS = 0,
        GET_DOUBLE_COIN = 1,
        REVIVE_PLAYER = 2,
    }

    #endregion



    #region --------------------View Enums
    public enum ViewType
    {
        HOME_VIEW = 0,
        LEADERBOARD_VIEW = 1,
        DAILY_REWARD_VIEW = 2,
        LOADING_VIEW = 3,
        INGAME_VIEW = 4,
        REVIVE_VIEW = 5,
        ENDGAME_VIEW = 6,
        CHARACTER_VIEW = 7,
    }

    #endregion



    #region --------------------Classes

    [System.Serializable]
    public class LevelConfiguration
    {
        [Header("Level Number Configuration")]
        [SerializeField] private int minLevel = 1;
        public int MinLevel { get { return minLevel; } }
        [SerializeField] private int maxLevel = 1;
        public int MaxLevel { get { return maxLevel; } }



        [Header("Background Configuration")]
        [SerializeField] private Color backgroundTopColor = Color.white;
        public Color BackgroundTopColor { get { return backgroundTopColor; } }
        [SerializeField] private Color backgroundBottomColor = Color.white;
        public Color BackgroundBottomColor { get { return backgroundBottomColor; } }
        [SerializeField] private AudioClip backgroundMusicClip = null;
        public AudioClip BackgroundMusicClip { get { return backgroundMusicClip; } }


        [Header("Platform Type Configuration")]
        [SerializeField] private PlatformType platformType = PlatformType.GREEN;
        public PlatformType PlatformType { get { return platformType; } }



        [Header("Player Parameters Configuration")]
        [SerializeField][Range(1, 200)] private int minPlayerMovementSpeed = 15;
        public int MinPlayerMovementSpeed { get { return minPlayerMovementSpeed; } }
        [SerializeField][Range(1, 200)] private int maxPlayerMovementSpeed = 35;
        public int MaxPlayerMovementSpeed { get { return maxPlayerMovementSpeed; } }
        [SerializeField][Range(1, 100)] private int minPlayerJumpingPoints = 25;
        public int MinPlayerJumpingPoints { get { return minPlayerJumpingPoints; } }
        [SerializeField][Range(1, 100)] private int maxPlayerJumpingPoints = 40;
        public int MaxPlayerJumpingPoints { get { return maxPlayerJumpingPoints; } }


        [Header("Items Configuration")]
        [SerializeField][Range(1, 20)] private int minCoinItemAmount = 1;
        public int MinCoinItemAmount { get { return minCoinItemAmount; } }
        [SerializeField][Range(1, 20)] private int maxCoinItemAmount = 5;
        public int MaxCoinItemAmount { get { return maxCoinItemAmount; } }
        [SerializeField][Range(0f, 1f)] private float magnetItemFrequency = 0.1f;
        public float MagnetItemFrequency { get { return magnetItemFrequency; } }
        [SerializeField][Range(0f, 1f)] private float shieldItemFrequency = 0.1f;
        public float ShieldItemFrequency { get { return shieldItemFrequency; } }



        [Header("List Platform Configuration")]
        [SerializeField] private List<PlatformConfiguration> listPlatformConfiguration = new List<PlatformConfiguration>();
        public List<PlatformConfiguration> ListPlatformConfiguration { get { return listPlatformConfiguration; } }
    }



    [System.Serializable]
    public class PlatformConfiguration
    {
        [Header("Platform Amount Configuration")]
        [SerializeField] private int minPlatformAmount = 10;
        public int MinPlatformAmount { get { return minPlatformAmount; } }
        [SerializeField] private int maxPlatformAmount = 20;
        public int MaxPlatformAmount { get { return maxPlatformAmount; } }

        [Header("Platform Creation Type Configuration")]
        [SerializeField] private PlatformCreationType platformCreationType = PlatformCreationType.RANDOM;
        public PlatformCreationType PlatformCreationType { get { return platformCreationType; } }

        [Header("List Platform Size")]
        [SerializeField] private List<PlatformSize> listPlatformSize = new List<PlatformSize>();
        public List<PlatformSize> ListPlatformSize { get { return listPlatformSize; } }



        [Header("List Obstacle Type")]
        [SerializeField] private List<ObstacleType> listObstacleType = new List<ObstacleType>();
        public List<ObstacleType> ListObstacleType { get { return listObstacleType; } }


        [Header("List Obstacle Amount Configuration")]
        [SerializeField] private List<ObstacleAmountConfiguration> listObstacleAmountConfiguration = new List<ObstacleAmountConfiguration>();
        public List<ObstacleAmountConfiguration> ListObstacleAmountConfiguration { get { return listObstacleAmountConfiguration; } }

    }


    [System.Serializable]
    public class ObstacleAmountConfiguration
    {
        [SerializeField][Range(0, 2)] private int obstacleAmount = 0;
        public int ObstacleAmount { get { return obstacleAmount; } }

        [SerializeField][Range(0f, 1f)] private float frequency = 0.1f;
        public float Frequency { get { return frequency; } }
    }



    [System.Serializable]
    public class PlatformPrefabConfiguration
    {
        [SerializeField] private PlatformType platformType = PlatformType.GREEN;
        public PlatformType PlatformType { get { return platformType; } }

        [SerializeField] private PlatformController[] platformControllerPrefabs = null;
        public PlatformController[] PlatformControllerPrefabs { get { return platformControllerPrefabs; } }
    }




    [System.Serializable]
    public class DailyRewardConfiguration
    {
        [SerializeField] private DayType dayType = DayType.DAY_1;

        /// <summary>
        /// the day type of this DailyRewardItem.
        /// </summary>
        public DayType DayType { get { return dayType; } }


        [SerializeField] private int coinAmount = 0;


        /// <summary>
        /// The amount of coins reward to player.
        /// </summary>
        public int CoinAmount { get { return coinAmount; } }
    }


    [System.Serializable]
    public class InterstitialAdConfiguration
    {
        [SerializeField] private IngameState ingameStateWhenShowingAd = IngameState.Ingame_CompleteLevel;
        public IngameState IngameStateWhenShowingAd { get { return ingameStateWhenShowingAd; } }

        [SerializeField] private int ingameStateAmountWhenShowingAd = 1;
        public int IngameStateAmountWhenShowingAd { get { return ingameStateAmountWhenShowingAd; } }


        [SerializeField] private float delayTimeWhenShowingAd = 2f;
        public float DelayTimeWhenShowingAd { get { return delayTimeWhenShowingAd; } }

        [SerializeField] private List<InterstitialAdType> listInterstitialAdType = new List<InterstitialAdType>();
        public List<InterstitialAdType> ListInterstitialAdType { get { return listInterstitialAdType; } }
    }




    public class PlatformParamsConfiguration
    {
        public PlatformCreationType PlatformCreationType { private set; get; }
        public void SetPlatformCreationType(PlatformCreationType platformCreationType)
        {
            PlatformCreationType = platformCreationType;
        }


        public PlatformSize PlatformSize { private set; get; }
        public void SetPlatformSize(PlatformSize platformSize)
        {
            PlatformSize = platformSize;
        }



        public int CoinItemAmount { private set; get; }
        public void SetCoinItemAmount(int coinItemAmount)
        {
            CoinItemAmount = coinItemAmount;
        }

        public float MagnetItemFrequency { private set; get; }
        public void SetMagnetItemFrequency(float magnetItemFrequency)
        {
            MagnetItemFrequency = magnetItemFrequency;
        }

        public float ShieldItemFrequency { private set; get; }
        public void SetShieldItemFrequency(float shieldItemFrequency)
        {
            ShieldItemFrequency = shieldItemFrequency;
        }


        public List<ObstacleAmountConfiguration> ListObstacleAmountConfig { private set; get; }
        public void SetListObstacleAmountConfig(List<ObstacleAmountConfiguration> obstacleAmountConfigs)
        {
            ListObstacleAmountConfig = obstacleAmountConfigs;
        }


        public List<ObstacleType> ListObstacleType { private set; get; }
        public void SetListObstacleType(List<ObstacleType> listObstacleType)
        {
            ListObstacleType = listObstacleType;
        }


        public bool IsLastPlatform { private set; get; }
        public void SetLastPlatform(bool isLastPlatform)
        {
            IsLastPlatform = isLastPlatform;
        }
    }


    public class PlayerParamsConfiguration
    {
        public float PlayerMovementSpeed { private set; get; }
        public void SetPlayerMovementSpeed(float playerMovementSpeed)
        {
            PlayerMovementSpeed = playerMovementSpeed;
        }

        public float PlayerJumpingPoints { private set; get; }
        public void SetPlayerJumpingPoints(float playerJumpingPoints)
        {
            PlayerJumpingPoints = playerJumpingPoints;
        }
    }




    public class LeaderboardData
    {
        public string Username { private set; get; }
        public void SetUsername(string username)
        {
            Username = username;
        }

        public int Level { private set; get; }
        public void SetLevel(int level)
        {
            Level = level;
        }
    }

    public class LeaderboardDataComparer : IComparer<LeaderboardData>
    {
        public int Compare(LeaderboardData dataX, LeaderboardData dataY)
        {
            if (dataX.Level < dataY.Level)
                return 1;
            if (dataX.Level > dataY.Level)
                return -1;
            else
                return 0;
        }
    }

    #endregion
}
