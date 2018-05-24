using System;
using UnityEngine;
using UnityEngine.UI;

public class TestScene : MonoBehaviour 
{
    [SerializeField]
    private Button insertButton, getAllEntriesButton;

    [SerializeField]
    private Text outputText;

    private void Start()
    {
        // Will block buttons until logged in.
        insertButton.interactable = false;
        getAllEntriesButton.interactable = false;
    }

    #region Functions called from Unity UI buttons' OnClick
    public void PressedLoginButton()
    {
        outputText.text += "\n Logging in...";
        FacebookLogin.Instance.LogInUser();
    }
    public void ClickedInsertButton()
    {
        Debug.Log("Insert started...");
        outputText.text += "\n" + "Insert started...";
        EasyTablesClient.Instance.Insert<TestPlayerData>(
            new TestPlayerData { name = "George", highScore = 999 },
            serverResponse =>
            {
                if (serverResponse.Status == CallBackResult.Success)
                {
                    string result = "Insert completed";
                    Debug.Log(result);
                    outputText.text += "\n" + result + "\n" + serverResponse.Result.id + " inserted.";
                }
                else
                {
                    Debug.Log(serverResponse.Exception.Message);
                    outputText.text += "\n" + serverResponse.Exception.Message;
                }
            }
        );
    }
    public void ClickedGetAllEntriesButton()
    {
        Debug.Log("GetAllEntries Started...");
        outputText.text += "\n" + "GetAllEntries Started...";
        EasyTablesClient.Instance.GetAllEntries<TestPlayerData>
           (
               serverResponse =>
               {
                   if (serverResponse.Status == CallBackResult.Success)
                   {
                       string result = serverResponse.Result.ToString();
                       Debug.Log(result);
                       outputText.text += "\n" + "Get All Entries succeeded." + "\n" + result + 
                       " Count: " + serverResponse.Result.Count;
                   }
                   else
                   {
                       Debug.Log(serverResponse.Exception.Message);
                       outputText.text += "\n" + serverResponse.Exception.Message;
                   }
               }
           );
    }
    #endregion

    #region Event handler / subscribe and unsubscribe for FacebookLogin.LoggedIn
    private void OnFacebookLoggedIn()
    {
        insertButton.interactable = true;
        getAllEntriesButton.interactable = true;
        outputText.text += "\n Log in completed. Access token: \n" + FacebookLogin.Instance.AccessToken;
    }
    private void OnEnable()
    {
        FacebookLogin.LoggedIn += OnFacebookLoggedIn;
    }
    private void OnDisable()
    {
        FacebookLogin.LoggedIn -= OnFacebookLoggedIn;
    }
    #endregion
}

// Test data model class. Requires matching easy table on Azure.
[Serializable]
public class TestPlayerData : EasyTablesObjectBase
{
    public string name;
    public int highScore;
}