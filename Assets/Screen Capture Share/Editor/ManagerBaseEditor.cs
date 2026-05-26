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

using ScreenCaptureShare.ScreenCapture.Managers;
using ScreenCaptureSharePlugin.Core;
using TryItCompatibilityEditor;
using UnityEditor;

namespace ScreenCaptureShareEditor
{
    [CustomEditor(typeof(ManagerBase), true)]
    public class ManagerBaseEditor : Editor
    {
        static string[] s_ExcludedProperties = new[] { "m_Script" };

        public override void OnInspectorGUI()
        {
            ShowTryIt.DrawOpenTryItButton(CaptureShareSettings.Instance.GetTryIt());
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, s_ExcludedProperties);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
