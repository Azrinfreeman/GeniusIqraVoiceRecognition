using UnityEngine;

namespace ClawbearGames
{
    public class HomeManager : MonoBehaviour
    {
        [Header("HomeManager References")]
        [SerializeField] private MeshFilter meshFilter = null;
        [SerializeField] private MeshRenderer meshRenderer = null;

        private void Awake()
        {
            //Set current level
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.PPK_SAVED_LEVEL))
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, 1);
            }

            if(!PlayerPrefs.HasKey(PlayerPrefsKeys.PPK_SAVED_ROUND)){
                PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_ROUND,1);
            }
        }


        private void Start()
        {
            Application.targetFrameRate = 60;
            ViewManager.Instance.OnShowView(ViewType.HOME_VIEW);

            //Setup character
            CharacterInforController charControl = ServicesManager.Instance.CharacterContainer.CharacterInforControllers[ServicesManager.Instance.CharacterContainer.SelectedCharacterIndex];
            meshFilter.mesh = charControl.Mesh;
            meshRenderer.material = charControl.Material;
        }
    }
}
