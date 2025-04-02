using System;
using Gigadrillgames.AUP.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Gigadrillgames.AUP.Tools
{
    public class ShareTextureFromWebDemo : MonoBehaviour
    {
        private SharePlugin sharePlugin;
        private Texture2D existingTexture;
        [SerializeField] private string webUrl = "https://gigadrillgames.com/images/AUP-256.jpg";
        private string texturePath = "";
        public RawImage rawImage;
        private bool isLoading = false;
        private bool isLoadComplete = false;

        public Text statusText;
        public Button loadButton;
        public Button shareButton;


        // Use this for initialization
        void Start()
        {
            sharePlugin = SharePlugin.GetInstance();
            sharePlugin.SetDebug(0);

            EnableDisableLoadButton(true);
            EnableDisableShareButton(false);
            UpdateStatusText("waiting.");
        }

        private void UpdateStatusText(string status)
        {
            statusText.text = String.Format("Status: {0}", status);
        }

        private void EnableDisableLoadButton(bool val)
        {
            loadButton.interactable = val;
        }

        private void EnableDisableShareButton(bool val)
        {
            shareButton.interactable = val;
        }

        public void LoadImageFromWeb()
        {
            if (!isLoading)
            {
                UpdateStatusText("Downloading Image...");
                EnableDisableLoadButton(false);
                EnableDisableShareButton(false);
                isLoading = true;
                isLoadComplete = false;
                Debug.Log("LoadImageFromWeb " + webUrl);
                StartCoroutine(Utils.LoadTextureFromWeb(webUrl, OnLoadImageComplete, OnLoadImageFail));
            }
        }

        private void OnLoadImageComplete(Texture2D texture)
        {
            rawImage.texture = texture;
            isLoading = false;
            isLoadComplete = true;
            EnableDisableLoadButton(true);
            EnableDisableShareButton(true);
            UpdateStatusText("Load Complete");
            Debug.Log("Load Image From Web compete texture " + texture);
        }

        private void OnLoadImageFail()
        {
            isLoading = false;
            isLoadComplete = false;
            EnableDisableLoadButton(true);
            EnableDisableShareButton(false);
            UpdateStatusText("Load Failed.");
            Debug.Log("Load Image From Web  fail! ");
        }


        public void ShareLoadedTexture()
        {
            if (isLoadComplete)
            {
                UpdateStatusText("Sharing...");
                SaveLoadedTextureOnDevice();
                ShareImage();
            }
        }

        private void SaveLoadedTextureOnDevice()
        {
            string textureName = "sampleTexture.png";
            texturePath = Application.persistentDataPath + "/" + textureName;

            existingTexture = rawImage.texture as Texture2D;
            StartCoroutine(Utils.SaveTexureOnDevice(texturePath, existingTexture));
        }

        private void ShareImage()
        {
            if (!texturePath.Equals("", StringComparison.Ordinal))
            {
                sharePlugin.ShareImage("ExistingTexture", "ExistingTextureContent", texturePath);
            }
            else
            {
                Debug.Log("[CameraDemo] texturePath is empty");
            }
        }
    }
}