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

namespace ScreenCaptureShare.Extras
{
    /// <summary>
    /// Ping Pong Color moves a color value from one color to the next and back again in a repeating fassion
    /// </summary>
    [AddComponentMenu("Scripts/Screen Capture Share/Extras/Ping Pong Color")]
    public class PingPongColor : MonoBehaviour
    {
        [SerializeField]
        Color m_ColorStart = Color.red;
        [SerializeField]
        Color m_ColorEnd = Color.green;
        [SerializeField]
        float m_Duration = 1.0f;

        Renderer m_Renderer = null;
        void Start()
        {
            m_Renderer = GetComponent<Renderer>();
        }

        void Update()
        {
            float lerp = Mathf.PingPong(Time.time, m_Duration) / m_Duration;
            m_Renderer.material.color = Color.Lerp(m_ColorStart, m_ColorEnd, lerp);
        }
    }
}
