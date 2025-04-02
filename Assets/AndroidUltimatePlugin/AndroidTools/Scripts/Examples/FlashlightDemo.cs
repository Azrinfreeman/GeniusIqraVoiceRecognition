using UnityEngine;
using UnityEngine.UI;

namespace Gigadrillgames.AUP.Tools
{
    public class FlashlightDemo : MonoBehaviour
    {
        private FlashlightPlugin flashLightPlugin;

        private bool isFlashlightOn = false;

        //public DemoController demoController;
        public Text flashlightButtonText;

        // Use this for initialization
        void Start()
        {
            flashLightPlugin = FlashlightPlugin.GetInstance();
            flashLightPlugin.SetDebug(0);
            flashLightPlugin.Init();

            SetText();
        }

        private void OnDisable()
        {
            ReleaseFlashlight();
        }

        private void SetText()
        {
            if (!isFlashlightOn)
            {
                flashlightButtonText.text = "ON";
            }
            else
            {
                flashlightButtonText.text = "OFF";
            }
        }

        public void flashlightToggle()
        {
            if (!isFlashlightOn)
            {
                flashLightPlugin.SetFlashlightOn();
                isFlashlightOn = true;
            }
            else
            {
                flashLightPlugin.SetFlashlightOff();
                isFlashlightOn = false;
            }

            SetText();
        }

        private void ReleaseFlashlight()
        {
            flashLightPlugin.SetFlashlightOff();
            isFlashlightOn = false;
            flashLightPlugin.ReleaseFlashlight();

            SetText();

            Debug.Log("[FlashlightDemo] release flashlight");
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                ReleaseFlashlight();
            }
        }
    }
}