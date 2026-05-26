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

using ScreenCaptureSharePlugin.Wrappers;
using UnityEngine;

namespace ScreenCaptureShare.ScreenCapture.Managers
{
    /// <summary>
    /// Manages the Native Android share wrapper intent
    /// </summary>
    [AddComponentMenu("Scripts/Screen Capture Share/Screen Capture/Managers/Share Manager")]
    public class ShareManager : ManagerBase
    {
        /// <summary>
        /// Calls the Android share intent to open the share options
        /// </summary>
        /// <param name="message"></param>
        public void ShareScreenshot(string message)
        {
            AndroidShareWrapper.CallShareIntent(message);
        }

        /// <summary>
        /// Gets the full filename for the base image file including the path and extension
        /// </summary>
        /// <param name="baseFilename">The base filename without path nor extension</param>
        /// <returns>Full filename and path</returns>
        public static string GetFilenameFull(string baseFilename)
        {
            return AndroidShareWrapper.GetFilenameFull(baseFilename);
        }
    }
}
