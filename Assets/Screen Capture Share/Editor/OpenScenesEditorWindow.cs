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

using ScreenCaptureSharePlugin.Core;
using TryItCompatibilityEditor;
using UnityEditor;

namespace ScreenCaptureShareEditor
{
    /// <summary>
    /// Opens example scene picker
    /// </summary>
    [InitializeOnLoad]
    internal sealed class OpenScenesEditorWindow
    {
        static OpenScenesEditorWindow()
        {
            //Debug.Log("OpenScenesEditorWindow UpdateShowPrompt");
            ExampleScenesWindow.newsletterSignupUrl = "http://u3d.as/10Lj";
            ExampleScenesWindow.isFullVersion = !CaptureShareSettings.Instance.GetTryIt();
            ExampleScenesWindow.packageName = "Native Screen Share";
            ExampleScenesWindow.buttonTexts = new SceneButton[]
            {
                new SceneButton("Capture Scene Example", "Capture Scene", "Assets/Screen Capture Share/Scenes/Capture Scene Example.unity",
                    "This scene, Live Test Scene, is a quick show of how the main component is used with an example of color changes, that will show on the screenshot.  Press play to save and share the scene view."),
            };

            ExampleScenesWindow.fullVersionUrl = "	http://u3d.as/Pto";
        }

        [MenuItem("Window/Screen Capture Share/Example Scenes")]
        internal static void Init()
        {
            ExampleScenesWindow.Init(height: 280, width: 280);
        }
    }
}
