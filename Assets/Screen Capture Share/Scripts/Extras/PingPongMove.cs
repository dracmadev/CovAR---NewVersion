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
    /// Ping Pong Move moves a transform for point a to point b and back again in a repeating fassion
    /// </summary>
    [AddComponentMenu("Scripts/Screen Capture Share/Extras/Ping Pong Move")]
    public class PingPongMove : MonoBehaviour
    {
        [SerializeField]
        float m_Length = 3.0f;

        float m_StartXP = 0.0f;

        void Update()
        {
            transform.position = new Vector3(m_StartXP + Mathf.PingPong(Time.time * 2, m_Length) - m_Length / 2, transform.position.y, transform.position.z);
        }
    }
}
