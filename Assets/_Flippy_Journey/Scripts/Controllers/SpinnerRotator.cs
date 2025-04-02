using System.Collections;
using UnityEngine;

namespace ClawbearGames
{
    public class SpinnerRotator : MonoBehaviour
    {
        [Header("Rotate Speed Configuration")]
        [SerializeField] private float minRotatingSpeed = 20f;
        [SerializeField] private float maxRotatingSpeed = 150f;

        [Header("Spinner Trans References")]
        [SerializeField] private Transform spinnerTrans = null;


        private void OnEnable()
        {
            StartCoroutine(CRRotate());
        }


        /// <summary>
        /// Coroutine rotate the spinner.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRRotate()
        {
            yield return null;
            float rotatingSpeed = Random.Range(minRotatingSpeed, maxRotatingSpeed);
            bool isRotateLeft = Random.value <= 0.5f ? true : false;
            while (gameObject.activeSelf)
            {
                spinnerTrans.localEulerAngles += isRotateLeft ? Vector3.up : (-Vector3.up) * rotatingSpeed * Time.deltaTime;
                yield return null;
            }
        }
    }
}
