using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class IngameManager : MonoBehaviour
    {
        public static IngameManager Instance { private set; get; }
        public static event System.Action<IngameState> IngameStateChanged = delegate { };

        [Header("Enter a number of level to test. Set back to 0 to disable this feature.")]
        [SerializeField]
        private int testingLevel = 0;

        [Header("Ingame Configuration")]
        [SerializeField]
        private float reviveWaitTime = 5f;

        [SerializeField]
        private int initialPlatformAmount = 10;

        [Header("Level Configuration")]
        [SerializeField]
        private List<LevelConfiguration> listLevelConfiguration = new List<LevelConfiguration>();

        [Header("Ingame References")]
        [SerializeField]
        private Material backgroundMaterial = null;

        [SerializeField]
        private FinishPlatformController finishPlatformController = null;

        [SerializeField]
        private LayerMask platformLayerMask = new LayerMask();

        public IngameState IngameState
        {
            get { return ingameState; }
            private set
            {
                if (value != ingameState)
                {
                    ingameState = value;
                    IngameStateChanged(ingameState);
                }
            }
        }
        public float ReviveWaitTime
        {
            get { return reviveWaitTime; }
        }
        public int CurrentLevel { private set; get; }
        public bool IsRevived { private set; get; }

        private IngameState ingameState = IngameState.Ingame_GameOver;
        private List<PlatformParamsConfiguration> listPlatformParamsConfig =
            new List<PlatformParamsConfiguration>();
        private LevelConfiguration currentLevelConfigs = null;
        private PlatformController previousPlatform = null;
        private AudioClip backgroundMusic = null;
        private int platformParamsIndex = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(Instance.gameObject);
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private IEnumerator Start()
        {
            Application.targetFrameRate = 60;
            ServicesManager.Instance.CoinManager.SetCollectedCoins(0);
            StartCoroutine(CRShowViewWithDelay(ViewType.INGAME_VIEW, 0f));

            //Setup variables
            IsRevived = false;
            finishPlatformController.gameObject.SetActive(false);

            //Load level parameters
            CurrentLevel =
                (testingLevel != 0)
                    ? testingLevel
                    : PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, 1);
            foreach (LevelConfiguration levelConfigs in listLevelConfiguration)
            {
                if (CurrentLevel >= levelConfigs.MinLevel && CurrentLevel < levelConfigs.MaxLevel)
                {
                    Debug.Log(levelConfigs.MaxLevel + " MAX LEVEL");
                    //Setup background and others parameters
                    currentLevelConfigs = levelConfigs;
                    backgroundMusic = levelConfigs.BackgroundMusicClip;
                    backgroundMaterial.SetColor("_TopColor", levelConfigs.BackgroundTopColor);
                    backgroundMaterial.SetColor("_BottomColor", levelConfigs.BackgroundBottomColor);

                    PlayerParamsConfiguration playerParams = new PlayerParamsConfiguration();
                    //adjust player speed
                    playerParams.SetPlayerMovementSpeed(
                        Random.Range(
                            levelConfigs.MinPlayerMovementSpeed,
                            levelConfigs.MaxPlayerMovementSpeed
                        )
                    );
                    playerParams.SetPlayerJumpingPoints(
                        Random.Range(
                            levelConfigs.MinPlayerJumpingPoints,
                            levelConfigs.MaxPlayerJumpingPoints
                        )
                    );
                    PlayerController.Instance.SetPlayerParams(playerParams);

                    for (int i = 0; i < levelConfigs.ListPlatformConfiguration.Count; i++)
                    {
                        PlatformConfiguration platformConfig =
                            levelConfigs.ListPlatformConfiguration[i];

                        //adjust platform list to appear
                        int platformAmount = Random.Range(
                            platformConfig.MinPlatformAmount,
                            platformConfig.MaxPlatformAmount
                        );
                        for (int a = 0; a < platformAmount; a++)
                        {
                            PlatformParamsConfiguration platformParams =
                                new PlatformParamsConfiguration();
                            platformParams.SetPlatformCreationType(
                                platformConfig.PlatformCreationType
                            );
                            platformParams.SetPlatformSize(
                                platformConfig.ListPlatformSize[
                                    Random.Range(0, platformConfig.ListPlatformSize.Count)
                                ]
                            );
                            platformParams.SetCoinItemAmount(
                                Random.Range(
                                    levelConfigs.MinCoinItemAmount,
                                    levelConfigs.MaxCoinItemAmount
                                )
                            );
                            platformParams.SetMagnetItemFrequency(levelConfigs.MagnetItemFrequency);
                            platformParams.SetShieldItemFrequency(levelConfigs.ShieldItemFrequency);
                            platformParams.SetListObstacleAmountConfig(
                                platformConfig.ListObstacleAmountConfiguration
                            );
                            platformParams.SetListObstacleType(platformConfig.ListObstacleType);

                            bool isLastPlatform =
                                (a == platformAmount - 1)
                                && (i == levelConfigs.ListPlatformConfiguration.Count - 1);
                            platformParams.SetLastPlatform(isLastPlatform);

                            listPlatformParamsConfig.Add(platformParams);
                        }
                    }

                    break;
                }
            }

            //Create a platform at player postiion
            previousPlatform = PoolManager.Instance.GetPlatformController(
                currentLevelConfigs.PlatformType,
                PlatformSize.HUGE
            );
            previousPlatform.transform.position = PlayerController.Instance.transform.position;
            previousPlatform.gameObject.SetActive(true);

            //Create intitial platforms
            initialPlatformAmount =
                (initialPlatformAmount > listPlatformParamsConfig.Count)
                    ? listPlatformParamsConfig.Count
                    : initialPlatformAmount;
            for (int i = 0; i < initialPlatformAmount; i++)
            {
                CreateNextPlatform();
                yield return new WaitForSeconds(0.25f);
            }

            if (Utilities.IsShowTutorial())
                Invoke(nameof(PlayingGame), 0.15f);
        }

        /// <summary>
        /// Call IngameState.Ingame_Playing event and handle other actions.
        /// Actual start the game.
        /// </summary>
        public void PlayingGame()
        {
            //Fire event
            IngameState = IngameState.Ingame_Playing;
            ingameState = IngameState.Ingame_Playing;

            //Other actions
            if (IsRevived)
            {
                StartCoroutine(CRShowViewWithDelay(ViewType.INGAME_VIEW, 0f));
                ServicesManager.Instance.SoundManager.ResumeMusic(0.5f);
            }
            else
            {
                ServicesManager.Instance.SoundManager.PlayMusic(backgroundMusic, 0.5f);
            }
        }

        /// <summary>
        /// Call IngameState.Ingame_Revive event and handle other actions.
        /// </summary>
        public void Revive()
        {
            //Fire event
            IngameState = IngameState.Ingame_Revive;
            ingameState = IngameState.Ingame_Revive;

            //Add another actions here
            StartCoroutine(CRShowViewWithDelay(ViewType.REVIVE_VIEW, 1f));
            ServicesManager.Instance.SoundManager.PauseMusic(0.5f);
        }

        /// <summary>
        /// Call IngameState.Ingame_GameOver event and handle other actions.
        /// </summary>
        public void GameOver()
        {
            //Fire event
            IngameState = IngameState.Ingame_GameOver;
            ingameState = IngameState.Ingame_GameOver;

            //Add another actions here
            StartCoroutine(CRShowViewWithDelay(ViewType.ENDGAME_VIEW, 0.25f));
            ServicesManager.Instance.SoundManager.StopMusic(0.5f);
            ServicesManager.Instance.SoundManager.PlaySound(
                ServicesManager.Instance.SoundManager.LevelFailed
            );
        }

        /// <summary>
        /// Call IngameState.Ingame_CompleteLevel event and handle other actions.
        /// </summary>
        public void CompletedLevel()
        {
            //Fire event
            IngameState = IngameState.Ingame_CompleteLevel;
            ingameState = IngameState.Ingame_CompleteLevel;

            //Other actions
            StartCoroutine(CRShowViewWithDelay(ViewType.ENDGAME_VIEW, 1f));
            ServicesManager.Instance.SoundManager.StopMusic(0.5f);
            ServicesManager.Instance.SoundManager.PlaySound(
                ServicesManager.Instance.SoundManager.LevelCompleted
            );
            finishPlatformController.PlayEffect();

            //Save level
            if (testingLevel == 0)
            {
                PlayerPrefs.SetInt(
                    PlayerPrefsKeys.PPK_SAVED_ROUND,
                    PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_ROUND) + 1
                );

                if (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL) == 1)
                {
                    if (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_ROUND) > 12)
                    {
                        PlayerPrefs.SetInt(
                            PlayerPrefsKeys.PPK_SAVED_LEVEL,
                            PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL) + 1
                        );
                        PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_ROUND, 1);
                    }
                }
                else if (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL) == 2)
                {
                    if (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_ROUND) > 12)
                    {
                        PlayerPrefs.SetInt(
                            PlayerPrefsKeys.PPK_SAVED_LEVEL,
                            PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL) + 1
                        );
                        PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_ROUND, 1);
                    }
                }
                else if (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL) == 3)
                {
                    if (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_ROUND) > 15)
                    {
                        PlayerPrefs.SetInt(
                            PlayerPrefsKeys.PPK_SAVED_LEVEL,
                            PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL)
                        );
                        PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_ROUND, 1);
                        PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, 1);
                    }
                }

                //Report level to leaderboard
                string username = PlayerPrefs.GetString(PlayerPrefsKeys.PPK_SAVED_USER_NAME);
                if (!string.IsNullOrEmpty(username))
                {
                    ServicesManager.Instance.LeaderboardManager.SetPlayerLeaderboardData();
                }
            }
        }

        /// <summary>
        /// Coroutine show the view with given viewType and delay time.
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator CRShowViewWithDelay(ViewType viewType, float delay)
        {
            yield return new WaitForSeconds(delay);
            ViewManager.Instance.OnShowView(viewType);
        }

        /// <summary>
        /// Calculate the x axis of current platform based on PlatformCreationType and previousPlatformPos.
        /// </summary>
        /// <param name="creationType"></param>
        /// <param name="previousPlatformPos"></param>
        /// <returns></returns>
        public float CalculateXAxis(PlatformCreationType creationType, Vector3 previousPlatformPos)
        {
            int previousPlatformXAxis = Mathf.RoundToInt(previousPlatformPos.x);
            if (creationType == PlatformCreationType.WAVE)
            {
                if (previousPlatformXAxis < 0 || previousPlatformXAxis > 0)
                {
                    return 0;
                }
                else
                {
                    Vector3 leftPos = new Vector3(-5f, 0f, previousPlatformPos.z);
                    Vector3 rightPos = new Vector3(5f, 0f, previousPlatformPos.z);

                    bool hitLeft = Physics.BoxCast(
                        leftPos,
                        Vector3.one,
                        -transform.forward,
                        Quaternion.identity,
                        previousPlatform.HalfSize * 2f,
                        platformLayerMask
                    );
                    bool hitRight = Physics.BoxCast(
                        rightPos,
                        Vector3.one,
                        -transform.forward,
                        Quaternion.identity,
                        previousPlatform.HalfSize * 2f,
                        platformLayerMask
                    );

                    if (!hitLeft && !hitRight)
                    {
                        return Random.value <= 0.5 ? -5 : 5;
                    }
                    else if (hitLeft)
                    {
                        return 5;
                    }
                    else
                    {
                        return -5;
                    }
                }
            }
            else if (creationType == PlatformCreationType.SYMMETRY_LEFT)
            {
                if (previousPlatformXAxis < 0 || previousPlatformXAxis > 0)
                {
                    return 0;
                }
                else
                {
                    return -5;
                }
            }
            else if (creationType == PlatformCreationType.SYMMETRY_RIGHT)
            {
                if (previousPlatformXAxis < 0 || previousPlatformXAxis > 0)
                {
                    return 0;
                }
                else
                {
                    return 5;
                }
            }
            else if (creationType == PlatformCreationType.RANDOM)
            {
                if (previousPlatformXAxis < 0 || previousPlatformXAxis > 0)
                {
                    return 0;
                }
                else
                {
                    return Random.value <= 0.5 ? -5 : 5;
                }
            }
            else
            {
                return 0;
            }
        }

        //////////////////////////////////////Publish functions



        /// <summary>
        /// Continue the game
        /// </summary>
        public void SetContinueGame()
        {
            IsRevived = true;
            Invoke(nameof(PlayingGame), 0.05f);
        }

        /// <summary>
        /// Handle actions when player died.
        /// </summary>
        public void HandlePlayerDied()
        {
            if (IsRevived)
            {
                //Fire event
                IngameState = IngameState.Ingame_GameOver;
                ingameState = IngameState.Ingame_GameOver;

                //Add another actions here
                StartCoroutine(CRShowViewWithDelay(ViewType.ENDGAME_VIEW, 1f));
                ServicesManager.Instance.SoundManager.StopMusic(0.5f);
                ServicesManager.Instance.SoundManager.PlaySound(
                    ServicesManager.Instance.SoundManager.LevelFailed
                );
            }
            else
            {
                Revive();
            }
        }

        /// <summary>
        /// Create next platform.
        /// </summary>
        public void CreateNextPlatform()
        {
            if (
                platformParamsIndex == listPlatformParamsConfig.Count
                && !finishPlatformController.gameObject.activeSelf
            )
            {
                //Create another platform at center
                PlatformController centerPlatform = PoolManager.Instance.GetPlatformController(
                    currentLevelConfigs.PlatformType,
                    PlatformSize.NORMAL
                );
                Vector3 centerPos =
                    previousPlatform.transform.position
                    + Vector3.forward * (previousPlatform.HalfSize + centerPlatform.HalfSize);
                centerPlatform.transform.position = new Vector3(0f, centerPos.y, centerPos.z);
                centerPlatform.gameObject.SetActive(true);

                //Enable finish platform
                finishPlatformController.transform.position =
                    centerPlatform.transform.position + Vector3.forward * centerPlatform.HalfSize;
                finishPlatformController.gameObject.SetActive(true);
                finishPlatformController.MoveUp(
                    PoolManager.Instance.GetHugePlatformController(currentLevelConfigs.PlatformType)
                );
            }
            else if (platformParamsIndex < listPlatformParamsConfig.Count)
            {
                //Create the platform
                PlatformParamsConfiguration platformParams = listPlatformParamsConfig[
                    platformParamsIndex
                ];
                PlatformController currentPlatform = PoolManager.Instance.GetPlatformController(
                    currentLevelConfigs.PlatformType,
                    platformParams.PlatformSize
                );
                Vector3 platformPos =
                    previousPlatform.transform.position
                    + Vector3.forward * (previousPlatform.HalfSize + currentPlatform.HalfSize);
                platformPos.x = CalculateXAxis(
                    platformParams.PlatformCreationType,
                    previousPlatform.transform.position
                );
                platformPos.y = 0f;
                currentPlatform.transform.position = platformPos;
                currentPlatform.gameObject.SetActive(true);
                currentPlatform.OnSetup(listPlatformParamsConfig[platformParamsIndex]);

                //Update paramaters
                previousPlatform = currentPlatform;
                platformParamsIndex++;
            }
        }
    }
}
