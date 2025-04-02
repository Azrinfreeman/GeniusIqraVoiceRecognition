using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBGames
{
    public class MeshUpdater : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter = null;
        [SerializeField] private Mesh[] meshes = null;

        private void OnEnable()
        {
            StartCoroutine(CRUpdateMesh());
        }


        /// <summary>
        /// Coroutine update the mesh.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRUpdateMesh()
        {
            yield return null;
            float delay = 0.2f;
            int i = 0;
            while (gameObject.activeSelf)
            {
                meshFilter.mesh = meshes[i];
                yield return new WaitForSeconds(delay);
                i++;
                if (i == meshes.Length)
                    i = 0;
            }
        }
    }
}
