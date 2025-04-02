using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class PlatformController : MonoBehaviour
    {
        [SerializeField] private PlatformType platformType = PlatformType.GREEN;
        [SerializeField] private PlatformSize platformSize = PlatformSize.SMALL;
        [SerializeField] private MeshRenderer meshRenderer = null;


        public PlatformType PlatformType { get { return platformType; } }
        public PlatformSize PlatformSize { get { return platformSize; } }
        public float HalfSize { get { return meshRenderer.bounds.size.z / 2f; } }



        private void Update()
        {
            if (PlayerController.Instance.PlayerState == PlayerState.Player_Living)
            {
                //Check and disable this platform
                Vector3 toPlayer = PlayerController.Instance.transform.position - transform.position;
                if (Vector3.Dot(transform.forward, toPlayer) > 0 && toPlayer.magnitude > 20f)
                {
                    gameObject.SetActive(false);
                    IngameManager.Instance.CreateNextPlatform();
                }
            }
        }



        /// <summary>
        /// Setup this platform
        /// </summary>
        /// <param name="platformParams"></param>
        public void OnSetup(PlatformParamsConfiguration platformParams)
        {

            List<Vector3> listItemPosition = new List<Vector3>();
            listItemPosition.Add(transform.position);
            for(int i = 1; i <= ItemPosAmount(); i++)
            {
                listItemPosition.Add(transform.position + Vector3.forward * i);
                listItemPosition.Add(transform.position - Vector3.forward * i);
            }
            List<int> listUsedIndex = new List<int>();


            //Create coin items
            int coinItemAmount = platformParams.CoinItemAmount;
            while (coinItemAmount > 0)
            {
                int posIndex = Random.Range(0, listItemPosition.Count);
                while (listUsedIndex.Contains(posIndex))
                {
                    posIndex = Random.Range(0, listItemPosition.Count);
                }
                listUsedIndex.Add(posIndex);

                ItemController itemController = PoolManager.Instance.GetItemController(ItemType.COIN);
                itemController.transform.position = listItemPosition[posIndex];
                itemController.transform.SetParent(transform);
                itemController.gameObject.SetActive(true);
                coinItemAmount--;
            }


            //Create magnet item
            if (Random.value <= platformParams.MagnetItemFrequency)
            {
                int posIndex = Random.Range(0, listItemPosition.Count);
                while (listUsedIndex.Contains(posIndex))
                {
                    posIndex = Random.Range(0, listItemPosition.Count);
                }
                listUsedIndex.Add(posIndex);

                ItemController itemController = PoolManager.Instance.GetItemController(ItemType.MAGNET);
                itemController.transform.position = listItemPosition[posIndex];
                itemController.transform.SetParent(transform);
                itemController.gameObject.SetActive(true);
            }


            //Create shield item
            if (Random.value <= platformParams.ShieldItemFrequency)
            {
                int posIndex = Random.Range(0, listItemPosition.Count);
                while (listUsedIndex.Contains(posIndex))
                {
                    posIndex = Random.Range(0, listItemPosition.Count);
                }
                listUsedIndex.Add(posIndex);

                ItemController itemController = PoolManager.Instance.GetItemController(ItemType.SHIELD);
                itemController.transform.position = listItemPosition[posIndex];
                itemController.transform.SetParent(transform);
                itemController.gameObject.SetActive(true);
            }



            int obstacleAmount = GetObstacleAmount(platformParams.ListObstacleAmountConfig);
            if (obstacleAmount > 0 && !platformParams.IsLastPlatform)
            {
                bool isCreateObstacleForward = false;
                bool isCreateObstacleBack = false;
                while (obstacleAmount > 0)
                {
                    //Create an obstacle right here
                    ObstacleType obstacleType = platformParams.ListObstacleType[Random.Range(0, platformParams.ListObstacleType.Count)];
                    ObstacleController obstacleController = PoolManager.Instance.GetObstacleController(obstacleType);

                    if (isCreateObstacleForward)
                    {
                        obstacleController.transform.position = transform.position + Vector3.back * (HalfSize - obstacleController.HalfZSize);
                    }
                    else if (isCreateObstacleBack)
                    {
                        obstacleController.transform.position = transform.position + Vector3.forward * (HalfSize - obstacleController.HalfZSize);
                    }
                    else
                    {
                        if (Random.value <= 0.5f)
                        {
                            isCreateObstacleForward = true;
                            obstacleController.transform.position = transform.position + Vector3.back * (HalfSize - obstacleController.HalfZSize);
                        }
                        else
                        {
                            isCreateObstacleBack = true;
                            obstacleController.transform.position = transform.position + Vector3.forward * (HalfSize - obstacleController.HalfZSize);
                        }
                    }
                    obstacleController.transform.SetParent(transform);
                    obstacleController.gameObject.SetActive(true);
                    obstacleAmount--;
                }
            }
            StartCoroutine(CRMoveUp());
        }




        /// <summary>
        /// Get the amount of item position based on platformSize.
        /// </summary>
        /// <returns></returns>
        private int ItemPosAmount()
        {
            if (platformSize == PlatformSize.HUGE)
                return 6;
            else if (platformSize == PlatformSize.BIG)
                return 5;
            else if (platformSize == PlatformSize.MEDIUM)
                return 4;
            else if (platformSize == PlatformSize.NORMAL)
                return 3;
            else
                return 2;
        }



        public int GetObstacleAmount(List<ObstacleAmountConfiguration> obstacleAmountConfigs)
        {
            float totalFrequency = 0;
            foreach (ObstacleAmountConfiguration config in obstacleAmountConfigs)
            {
                totalFrequency += config.Frequency;
            }
            totalFrequency = Mathf.Clamp(totalFrequency, 1f, Mathf.Infinity);


            float randomFreq = Random.Range(0, totalFrequency);
            for (int i = 0; i < obstacleAmountConfigs.Count; i++)
            {
                if (randomFreq < obstacleAmountConfigs[i].Frequency)
                {
                    return obstacleAmountConfigs[i].ObstacleAmount;
                }
                else
                {
                    randomFreq -= obstacleAmountConfigs[i].Frequency;
                }
            }

            return 0;
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


            //Unparent all items
            ItemController[] itemControllers = GetComponentsInChildren<ItemController>();
            foreach(ItemController item in itemControllers)
            {
                item.transform.SetParent(null);
            }

            //Unparent all obstacles
            ObstacleController[] obstacleControllers = GetComponentsInChildren<ObstacleController>();
            foreach(ObstacleController obstacle in obstacleControllers)
            {
                obstacle.transform.SetParent(null);
            }
        }

    }
}
