using System.Collections;
using UnityEngine;

namespace ClawbearGames
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { private set; get; }

        [Header("Camera Configurations")]
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private float shakeDuration = 0.5f;
        [SerializeField] private float shakeAmount = 0.25f;
        [SerializeField] private float decreaseFactor = 1.5f;

        private Vector3 offset = Vector3.zero;
        private Vector3 velocity = Vector3.zero;

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
            offset = transform.position - PlayerController.Instance.transform.position;
        }

        private void LateUpdate()
        {
            if (PlayerController.Instance.PlayerState == PlayerState.Player_Living)
            {
                Vector3 targetPos = PlayerController.Instance.transform.position + offset;
                targetPos.y = transform.position.y;
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            }
        }


        /// <summary>
        /// Shake this camera.
        /// </summary>
        public void Shake()
        {
            StartCoroutine(CRShake());
        }


        /// <summary>
        /// Coroutine skae this camera.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRShake()
        {
            yield return new WaitForSeconds(0.15f);
            Vector3 originalPos = transform.position;
            float shakeDurationTemp = shakeDuration;
            while (shakeDurationTemp > 0)
            {
                Vector3 newPos = originalPos + Random.insideUnitSphere * shakeAmount;
                newPos.z = originalPos.z;
                transform.position = newPos;
                shakeDurationTemp -= Time.deltaTime * decreaseFactor;
                yield return null;
            }

            transform.position = originalPos;
        }
    }
}
