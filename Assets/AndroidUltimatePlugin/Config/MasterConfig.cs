using UnityEditor;

namespace Gigadrillgames.AUP.ScriptableObjects
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "MasterConfig", menuName = "ScriptableObjects/Config/MasterConfig", order = 0)]
    public class MasterConfig : ScriptableObject
    {
        public static MasterConfig Instance;
        [SerializeField] private BuildConfig _buildConfig;

        public static BuildConfig BuildConfig
        {
            get
            {
                CreateMasterConfig();
                return Instance._buildConfig;
            }
        }

        private static void CreateMasterConfig()
        {
            if (Instance == null)
            {
                
                // as first option check if maybe there is an instance already
                // and only the reference got lost
                // won't work ofcourse if you moved it elsewhere ...
#if UNITY_EDITOR
                if (!Instance)
                {
                    Instance = AssetDatabase.LoadAssetAtPath<MasterConfig>(
                        "Assets/AndroidUltimatePlugin/Resources/Config/MasterConfig.asset");
                }

                // if that wasn't successful we will create it instead
                if (!Instance)
                {
                    // otherwise create and reference a new instance
                    Instance = CreateInstance<MasterConfig>();
                    AssetDatabase.CreateAsset(Instance, "Assets/AndroidUltimatePlugin/Resources/Config/MasterConfig.asset");
                    AssetDatabase.Refresh();
                }
#elif UNITY_ANDROID
                if (!Instance)
            {
                MasterConfig[] foundObjects =
                    Resources.LoadAll<MasterConfig>("Config/MasterConfig");
                if (foundObjects.Length > 0)
                {
                    Instance = foundObjects[0];
                }
            }
#endif
            }
        }
    }
}