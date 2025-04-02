using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class PlayerController : MonoBehaviour
    {

        public static PlayerController Instance { private set; get; }
        public static event System.Action<PlayerState> PlayerStateChanged = delegate { };


        [Header("Player Configuration")]
        [SerializeField] private float swipeThresholdAmount = 1f;
        [SerializeField] private int heightAmount = 10;
        [SerializeField] private Color burnedColor = Color.gray;
        [SerializeField] private Color freezedColor = Color.blue;
        [SerializeField][Range(1, 30)] private int magnetModeActiveTime = 15;
        [SerializeField][Range(1, 30)] private int shieldModeActiveTime = 10;


        [Header("Player References")]
        [SerializeField] private MeshFilter meshFilter = null;
        [SerializeField] private MeshRenderer meshRenderer = null;
        [SerializeField] private MeshCollider meshCollider = null;
        [SerializeField] private Rigidbody playerRigid = null;
        [SerializeField] private Transform shieldTrans = null;
        [SerializeField] private ParticleSystem burnedEffect = null;
        [SerializeField] private ParticleSystem freezedEffect = null;
        [SerializeField] private LayerMask platformLayerMask = new LayerMask();
        [SerializeField] private LayerMask obstacleLayerMask = new LayerMask();
        [SerializeField] private LayerMask finishLayerMask = new LayerMask();


        [Header("QuestionCanvas")]
        public Transform canvasQuestion;


        public PlayerState PlayerState
        {
            get { return playerState; }
            private set
            {
                if (value != playerState)
                {
                    value = playerState;
                    PlayerStateChanged(playerState);
                }
            }
        }

        public bool IsActiveMagnetMode { private set; get; }
        public bool IsActiveShieldMode { private set; get; }



        private PlayerState playerState = PlayerState.Player_Prepare;
        private PlayerParamsConfiguration playerParamsConfig = null;
        private Transform savedPlatformTrans = null;
        private Color originalColor = Color.white;
        private RaycastHit raycastHit;
        private float movementSpeed = 0;
        private float firstInputX = 0;
        private bool isFlipping = false;
        private bool isStopControl = false;


        private void OnEnable()
        {
            IngameManager.IngameStateChanged += IngameManager_IngameStateChanged;
        }
        private void OnDisable()
        {
            IngameManager.IngameStateChanged -= IngameManager_IngameStateChanged;
        }
        private void IngameManager_IngameStateChanged(IngameState obj)
        {
            if (obj == IngameState.Ingame_Playing)
            {
                PlayerLiving();
            }
            else if (obj == IngameState.Ingame_CompleteLevel)
            {
                PlayerCompletedLevel();
            }
        }




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



        private void Start()
        {
            canvasQuestion = GameObject.Find("CanvasQuestion").GetComponent<Transform>();
            canvasQuestion.transform.gameObject.SetActive(false);
            //Fire event
            PlayerState = PlayerState.Player_Prepare;
            playerState = PlayerState.Player_Prepare;

            //Add other actions here

            //Setup character
            CharacterInforController charControl = ServicesManager.Instance.CharacterContainer.CharacterInforControllers[ServicesManager.Instance.CharacterContainer.SelectedCharacterIndex];
            meshFilter.mesh = charControl.Mesh;
            meshRenderer.material = charControl.Material;
            meshCollider.sharedMesh = charControl.Mesh;

            //Setup parameters and objects
            IsActiveMagnetMode = false;
            IsActiveShieldMode = false;
            isStopControl = true;
            playerRigid.isKinematic = true;
            shieldTrans.gameObject.SetActive(false);
            burnedEffect.gameObject.SetActive(false);
            freezedEffect.gameObject.SetActive(false);
            originalColor = meshRenderer.material.color;
        }

        private void Update()
        {
            if (playerState == PlayerState.Player_Living)
            {
                if (!isStopControl)
                {
                    //Move the player forward
                    transform.position += Vector3.forward * movementSpeed * Time.deltaTime;


                    if (Input.GetMouseButtonDown(0))
                    {
                        firstInputX = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)).x;
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        float currentInputX = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)).x;
                        float amount = currentInputX - firstInputX;
                        if (Mathf.Abs(amount) >= swipeThresholdAmount && !isFlipping)
                        {
                            if (currentInputX < firstInputX && Mathf.RoundToInt(transform.position.x) >= 0) //Flip the player to left
                            {
                                if (Mathf.RoundToInt(transform.position.x) > -6)
                                {
                                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerJumped);

                                    isFlipping = true;
                                    Vector3 endPos = transform.position + Vector3.left * 5f;
                                    StartCoroutine(CRSwitchLane(endPos, Vector3.forward));
                                }
                            }
                            else if (currentInputX > firstInputX && Mathf.RoundToInt(transform.position.x) <= 0) //Flip the player to right
                            {
                                if (Mathf.RoundToInt(transform.position.x) < 6)
                                {
                                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerJumped);

                                    isFlipping = true;
                                    Vector3 endPos = transform.position + Vector3.right * 5f;
                                    StartCoroutine(CRSwitchLane(endPos, Vector3.back));
                                }
                            }
                        }
                    }


                    if (!isFlipping)
                    {
                        if (!IsActiveShieldMode)
                        {
                            //Check collide with obstacle
                            Collider[] obstacleColliders = Physics.OverlapBox(meshRenderer.transform.position, new Vector3(0.8f, 1f, 1.25f), transform.rotation, obstacleLayerMask);
                            if (obstacleColliders.Length > 0)
                            {
                                isStopControl = true;
                                StartCoroutine(CRHandleCollidedObstacle(obstacleColliders[0].transform.root.GetComponent<ObstacleController>()));
                            }
                        }


                        //Check if player is still surfing on a platform
                        bool isHitPlatform = Physics.Raycast(transform.position + Vector3.up, Vector3.down, out raycastHit, 10f, platformLayerMask);
                        if (!isHitPlatform)
                        {
                            isStopControl = true;
                            StartCoroutine(CRForcePlayerFall());
                        }
                        else
                        {
                            //Save the current platform
                            if (savedPlatformTrans != raycastHit.collider.transform.root)
                            {
                                savedPlatformTrans = raycastHit.collider.transform.root;
                            }
                        }
                    }




                    //Check collider with finish line
                    Collider[] finishColliders = Physics.OverlapBox(meshRenderer.transform.position, meshRenderer.bounds.extents, transform.rotation, finishLayerMask);
                    if (finishColliders.Length > 0)
                    {

                        //add questionn here
                        //if finish line.
                        isStopControl = true;
                        canvasQuestion.transform.gameObject.SetActive(true);
                        ServicesManager.Instance.SoundManager.musicSource.Stop();
                        //IngameManager.Instance.CompletedLevel();
                        //StartCoroutine(CRHandleActionsReachedFinishLine());
                    }
                }
            }
        }



        /// <summary>
        /// Call PlayerState.Player_Living event and handle other actions.
        /// </summary>
        private void PlayerLiving()
        {
            //Fire event
            PlayerState = PlayerState.Player_Living;
            playerState = PlayerState.Player_Living;

            //Add other actions here
            if (IngameManager.Instance.IsRevived)
            {
                StartCoroutine(CRHandleActionsAfterRevived());
            }
            else
            {
                isStopControl = false;
                StartCoroutine(CRIncreaseMovementSpeed());
            }
        }


        /// <summary>
        /// Call PlayerState.Player_Died event and handle other actions.
        /// </summary>
        public void PlayerDied()
        {
            //Fire event
            PlayerState = PlayerState.Player_Died;
            playerState = PlayerState.Player_Died;

            //Add other actions here
            ServicesManager.Instance.ShareManager.CreateScreenshot();
            shieldTrans.gameObject.SetActive(false);
        }



        /// <summary>
        /// Fire Player_CompletedLevel event and handle other actions.
        /// </summary>
        private void PlayerCompletedLevel()
        {
            //Fire event
            PlayerState = PlayerState.Player_CompletedLevel;
            playerState = PlayerState.Player_CompletedLevel;

            //Add others action here
            ServicesManager.Instance.ShareManager.CreateScreenshot();
        }


        /// <summary>
        /// Coroutine handle actions after player revived.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRHandleActionsAfterRevived()
        {
            IsActiveMagnetMode = false;
            IsActiveShieldMode = false;
            meshRenderer.enabled = true;
            meshRenderer.material.color = originalColor;
            playerRigid.isKinematic = true;
            transform.eulerAngles = Vector3.zero;
            transform.position = savedPlatformTrans.position;
            transform.forward = Vector3.forward;
            burnedEffect.gameObject.SetActive(false);
            freezedEffect.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            isStopControl = false;
            isFlipping = false;
            StartCoroutine(CRIncreaseMovementSpeed());
        }




        /// <summary>
        /// Handle actions when player colliding with an obstacle.
        /// </summary>
        /// <param name="obstacleController"></param>
        /// <returns></returns>
        private IEnumerator CRHandleCollidedObstacle(ObstacleController obstacleController)
        {
            if (obstacleController.ObstacleType == ObstacleType.SPIKE_OBSTACLE || obstacleController.ObstacleType == ObstacleType.SPINNER_OBSTACLE)
            {
                ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerExploded);


                meshRenderer.enabled = false;
                CameraController.Instance.Shake();
                EffectManager.Instance.CreatePlayerExplodeEffect(meshFilter.sharedMesh, transform.position);

                yield return new WaitForSeconds(0.5f);
                PlayerDied();
                IngameManager.Instance.HandlePlayerDied();
            }
            else if (obstacleController.ObstacleType == ObstacleType.FIRE_OBSTACLE)
            {
                ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerBurned);


                meshRenderer.material.color = burnedColor;
                burnedEffect.gameObject.SetActive(true);
                burnedEffect.Play();

                yield return new WaitForSeconds(1f);
                PlayerDied();
                IngameManager.Instance.HandlePlayerDied();
            }
            else if (obstacleController.ObstacleType == ObstacleType.ICE_OBSTACLE)
            {
                ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerFreezed);


                meshRenderer.material.color = freezedColor;
                freezedEffect.gameObject.SetActive(true);
                freezedEffect.Play();

                yield return new WaitForSeconds(1f);
                PlayerDied();
                IngameManager.Instance.HandlePlayerDied();
            }
        }




        /// <summary>
        /// Calculate position for flipping player
        /// </summary>
        /// <param name="t"></param>
        /// <param name="from"></param>
        /// <param name="middle"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 from, Vector3 middle, Vector3 to)
        {
            return Mathf.Pow((1 - t), 2) * transform.position + 2 * (1 - t) * t * middle + Mathf.Pow(t, 2) * to;
        }


        /// <summary>
        /// Get the calculated list of position for player to move through.
        /// </summary>
        /// <param name="endPos"></param>
        /// <returns></returns>
        private List<Vector3> GetCalculatedList(Vector3 endPos)
        {
            List<Vector3> listPositions = new List<Vector3>();
            Vector3 startPos = transform.position;
            Vector3 midPoint = Vector3.Lerp(startPos, endPos, 0.5f) + Vector3.up * heightAmount;
            listPositions.Add(transform.position);
            for (int i = 1; i <= playerParamsConfig.PlayerJumpingPoints; i++)
            {
                float t = i / (float)playerParamsConfig.PlayerJumpingPoints;
                listPositions.Add(CalculateQuadraticBezierPoint(t, startPos, midPoint, endPos));
            }
            return listPositions;
        }


        /// <summary>
        /// Coroutine switch the lane of the player.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRSwitchLane(Vector3 endPos, Vector3 rotateDirection)
        {
            List<Vector3> listPositions = GetCalculatedList(endPos);

            //Moving player through each point
            float tAngle = 360f / listPositions.Count;
            for (int i = 0; i < listPositions.Count; i++)
            {
                transform.position = new Vector3(listPositions[i].x, listPositions[i].y, transform.position.z);
                meshRenderer.transform.RotateAround(transform.position, rotateDirection, tAngle);
                yield return null;
            }


            bool isHitPlatform = Physics.Raycast(transform.position + Vector3.up, Vector3.down, out raycastHit, 10f, platformLayerMask);
            if (!isHitPlatform)
            {
                isStopControl = true;

                if (Physics.SphereCast(transform.position, 0.25f, transform.forward, out raycastHit, 1.5f, platformLayerMask))
                {
                    //Force player fall down back
                    yield return new WaitForFixedUpdate();
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerFalled);
                    playerRigid.isKinematic = false;
                    playerRigid.AddForce((-Vector3.forward + Vector3.down * 2f).normalized * 30f, ForceMode.Impulse);
                    playerRigid.AddTorque(-Vector3.right * 15f, ForceMode.Impulse);
                }
                else
                {
                    //Force player fall down forward
                    yield return new WaitForFixedUpdate();
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerFalled);
                    playerRigid.isKinematic = false;
                    playerRigid.AddForce((Vector3.forward + Vector3.down * 2f).normalized * 30f, ForceMode.Impulse);
                    playerRigid.AddTorque(Vector3.right * 15f, ForceMode.Impulse);
                }
                yield return new WaitForSeconds(0.5f);
                PlayerDied();
                IngameManager.Instance.HandlePlayerDied();
            }
            else
            {
                if (!IsActiveShieldMode) //Shield mode not active -> check for collding with obstacles
                {
                    //Check collide with obstacle
                    Collider[] obstacleColliders = Physics.OverlapBox(meshRenderer.transform.position, new Vector3(0.8f, 1f, 1.25f), transform.rotation, obstacleLayerMask);
                    if (obstacleColliders.Length > 0)
                    {
                        isStopControl = true;
                        StartCoroutine(CRHandleCollidedObstacle(obstacleColliders[0].transform.root.GetComponent<ObstacleController>()));
                    }
                    else
                    {
                        //Keep moving because player didn't collide with any obstacle
                        yield return null;
                        ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerLanded);
                        EffectManager.Instance.CreateSquareEffect(transform.position);
                        isFlipping = false;
                    }
                }
                else //Keep moving because Shield mode of the player is still active
                {
                    yield return null;
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerLanded);
                    EffectManager.Instance.CreateSquareEffect(transform.position);
                    isFlipping = false;
                }
            }
        }



        /// <summary>
        /// Coroutine add force to make player fall.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRForcePlayerFall()
        {
            yield return new WaitForFixedUpdate();
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerFalled);

            playerRigid.isKinematic = false;
            playerRigid.AddForce((Vector3.forward + Vector3.down * 1.5f).normalized * 30f, ForceMode.Impulse);
            playerRigid.AddTorque(Vector3.right * 15f, ForceMode.Impulse);
            yield return new WaitForSeconds(0.5f);
            PlayerDied();
            IngameManager.Instance.HandlePlayerDied();
        }




        /// <summary>
        /// Coroutine increase the movement speed of the player.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRIncreaseMovementSpeed()
        {
            movementSpeed = 0;
            float increaseTime = 1f;
            float t = 0;
            while (t < increaseTime)
            {
                t += Time.deltaTime;
                float factor = t / increaseTime;
                movementSpeed = Mathf.Lerp(0, playerParamsConfig.PlayerMovementSpeed, factor);
                yield return null;
            }
            movementSpeed = playerParamsConfig.PlayerMovementSpeed;
        }



        /// <summary>
        /// Coroutine handle actions when player reached finish line.
        /// </summary>
        /// <returns></returns>
        public IEnumerator CRHandleActionsReachedFinishLine()
        {

            float timeCount = 6;
            while (timeCount > 0)
            {
                timeCount -= Time.deltaTime;
                transform.position += transform.forward * playerParamsConfig.PlayerMovementSpeed * Time.deltaTime;
                yield return null;
            }

        }

        public IEnumerator EndingTheRound()
        {
            canvasQuestion.GetComponent<CanvasGroup>().alpha = 0f;
            yield return new WaitForSeconds(1.2f);
            canvasQuestion.GetComponent<Transform>().gameObject.SetActive(false);
        }






        /// <summary>
        /// Coroutine count down the magnet mode.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRCountDownMagnetMode()
        {
            float activeTime = magnetModeActiveTime;
            float maxActiveTime = activeTime;
            ViewManager.Instance.IngameViewController.UpdateMagnetModeActiveTime(maxActiveTime, activeTime);
            while (activeTime > 0)
            {
                yield return null;
                activeTime -= Time.deltaTime;
                ViewManager.Instance.IngameViewController.UpdateMagnetModeActiveTime(maxActiveTime, activeTime);
                if (playerState != PlayerState.Player_Living)
                {
                    break;
                }
            }
            IsActiveMagnetMode = false;
            ViewManager.Instance.IngameViewController.DisableMagnetModePanel();
        }


        /// <summary>
        /// Coroutine count down the shield mode.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRCountDownShieldMode()
        {
            float activeTime = shieldModeActiveTime;
            float maxActiveTime = activeTime;
            ViewManager.Instance.IngameViewController.UpdateShieldModeActiveTime(maxActiveTime, activeTime);
            while (activeTime > 0)
            {
                yield return null;
                activeTime -= Time.deltaTime;
                ViewManager.Instance.IngameViewController.UpdateShieldModeActiveTime(maxActiveTime, activeTime);
                if (playerState != PlayerState.Player_Living)
                {
                    break;
                }
            }

            shieldTrans.gameObject.SetActive(false);
            IsActiveShieldMode = false;
            ViewManager.Instance.IngameViewController.DisableShieldModePanel();
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////Public functions



        /// <summary>
        /// Setup player params.
        /// </summary>
        /// <param name="playerParams"></param>
        public void SetPlayerParams(PlayerParamsConfiguration playerParams)
        {
            playerParamsConfig = playerParams;
        }




        /// <summary>
        /// Active the magnet mode for this player.
        /// </summary>
        public void ActiveMagnetMode()
        {
            if (!IsActiveMagnetMode)
            {
                IsActiveMagnetMode = true;
                StartCoroutine(CRCountDownMagnetMode());
            }
        }


        /// <summary>
        /// Active the shield mode for this player.
        /// </summary>
        public void ActiveShieldMode()
        {
            if (!IsActiveShieldMode)
            {
                IsActiveShieldMode = true;
                shieldTrans.gameObject.SetActive(true);
                StartCoroutine(CRCountDownShieldMode());
            }
        }

    }
}
