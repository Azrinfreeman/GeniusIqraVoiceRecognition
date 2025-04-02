using System.Collections;
using UnityEngine;

namespace ClawbearGames
{
    public class SquareController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer = null;


        /// <summary>
        /// Scale this square and fade out.
        /// </summary>
        public void ScaleAndFadeOut()
        {
            transform.localScale = new Vector3(1, 0.1f, 1);
            StopAllCoroutines();
            StartCoroutine(CRScaleAndFadeOut());
        }



        /// <summary>
        /// Coroutine scale up and fade out.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRScaleAndFadeOut()
        {
            float fadingTime = 1f;
            float t = 0;
            Color startColor = meshRenderer.material.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
            Vector3 startScale = transform.localScale;
            Vector3 endScale = new Vector3(2f, startScale.y, 2f);
            while (t < fadingTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.Liner, t / fadingTime);
                meshRenderer.material.color = Color.Lerp(startColor, endColor, factor);
                transform.localScale = Vector3.Lerp(startScale, endScale, factor);
                yield return null;
            }

            meshRenderer.material.color = endColor;
            yield return new WaitForSeconds(0.25f);
            transform.localScale = startScale;
            meshRenderer.material.color = startColor;
            gameObject.SetActive(false);
        }
    }
}
