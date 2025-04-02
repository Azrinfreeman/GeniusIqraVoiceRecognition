using System.Collections;
using UnityEngine;


namespace ClawbearGames
{
    public class ItemController : MonoBehaviour
    {

        [Header("Item Configuration")]
        [SerializeField] private float minRotateSpeed = 150f;
        [SerializeField] private float maxRotateSpeed = 350f;

        [Header("Item References")]
        [SerializeField] private ItemType itemType = ItemType.COIN;
        [SerializeField] private MeshRenderer meshRenderer = null;
        [SerializeField] private LayerMask playerLayerMask = new LayerMask();

        public ItemType ItemType { get { return itemType; } }
        private float rotatingSpeed = 0;


        public void OnEnable()
        {
            rotatingSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
            if (itemType == ItemType.COIN)
            {
                StartCoroutine(CRMoveToPlayer());
            }
        }


        private void Update()
        {
            transform.eulerAngles += Vector3.up * rotatingSpeed * Time.deltaTime;

            //Check collide with player
            Collider[] colliders = Physics.OverlapBox(meshRenderer.bounds.center, meshRenderer.bounds.extents, transform.rotation, playerLayerMask);
            if (colliders.Length > 0)
            {
                if (itemType == ItemType.COIN)
                {
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.CollectCoinItem);
                    ServicesManager.Instance.CoinManager.AddCollectedCoins(1);
                    EffectManager.Instance.PlayCollectCoinItemEffect(meshRenderer.bounds.center);
                    transform.SetParent(null);
                    gameObject.SetActive(false);
                }
                else if (itemType == ItemType.MAGNET)
                {
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.EnableMagnetMode);
                    ServicesManager.Instance.CoinManager.AddCollectedCoins(1);
                    EffectManager.Instance.PlayCollectMagnetItemEffect(meshRenderer.bounds.center);
                    PlayerController.Instance.ActiveMagnetMode();
                    transform.SetParent(null);
                    gameObject.SetActive(false);
                }
                else if (itemType == ItemType.SHIELD)
                {
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.EnableShieldMode);
                    EffectManager.Instance.PlayCollectShieldItemEffect(meshRenderer.bounds.center);
                    PlayerController.Instance.ActiveShieldMode();
                    transform.SetParent(null);
                    gameObject.SetActive(false);
                }
            }


            if (PlayerController.Instance.PlayerState == PlayerState.Player_Living)
            {
                //Check and disable this item
                if (PlayerController.Instance.transform.position.z > transform.position.z + 10f)
                {
                    gameObject.SetActive(false);
                }
            }
        }



        /// <summary>
        /// Coroutine move this coin to player when magnet mode of the player is active.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRMoveToPlayer()
        {
            yield return null;
            while (true)
            {
                if (PlayerController.Instance.IsActiveMagnetMode)
                {
                    float yDistance = transform.position.z - PlayerController.Instance.transform.position.z;
                    if (yDistance > 0 && yDistance <= 30)
                    {
                        break;
                    }
                }
                yield return null;
            }


            while (IngameManager.Instance.IngameState == IngameState.Ingame_Playing)
            {
                Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
                transform.position += direction * 150f * Time.deltaTime;
                yield return null;
            }
        }

    }
}
