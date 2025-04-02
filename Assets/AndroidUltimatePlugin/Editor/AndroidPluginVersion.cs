using UnityEditor;
using UnityEngine;

namespace Gigadrillgames.AUP.ScriptableObjects
{
    public class AndroidPluginVersion : EditorWindow
    {
        #region Fields

        private static BuildConfig _buildConfig;
        private static string _version;

        #endregion Fields


        #region Methods
        // This method will be called on load or recompile
        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            // if no data exists yet create and reference a new instance
            if (!_buildConfig)
            {
                // as first option check if maybe there is an instance already
                // and only the reference got lost
                // won't work ofcourse if you moved it elsewhere ...
                _buildConfig = AssetDatabase.LoadAssetAtPath<BuildConfig>("Assets/AndroidUltimatePlugin/Config/BuildConfig.asset");

                // if that was successful we are done
                if(_buildConfig) return;

                // otherwise create and reference a new instance
                _buildConfig = CreateInstance<BuildConfig>();

                AssetDatabase.CreateAsset(_buildConfig, "Assets/AndroidUltimatePlugin/Config/BuildConfig.asset");
                AssetDatabase.Refresh();
            }
        }


        [MenuItem("Window/AUP/About")]
        static void ShowWindow()
        {
            AndroidPluginVersion window = CreateInstance<AndroidPluginVersion>();
            window.titleContent = new GUIContent("Android Ultimate Plugin");
            Vector2 size = new Vector2(200, 135);
            window.maxSize = size;
            window.minSize = size;
            window.ShowUtility();
        }

        void OnGUI()
        {
            // Note that going through the SerializedObject
            // and SerilaizedProperties is the better way of doing this!
            // 
            // Not only will Unity automatically handle the set dirty and saving
            // but it also automatically adds Undo/Redo functionality!
            var serializedObject = new SerializedObject(_buildConfig);

            // fetches the values of the real instance into the serialized one
            serializedObject.Update();

            // get the Id field
            var version = serializedObject.FindProperty("Version");
            
            GUILayout.Space(5);
            EditorGUILayout.LabelField($"Version: {version.stringValue}");
            GUILayout.Space(15);

            if (GUILayout.Button("About"))
            {
                Application.OpenURL("https://gigadrillgames.com/2020/03/13/android-ultimate-plugin/");
            }

            GUILayout.Space(2);
            if (GUILayout.Button("How to setup"))
            {
                Application.OpenURL(
                    "https://docs.google.com/document/d/1QHM7Kr_ThVn-Q-olzUumBuZZiAnJDnICQHg3y_Ovpos/edit?usp=sharing");
            }

            GUILayout.Space(2);
            if (GUILayout.Button("Video tutorial"))
            {
                Application.OpenURL("https://youtu.be/Xg7-uia7yes");
            }

            GUILayout.Space(2);
            if (GUILayout.Button("Features"))
            {
                Application.OpenURL("https://youtu.be/_vzlLWpUOyU");
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        #endregion Methods

    }
}