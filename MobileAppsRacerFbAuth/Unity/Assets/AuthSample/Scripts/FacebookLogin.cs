using UnityEngine;
using Facebook.Unity;
using System;
using System.Collections.Generic;

public class FacebookLogin : MonoBehaviour 
{
    public static event Action LoggedIn;
    private string accessToken;
    private static FacebookLogin instance;

    #region Properties
    public static FacebookLogin Instance
    {
        get
        {
            if (instance == null)
            {
                var newGameObject = new GameObject(typeof(FacebookLogin).ToString());
                instance = newGameObject.AddComponent<FacebookLogin>();
                DontDestroyOnLoad(newGameObject);
            }

            return instance;
        }
    }
    public string AccessToken
    {
        get
        {
            return accessToken;
        }
        private set
        {
            accessToken = value;
        }
    }
    #endregion

    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    /// <summary>
    /// Call this to log into Facebook via the Facebook SDK and get an access token.
    /// </summary>
    public void LogInUser()
    {
        // If in the editor, you will need to test with a debug access token.
        // This can be found here: https://developers.facebook.com/tools/accesstoken/
        // The Facebook SDK only supports Android, iOS, and WebGL platforms. 
        // Alternatively you can make a Facebook build to support PC via FB Gameroom:
        // https://unity3d.com/partners/facebook/gameroom
#if (UNITY_EDITOR)
        Debug.Log("Using debug access token. It does expire!");
        accessToken = "INSERT_YOUR_DEBUG_ACCESS_TOKEN_HERE";
        if (LoggedIn != null) LoggedIn.Invoke();
#else
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
#endif
    }

    #region Facebook SDK callbacks
    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            if (LoggedIn != null)
            {
                accessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
                LoggedIn.Invoke();
            }
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }
    #endregion
}