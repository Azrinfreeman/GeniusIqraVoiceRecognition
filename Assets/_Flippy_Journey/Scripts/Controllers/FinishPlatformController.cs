using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClawbearGames
{
    public class FinishPlatformController : MonoBehaviour
    {

        [SerializeField] private Transform confettiEffectsTrans = null;
        [SerializeField] private Text nextLevelText = null;
        [SerializeField] private ParticleSystem[] confettiEffects = null;
        [SerializeField] private Transform[] hugePlatformsTrans = null;

        private void OnEnable()
        {
            confettiEffectsTrans.gameObject.SetActive(false);
        }


        /// <summary>
        /// Move this finish platform up.
        /// <param name="hugePlatform"></param>
        /// </summary>
        public void MoveUp(PlatformController hugePlatform)
        {
            foreach (Transform trans in hugePlatformsTrans)
            {
                trans.GetComponent<MeshFilter>().sharedMesh = hugePlatform.GetComponent<MeshFilter>().sharedMesh;
                trans.GetComponent<MeshRenderer>().sharedMaterial = hugePlatform.GetComponent<MeshRenderer>().sharedMaterial;
            }

            nextLevelText.text = "Next Round: " + (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_ROUND) + 1).ToString();
            StartCoroutine(CRMoveUp());
        }


        /// <summary>
        /// Play the confetti effects.
        /// </summary>
        public void PlayEffect()
        {
            confettiEffectsTrans.gameObject.SetActive(true);
            foreach (ParticleSystem o in confettiEffects)
            {
                o.Play();
            }
        }



        /// <summary>
        /// Coroutine move this platform up.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRMoveUp()
        {
            Vector3 startPos = transform.position + Vector3.down * 10f;
            Vector3 endPos = transform.position;
            transform.position = startPos;

            float moveTime = 0.5f;
            float t = 0;
            while (t < moveTime)
            {
                t += Time.deltaTime;
                float factor = t / moveTime;
                transform.position = Vector3.Lerp(startPos, endPos, factor);
                yield return null;
            }
        }
    }
}
