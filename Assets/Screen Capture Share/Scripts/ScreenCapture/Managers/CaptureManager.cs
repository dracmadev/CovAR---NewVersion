#region Author
/*
     
     Jones St. Lewis Cropper (caLLow)
     
     Another caLLowCreation
     
     Visit us on Google+ and other social media outlets @caLLowCreation
     
     Thanks for using our product.
     
     Send questions/comments/concerns/requests to 
      e-mail: caLLowCreation@gmail.com
      subject: ScreenCaptureShare
     
*/
#endregion

using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using ScreenCaptureSharePlugin.Render;
using ScreenCaptureSharePlugin.CaptureIO;
using ScreenCaptureSharePlugin.Core;
using System.Collections.Generic;

namespace ScreenCaptureShare.ScreenCapture.Managers
{
    /// <summary>
    /// Manages the screen capture and image attributes and storage
    /// </summary>
    [AddComponentMenu("Scripts/Screen Capture Share/Screen Capture/Managers/Capture Manager")]
    public class CaptureManager : ManagerBase
    {
        [Serializable]
        class CaptureEventString : UnityEvent<string> { }

        [Serializable]
        class CaptureEventTexture : UnityEvent<Texture2D> { }
        
        /// <summary>
        /// Event fired when the capture process is complete
        /// </summary>
        public static event EventHandler<CaptureEventArgs> OnCaptureRecorded;

        static Func<Component, Camera>[] s_SearchForCameraMethods = new Func<Component, Camera>[]
        {
            (component) => component.GetComponent<Camera>(),
            (component) => component.GetComponentInParent<Camera>(),
            (component) => component.GetComponentInChildren<Camera>(),
            (component) => Camera.main,
            (component) => FindObjectOfType<Camera>(),
            (component) => { throw new UnityException("A camera is required to use this component"); }
        };

        [SerializeField, Tooltip("If left empty a camera in the scene will be used")]
        Camera m_CaptureCamera = null;

        [SerializeField]
        Config m_Config = new Config("temp_screenshot", 1920, 1080, CaptureFormat.PNG);

        [SerializeField]
        CaptureEventString m_CaptureEventString = null;
        [SerializeField]
        CaptureEventTexture m_CaptureEventTexture = null;

        string m_CaptureFilenameFull = string.Empty;

        void Awake()
        {
            for (int i = 0; i < s_SearchForCameraMethods.Length; i++)
            {
                if (m_CaptureCamera) break;
                m_CaptureCamera = s_SearchForCameraMethods[i].Invoke(this);
            }
        }
        
        /// <summary>
        /// Starts a coroutine to capture and store the screenshot
        /// </summary>
        public void CaptureScreenshot()
        {
            StartCoroutine(SaveScreenshot());
        }

        IEnumerator SaveScreenshot()
        {
            CaptureRenderer configRenderer = CaptureRenderer.RecordScreenshot(m_CaptureCamera, m_Config);

            byte[] fileHeader = null;
            byte[] fileData = null;
            switch (m_Config.format)
            {
                case CaptureFormat.RAW:
                    fileData = configRenderer.screenshot.GetRawTextureData();
                    break;
                case CaptureFormat.JPG:
                    fileData = configRenderer.screenshot.EncodeToJPG();
                    break;
                case CaptureFormat.PNG:
                    fileData = configRenderer.screenshot.EncodeToPNG();
                    break;
                case CaptureFormat.PPM:// create a file header for ppm formatted file
                    string headerStr = string.Format("P6\n{0} {1}\n255\n", configRenderer.screenshot.width, configRenderer.screenshot.height);
                    fileHeader = System.Text.Encoding.ASCII.GetBytes(headerStr);
                    fileData = configRenderer.screenshot.GetRawTextureData();
                    break;
                default:
                    break;
            }
            m_CaptureFilenameFull = ShareManager.GetFilenameFull(m_Config.baseFilename);

            yield return StartCoroutine(CaptureFile.SaveToFile(m_CaptureFilenameFull, fileHeader, fileData));

            configRenderer.ResetValues();

            Texture2D texture2D = CaptureFile.LoadFromFile(m_CaptureFilenameFull);

            if (OnCaptureRecorded != null)
            {
                OnCaptureRecorded.Invoke(this, new CaptureEventArgs(m_CaptureFilenameFull, texture2D));
            }

            m_CaptureEventString.Invoke(m_CaptureFilenameFull);
            m_CaptureEventTexture.Invoke(texture2D);
        }
    }

}
