/*
using UnityEngine;
using Firebase;
using Firebase.Messaging;
using System.Threading.Tasks;

// Aquesta línia només s'activa quan compiles per a Android
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class FirebaseManager : MonoBehaviour
{
    void Start()
    {
        // 1. Comprovar dependències (necessari per a ambdós sistemes)
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("No s'han pogut resoldre les dependències: " + dependencyStatus);
            }
        });
    }

    void InitializeFirebase()
    {
        // 2. Configurem els esdeveniments de recepció
        FirebaseMessaging.MessageReceived += OnMessageReceived;
        FirebaseMessaging.TokenReceived += OnTokenReceived;

        // 3. Obtenir el Token (per fer proves individuals)
        FirebaseMessaging.GetTokenAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Firebase Token: " + task.Result);
            }
        });

        // 4. Llançar la petició de permís segons el sistema
        Invoke("RequestNotificationPermission", 1.5f);

        Debug.Log("Firebase Messaging inicialitzat correctament.");
    }

    void RequestNotificationPermission()
    {
#if UNITY_ANDROID
        // Codi específic per a Android 14 (API 34)
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Debug.Log("Demanant permís nativament a Android...");
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
#elif UNITY_IOS
        // Codi per a iPhone (Xcode s'encarregarà de la resta)
        Debug.Log("Demanant permís a iOS...");
        FirebaseMessaging.RequestPermissionAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Petició de permís a iOS finalitzada.");
            }
        });
#endif
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Notificació rebuda!");
        if (e.Message.Notification != null)
        {
            Debug.Log("Títol: " + e.Message.Notification.Title);
            Debug.Log("Cos: " + e.Message.Notification.Body);
        }
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Nou Token rebut: " + token.Token);
    }
}*/