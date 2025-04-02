using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ClawbearGames
{
    public class EffectManager : MonoBehaviour
    {

        public static EffectManager Instance { private set; get; }

        [SerializeField] private ParticleSystem collectCoinItemEffectPrefab = null;
        [SerializeField] private ParticleSystem collectMagnetItemEffectPrefab = null;
        [SerializeField] private ParticleSystem collectShieldItemEffectPrefab = null;
        [SerializeField] private ParticleSystem playerExplodeEffectPrefab = null;
        [SerializeField] private SquareController squareControllerPrefab = null;

        private List<ParticleSystem> listCollectCoinItemEffect = new List<ParticleSystem>();
        private List<ParticleSystem> listCollectMagnetItemEffect = new List<ParticleSystem>();
        private List<ParticleSystem> listCollectShieldItemEffect = new List<ParticleSystem>();
        private List<ParticleSystem> listPlayerExplodeEffect = new List<ParticleSystem>();
        private List<SquareController> listSquareController = new List<SquareController>();

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


        /// <summary>
        /// Play the given particle then disable it 
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        private IEnumerator CRPlayParticle(ParticleSystem par)
        {
            par.Play();
            yield return new WaitForSeconds(2f);
            par.gameObject.SetActive(false);
        }



        /// <summary>
        /// Play a collect coin effect at given position.
        /// </summary>
        /// <param name="pos"></param>
        public void PlayCollectCoinItemEffect(Vector3 pos)
        {
            //Find in the list
            ParticleSystem collectCoinItemEffect = listCollectCoinItemEffect.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (collectCoinItemEffect == null)
            {
                //Didn't find one -> create new one
                collectCoinItemEffect = Instantiate(collectCoinItemEffectPrefab, pos, Quaternion.identity);
                collectCoinItemEffect.gameObject.SetActive(false);
                listCollectCoinItemEffect.Add(collectCoinItemEffect);
            }

            collectCoinItemEffect.transform.position = pos;
            collectCoinItemEffect.gameObject.SetActive(true);
            StartCoroutine(CRPlayParticle(collectCoinItemEffect));
        }


        /// <summary>
        /// Play a collect magnet effect at given position.
        /// </summary>
        /// <param name="pos"></param>
        public void PlayCollectMagnetItemEffect(Vector3 pos)
        {
            //Find in the list
            ParticleSystem collectMagnetItemEffect = listCollectMagnetItemEffect.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (collectMagnetItemEffect == null)
            {
                //Didn't find one -> create new one
                collectMagnetItemEffect = Instantiate(collectMagnetItemEffectPrefab, pos, Quaternion.identity);
                collectMagnetItemEffect.gameObject.SetActive(false);
                listCollectMagnetItemEffect.Add(collectMagnetItemEffect);
            }

            collectMagnetItemEffect.transform.position = pos;
            collectMagnetItemEffect.gameObject.SetActive(true);
            StartCoroutine(CRPlayParticle(collectMagnetItemEffect));
        }



        /// <summary>
        /// Play a collect shield item effect at given position.
        /// </summary>
        /// <param name="pos"></param>
        public void PlayCollectShieldItemEffect(Vector3 pos)
        {
            //Find in the list
            ParticleSystem collectShieldItemEffect = listCollectShieldItemEffect.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (collectShieldItemEffect == null)
            {
                //Didn't find one -> create new one
                collectShieldItemEffect = Instantiate(collectShieldItemEffectPrefab, pos, Quaternion.identity);
                collectShieldItemEffect.gameObject.SetActive(false);
                listCollectShieldItemEffect.Add(collectShieldItemEffect);
            }

            collectShieldItemEffect.transform.position = pos;
            collectShieldItemEffect.gameObject.SetActive(true);
            StartCoroutine(CRPlayParticle(collectShieldItemEffect));
        }


        /// <summary>
        /// Create the player explode effect with given mesh and position.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="pos"></param>
        public void CreatePlayerExplodeEffect(Mesh mesh, Vector3 pos)
        {
            //Find in the list
            ParticleSystem obstacleExplodeEffect = listPlayerExplodeEffect.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (obstacleExplodeEffect == null)
            {
                //Didn't find one -> create new one
                obstacleExplodeEffect = Instantiate(playerExplodeEffectPrefab, Vector3.zero, Quaternion.identity);
                obstacleExplodeEffect.gameObject.SetActive(false);
                listPlayerExplodeEffect.Add(obstacleExplodeEffect);
            }

            obstacleExplodeEffect.transform.position = pos;
            var shape = obstacleExplodeEffect.shape;
            shape.mesh = mesh;
            shape.rotation = PlayerController.Instance.transform.eulerAngles;
            obstacleExplodeEffect.gameObject.SetActive(true);
            StartCoroutine(CRPlayParticle(obstacleExplodeEffect));
        }



        /// <summary>
        /// Create a square effect at given position.
        /// </summary>
        /// <param name="pos"></param>
        public void CreateSquareEffect(Vector3 pos)
        {
            //Find in the list
            SquareController squareController = listSquareController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (squareController == null)
            {
                //Didn't find one -> create new one
                squareController = Instantiate(squareControllerPrefab, pos, Quaternion.identity);
                squareController.gameObject.SetActive(false);
                listSquareController.Add(squareController);
            }

            squareController.transform.position = pos;
            squareController.gameObject.SetActive(true);
            squareController.ScaleAndFadeOut();
        }
    }
}