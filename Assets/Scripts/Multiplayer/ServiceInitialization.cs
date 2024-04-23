using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.Android;

public class ServiceInitialization : MonoBehaviour
{
    private void Awake()
    {
        Permission.RequestUserPermission("com.oculus.permissionPassthrough");
        Permission.RequestUserPermission("com.oculus.permission.USE_SCENE");
        Permission.RequestUserPermission("com.oculus.permission.BODY_TRACKING");
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        SetDisplayName();
    }

    void SetDisplayName()
    {
        NetworkSettings.Instance.data.displayName = "Human" + (UnityEngine.Random.Range(0, 9999)).ToString();
    }
}
