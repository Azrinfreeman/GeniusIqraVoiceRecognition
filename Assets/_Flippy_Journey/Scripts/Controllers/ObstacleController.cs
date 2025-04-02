using UnityEngine;

namespace ClawbearGames
{
    public class ObstacleController : MonoBehaviour
    {
        [Header("Obstacle Configuration")]
        [SerializeField] private ObstacleType obstacleType = ObstacleType.SPIKE_OBSTACLE;
        [SerializeField] private Vector3 obstacleSize = Vector3.one;

        public ObstacleType ObstacleType { get { return obstacleType; } }
        public float HalfZSize { get { return obstacleSize.z / 2f; } }


        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, obstacleSize);
        }

        private void OnEnable()
        {
            if (obstacleType == ObstacleType.FIRE_OBSTACLE || obstacleType == ObstacleType.ICE_OBSTACLE)
            {
                transform.localPosition += Vector3.up * 0.01f;
            }
        }


        private void Update()
        {
            if (PlayerController.Instance.PlayerState == PlayerState.Player_Living)
            {
                //Check and disable this obstacle
                if (PlayerController.Instance.transform.position.z > transform.position.z + 10f)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
