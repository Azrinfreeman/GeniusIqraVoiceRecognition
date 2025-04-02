using System;
using UnityEngine;

namespace Gigadrillgames.AUP.Tools
{
    public class FilePickerCallback : AndroidJavaProxy
    {
        public Action<string,string> onHandleGetFilePath;
        public FilePickerCallback() : base("com.gigadrillgames.androidplugin.filepicker.IFilePicker")
        {
        }

        void HandleGetFilePath(string message, string filepath)
        {
            onHandleGetFilePath(message, filepath);
        }
    }
}