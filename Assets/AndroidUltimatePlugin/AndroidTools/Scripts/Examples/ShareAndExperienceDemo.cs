using System;
using Gigadrillgames.AUP.Common;
using UnityEngine;

namespace Gigadrillgames.AUP.Tools
{
    public class ShareAndExperienceDemo : MonoBehaviour
    {
        private bool isImmersive = false;
        private SharePlugin sharePlugin;
        private UtilsPlugin utilsPlugin;
        private MediaScannerPlugin mediaScannerPlugin;
        private Dispatcher dispatcher;

        // Use this for initialization
        void Start()
        {
            dispatcher = Dispatcher.GetInstance();

            utilsPlugin = UtilsPlugin.GetInstance();
            utilsPlugin.Init();
            utilsPlugin.SetDebug(0);

            sharePlugin = SharePlugin.GetInstance();
            sharePlugin.SetDebug(0);

            mediaScannerPlugin = MediaScannerPlugin.GetInstance();
            mediaScannerPlugin.SetDebug(0);
            mediaScannerPlugin.Init();
            mediaScannerPlugin.SetCallbackListener(onScanStarted, onScanComplete, onScanFail);
        }

        public void ImmersiveToggle()
        {
            if (!isImmersive)
            {
                utilsPlugin.ImmersiveOn(500);
                isImmersive = true;
            }
            else
            {
                utilsPlugin.ImmersiveOff();
                isImmersive = false;
            }
        }

        public void ShareText()
        {
            //share link
            sharePlugin.ShareUrl("my subject", "my subject content", "https://www.urltoshare.com");
        }

        public void ShareImage()
        {
            long unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Debug.Log("ShareImage unixTimeSeconds " + unixTimeSeconds);
            string screenShotName = "screenShot" + "_" + unixTimeSeconds;
            Debug.Log(" screenShotName " + screenShotName);
            string folderPath = utilsPlugin.CreateFolder("AUPScreenShot", 0);
            string pathToSave = "";

            if (!folderPath.Equals("", StringComparison.Ordinal))
            {
                // test create no media file
                // uncomment this if you don't want the screenshot to appear in gallery
                /*string nomediaPath = utilsPlugin.WriteNoMediaFile(folderPath);
                Debug.Log( " nomediaPath " + nomediaPath );
                utilsPlugin.ShowToast( " nomediaPath " + nomediaPath);*/

                pathToSave = folderPath + "/" + screenShotName;
                Debug.Log("[AUP] ShareImage pathToSave " + pathToSave);
                //if you want to save on Application.persistentDataPath use this
                //file on persistentDataPath path is remove when app is uninstal
                //pathToSave = Application.persistentDataPath + "/" + screenShotName;

                //note: we added new required variable to pass which is screenShotName to determined what image format to use
                //jpg or png
                string mimeType = "image/jpeg";
                ImageType imageType = ImageType.JPG;
                string formatExtension = null;

                if (imageType == ImageType.JPG)
                {
                    formatExtension = ".jpg";
                }
                else if (imageType == ImageType.PNG)
                {
                    formatExtension = ".png";
                }
                else
                {
                    formatExtension = ".jpg";
                }

                // the actual screenshot
                StartCoroutine(Utils.TakeScreenshot(pathToSave + formatExtension, ImageType.JPG));
                // share it
                sharePlugin.ShareImage("subject", "subjectContent", pathToSave + formatExtension);

                // refresh device gallery
                mediaScannerPlugin.Scan(pathToSave + formatExtension, mimeType);
                utilsPlugin.RefreshGallery(pathToSave + formatExtension, ImageType.JPG);

                Debug.Log("[AUP] ShareImage checking screenshots filenames...");
                string[] fileNames = utilsPlugin.GetFileNames(folderPath);
                foreach (string fileName in fileNames)
                {
                    Debug.Log("[AUP] ShareImage checking filename " + fileName);
                    Debug.Log("[AUP] ShareImage checking absolute path " + folderPath + "/" + fileName);
                    // using this you can load this images in unity3d
                    //loads in unity3d  texture
                    //RawImage rawImage = null;
                    //rawImage.texture = AUP.Utils.LoadTexture(folderPath + "/" + fileName );
                }
            }
        }

        public void onScanStarted()
        {
            dispatcher.InvokeAction(
                () => { Debug.Log("[ShareAndExperienceDemo] onScanStarted media "); }
            );
        }

        public void onScanComplete()
        {
            dispatcher.InvokeAction(
                () => { Debug.Log("[ShareAndExperienceDemo] onScanComplete media "); }
            );
        }

        public void onScanFail()
        {
            dispatcher.InvokeAction(
                () => { Debug.Log("[ShareAndExperienceDemo] onScanFail media "); }
            );
        }
    }
}