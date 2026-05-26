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

using System;
using UnityEngine;

namespace ScreenCaptureShare.Extras
{
    /// <summary>
    /// Uses to go back on mobile devices and quit in Unity Editor
    /// </summary>
    public class GoBack : MonoBehaviour
    {
        public static event Action OnGoBackPressed;

        void Update()
        {
            BackButton();
        }

        void BackButton()
        {
            if (PlatformChecks())
            {
                if (InputBackPressed())
                {
                    if (OnGoBackPressed != null)
                    {
                        OnGoBackPressed.Invoke();
                    }
                    QuitOrGoBack();
                }
            }
        }

        bool PlatformChecks()
        {
            return Application.platform == RuntimePlatform.Android
                || Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.WindowsPlayer;
        }

        bool InputBackPressed()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        void QuitOrGoBack()
        {
            QuitApplication();
        }

        /// <summary>
        /// QUit the application running on device or in Unity Editor
        /// </summary>
        public static void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
