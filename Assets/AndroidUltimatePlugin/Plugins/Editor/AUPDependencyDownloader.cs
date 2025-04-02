using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AndroidUltimatePlugin.Helpers.Editor
{
    public class AUPDependencyDownloader : EditorWindow
    {
        private static int _libraryCount = 0, _downloadedCount = 0, _downloadErrorCount = 0;
        private static bool _downloadStarted = false;

        private static readonly string[] Libraries = new[]
        {
            "https://gigadrillgames.com/aup-dependency/appcompat-v7-23.0.1.aar",
            "https://gigadrillgames.com/aup-dependency/core-1.1.0.aar",
            "https://gigadrillgames.com/aup-dependency/loader-1.0.0.aar",
            "https://gigadrillgames.com/aup-dependency/play-services-base-9.2.1.aar",
            "https://gigadrillgames.com/aup-dependency/play-services-basement-9.2.1.aar",
            "https://gigadrillgames.com/aup-dependency/play-services-games-9.2.1.aar",
            "https://gigadrillgames.com/aup-dependency/kaldi-android-5.2.aar",
            "https://gigadrillgames.com/aup-dependency/models-release.aar"
        };

        private static readonly string[] JarLibraries = new[]
        {
            "https://gigadrillgames.com/aup-dependency/universal-image-loader-1.9.5.jar"
        };

        [MenuItem("Window/AUP/Download Dependency")]
        public static void ShowWindow()
        {
            AUPDependencyDownloader window = CreateInstance<AUPDependencyDownloader>();
            window.titleContent = new GUIContent("Download Dependencies");
            Vector2 size =  new Vector2(350, 135);
            window.maxSize = size;
            window.minSize = size;
            window.ShowUtility();
            
            _libraryCount = Libraries.Length + 1;
            _downloadedCount = 0;
            _downloadErrorCount = 0;
            _downloadStarted = false;
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Download"))
            {
                Download();
            }
            
            if (GUILayout.Button("Delete all"))
            {
                DeleteAll();
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Progress:");
                EditorGUILayout.LabelField($"{_downloadedCount}/{_libraryCount}");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("ErrorCount:");
                EditorGUILayout.LabelField($"{_downloadErrorCount}");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Status:");
                if (_downloadStarted)
                {
                    EditorGUILayout.LabelField((_downloadedCount == _libraryCount)
                        ? "Download Complete!"
                        : "Downloading...");
                }
                else
                {
                    EditorGUILayout.LabelField("Click Download to start!");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private static async void Download()
        {
            _downloadStarted = true;
            _libraryCount = Libraries.Length + JarLibraries.Length;
            _downloadedCount = 0;
            _downloadErrorCount = 0;
            string pluginsAndroidDirectory = $"{Application.dataPath}/AndroidUltimatePlugin/Plugins/Android";
            string specialAndroidDirectory = $"{Application.dataPath}/Plugins/Android/aup.androidlib/libs";

            foreach (var path in Libraries)
            {
                string fileName = Path.GetFileName(path);
                //string fileExtension = Path.GetExtension(path);
                if (!File.Exists($"{pluginsAndroidDirectory}/{fileName}"))
                {
                    await DownloadFile(path,
                        fileName,pluginsAndroidDirectory);
                }
                else
                {
                    _downloadedCount++;
                    Debug.Log($"File: {fileName} already Exists!");
                }
            }

            foreach (var path in JarLibraries)
            {
                string fileName = Path.GetFileName(path);
                if (!File.Exists($"{specialAndroidDirectory}/{fileName}"))
                {
                    await DownloadFile(path,
                        fileName,specialAndroidDirectory);
                }
                else
                {
                    _downloadedCount++;
                    Debug.Log($"File: {fileName} already Exists!");
                }
            }
        }
        
        private static void DeleteAll()
        {
            string pluginsAndroidDirectory = $"{Application.dataPath}/AndroidUltimatePlugin/Plugins/Android";
            string specialAndroidDirectory = $"{Application.dataPath}/Plugins/Android/aup.androidlib/libs";
            int deletedCount = 0;
            
            foreach (var path in Libraries)
            {
                string fileName = Path.GetFileName(path);
                string filepath = $"{pluginsAndroidDirectory}/{fileName}";

                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                    deletedCount++;
                }
            }

            foreach (var path in JarLibraries)
            {
                string fileName = Path.GetFileName(path);
                string filepath = $"{specialAndroidDirectory}/{fileName}";
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                    deletedCount++;
                }
            }

            if (deletedCount>0)
            {
                AssetDatabase.Refresh();
                Debug.Log($"<color=blue> deleted: {deletedCount} library dependency.</color>");    
            }
            else
            {
                Debug.Log($"<color=blue> all dependency library is deleted.</color>");
            }
            
        }

        private static async Task DownloadFile(String url, string fileName, string directory)
        {
            try
            {
                // Create an instance of WebClient
                WebClient client = new WebClient();
                // Hookup DownloadFileCompleted Event
                client.DownloadProgressChanged +=
                    (object sender, DownloadProgressChangedEventArgs e) =>
                        onProgress($"{fileName}", e);

                client.DownloadFileCompleted +=
                    (object sender, AsyncCompletedEventArgs e) =>
                        onComplete($"{fileName}", e);
                
                Debug.Log($"<color=blue> Begin download {fileName} library.</color>");
                await client.DownloadFileTaskAsync(new Uri(url), $"{directory}/{fileName}");
            }
            catch (Exception e)
            {
                Debug.Log($"<color=red>Download Library Error Exception {e.ToString()}</color>");
            }
        }

        private static Action<string, DownloadProgressChangedEventArgs> onProgress =
            (string localFile, DownloadProgressChangedEventArgs e) =>
            {
                Debug.LogFormat("<color=blue>{0}: {1}/{2} bytes received ({3}%)</color>",
                    localFile, e.BytesReceived,
                    e.TotalBytesToReceive, e.ProgressPercentage);
            };

        private static Action<string, AsyncCompletedEventArgs> onComplete =
            (string localFile, AsyncCompletedEventArgs e) =>
            {
                if (e.Error != null)
                {
                    Debug.Log($"<color=red>Download {localFile} library failed.</color>");
                    _downloadErrorCount++;
                }
                else
                {
                    Debug.Log($"<color=green>Download {localFile} library Completed.</color>");
                    _downloadedCount++;
                    AssetDatabase.Refresh();
                }
            };
    }
}