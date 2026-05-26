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

namespace ScreenCaptureShare.ScreenCapture.Managers
{
    /// <summary>
    /// Manages the testing display items, text for filename and image texture 6.35
    /// </summary>
    [AddComponentMenu("Scripts/Screen Capture Share/Screen Capture/Managers/Test Manager")]
    public class TestManager : ManagerBase
    {
        [SerializeField]
        bool m_ShowDisplay = true;

        [SerializeField]
        GameObject m_DisplayPanelGO = null;

        void Awake()
        {
            m_DisplayPanelGO.SetActive(m_ShowDisplay);
        }        
    }
}
