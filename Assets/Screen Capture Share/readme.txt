Native Screen Share Version 1.0.0 11/15/2017


GENERAL USAGE NOTES
==============================================
* Get a screenshot of the camera view
* All exposed native share functionality


INSTALLATION
==============================================
* Download the package from Unity Asset Store
* Import all the assets
* Add a "Basic Capture Share Canvas" Prefab to the Hierarchy
* Add a camera to capture the scene (If not an available camera will be used)
* Press play


EXAMPLE
==============================================
This example show methods to capture a screenshot
 /// <summary>
/// Capture a screenshot with one method call
/// </summary>
void OnDestroy()
{
            FindObjectOfType<CaptureManager>().CaptureScreenshot();
}
--------------------------------------------
This example show methods to share a screenshot
/// <summary>
 /// Share to any device app with one method call
/// </summary>
void OnDestroy()
{
	FindObjectOfType<ShareManager>().ShareScreenshot("My Cool Screenshot, look firends");
}


CONTACT
==============================================
Developer: caLLowCreation
Author: Jones S. Cropper
E-mail: callowcreation@gmail.com 
Subject: Screen Capture Share
Website: www.caLLowCreation.com