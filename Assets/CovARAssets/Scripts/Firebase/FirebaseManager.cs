using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Messaging;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

public class FirebaseManager : MonoBehaviour
{
    private FirebaseApp app;
    private bool messagingInitialized;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (this == null)
                    return;

                if (task.IsCanceled)
                {
                    Debug.LogWarning(
                        "Comprovació de dependències Firebase cancel·lada.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogException(task.Exception);
                    return;
                }

                if (task.Result != DependencyStatus.Available)
                {
                    Debug.LogError(
                        "No s'han pogut resoldre les dependències: "
                        + task.Result);
                    return;
                }

                app = FirebaseApp.DefaultInstance;
                InitializeFirebase();
            });
    }

    private void InitializeFirebase()
    {
        if (messagingInitialized)
            return;

        FirebaseMessaging.MessageReceived += OnMessageReceived;
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        messagingInitialized = true;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        FirebaseMessaging.GetTokenAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (this == null)
                    return;

                if (task.IsCanceled)
                {
                    Debug.LogWarning(
                        "Obtenció del token Firebase cancel·lada.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogException(task.Exception);
                    return;
                }

                Debug.Log("Firebase Token: " + task.Result);
            });

        Invoke(nameof(RequestNotificationPermission), 1.5f);
#endif

        Debug.Log("Firebase inicialitzat correctament.");
    }

    private void RequestNotificationPermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // El permís de notificacions existeix des d'Android 13 (API 33).
        using (var version =
               new AndroidJavaClass("android.os.Build$VERSION"))
        {
            int apiLevel = version.GetStatic<int>("SDK_INT");

            const string notificationPermission =
                "android.permission.POST_NOTIFICATIONS";

            if (apiLevel >= 33 &&
                !Permission.HasUserAuthorizedPermission(
                    notificationPermission))
            {
                Debug.Log("Demanant permís de notificacions a Android...");

                Permission.RequestUserPermission(
                    notificationPermission);
            }
        }

#elif UNITY_IOS && !UNITY_EDITOR
        Debug.Log("Demanant permís de notificacions a iOS...");

        FirebaseMessaging.RequestPermissionAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (this == null)
                    return;

                if (task.IsCanceled)
                {
                    Debug.LogWarning(
                        "Petició de permís a iOS cancel·lada.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogException(task.Exception);
                    return;
                }

                // Completar la petició no confirma que s'hagi acceptat.
                Debug.Log("Petició de permís a iOS finalitzada.");
            });
#endif
    }

    private void OnMessageReceived(
        object sender,
        MessageReceivedEventArgs e)
    {
        Debug.Log("Missatge Firebase rebut!");

        if (e.Message.Notification != null)
        {
            Debug.Log("Títol: " + e.Message.Notification.Title);
            Debug.Log("Cos: " + e.Message.Notification.Body);
        }
    }

    private void OnTokenReceived(
        object sender,
        TokenReceivedEventArgs token)
    {
        Debug.Log("Nou token Firebase rebut: " + token.Token);
    }

    private void OnDestroy()
    {
        CancelInvoke();

        if (messagingInitialized)
        {
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
        }
    }
}