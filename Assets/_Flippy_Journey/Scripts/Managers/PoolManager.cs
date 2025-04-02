using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ClawbearGames
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { private set; get; }

        [SerializeField] private ItemController[] itemControllerPrefabs = null;
        [SerializeField] private ObstacleController[] obstacleControllerPrefabs = null;
        [SerializeField] private PlatformPrefabConfiguration[] platformPrefabConfigurations = null;


        private List<ItemController> listItemController = new List<ItemController>();
        private List<ObstacleController> listObstacleController = new List<ObstacleController>();
        private List<PlatformController> listPlatformController = new List<PlatformController>();

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
        /// Get PlatformController object that has PlatformSize is HUGE_PLATFORM and given platformType.
        /// </summary>
        /// <param name="platformType"></param>
        /// <returns></returns>
        public PlatformController GetHugePlatformController(PlatformType platformType)
        {
            foreach(PlatformPrefabConfiguration platformPrefab in platformPrefabConfigurations)
            {
                if (platformPrefab.PlatformType.Equals(platformType))
                    return platformPrefab.PlatformControllerPrefabs.Where(a => a.PlatformSize.Equals(PlatformSize.HUGE)).FirstOrDefault();
            }

            return platformPrefabConfigurations[0].PlatformControllerPrefabs.Where(a => a.PlatformSize.Equals(PlatformSize.HUGE)).FirstOrDefault();
        }




        /// <summary>
        /// Get an inactive ItemController with given type.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public ItemController GetItemController(ItemType itemType)
        {
            //Find in the list
            ItemController itemController = listItemController.Where(a => !a.gameObject.activeSelf && a.ItemType.Equals(itemType)).FirstOrDefault();

            if (itemController == null)
            {
                //Did not find one -> create new one
                ItemController prefab = itemControllerPrefabs.Where(a => a.ItemType.Equals(itemType)).FirstOrDefault();
                itemController = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                itemController.gameObject.SetActive(false);
                listItemController.Add(itemController);
            }

            return itemController;
        }



        /// <summary>
        /// Get an inactive ObstacleController object with given type.
        /// </summary>
        /// <param name="obstacleType"></param>
        /// <returns></returns>
        public ObstacleController GetObstacleController(ObstacleType obstacleType)
        {
            //Find in the list
            ObstacleController obstacleController = listObstacleController.Where(a => !a.gameObject.activeSelf && a.ObstacleType.Equals(obstacleType)).FirstOrDefault();

            if (obstacleController == null)
            {
                //Did not find one -> create new one
                ObstacleController prefab = obstacleControllerPrefabs.Where(a => a.ObstacleType.Equals(obstacleType)).FirstOrDefault();
                obstacleController = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                obstacleController.gameObject.SetActive(false);
                listObstacleController.Add(obstacleController);
            }

            return obstacleController;
        }



        /// <summary>
        /// Get an inactive PlatformController object with given type and size.
        /// </summary>
        /// <param name="platformType"></param>
        /// <param name="platformSize"></param>
        /// <returns></returns>
        public PlatformController GetPlatformController(PlatformType platformType, PlatformSize platformSize)
        {
            //Find in the list
            PlatformController platformController = listPlatformController.Where(a => !a.gameObject.activeSelf && a.PlatformType.Equals(platformType) && a.PlatformSize.Equals(platformSize)).FirstOrDefault();

            if (platformController == null)
            {
                //Did not find one -> create new one
                PlatformController[] prefabs = platformPrefabConfigurations.Where(a => a.PlatformType.Equals(platformType)).FirstOrDefault().PlatformControllerPrefabs;
                PlatformController prefab = prefabs.Where(a => a.PlatformSize.Equals(platformSize)).FirstOrDefault();
                platformController = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                listPlatformController.Add(platformController);
            }

            return platformController;
        }

    }
}
