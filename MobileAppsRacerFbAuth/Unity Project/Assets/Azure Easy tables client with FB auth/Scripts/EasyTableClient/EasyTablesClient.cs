using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EasyTablesClient : MonoBehaviour 
{
    // Update with your Azure Function App URL. Do not include a "/" at the end!
    private const string url = "INSERT_YOUR_AZURE_FUNCTION_APP_URL_HERE";
    // Update with your Function App Host key.
    // Don't use the _master key!
    private const string hostKey = "INSERT_YOUR_FUNCTION_APP_HOST_KEY_HERE";

    private static EasyTablesClient instance;

    public static EasyTablesClient Instance
    {
        get
        {
            if (instance == null)
            {
                var newGameObject = new GameObject(typeof(EasyTablesClient).ToString());
                instance = newGameObject.AddComponent<EasyTablesClient>();
                DontDestroyOnLoad(newGameObject);
            }

            return instance;
        }
    }

    public void Insert<T>(T instance, Action<CallbackResponse<T>> onInsertCompleted)
        where T : EasyTablesObjectBase
    {
        StartCoroutine(InsertCoroutine<T>(instance, onInsertCompleted));
    }

    public void GetAllEntries<T>(Action<CallbackResponse<List<T>>> onGetAllEntriesCompleted)
        where T : EasyTablesObjectBase
    {
        StartCoroutine(GetAllEntriesCoroutine(onGetAllEntriesCompleted));
    }

    private IEnumerator GetAllEntriesCoroutine<T>(Action<CallbackResponse<List<T>>> onGetAllEntriesCompleted)
        where T : EasyTablesObjectBase
    {
        string functionUrl = url + "/api/GetAllEntries?code=" + hostKey;

        // Server expects a json arrary with the format:
        // [{"access_token":"value"},{"tableName":"value"}]
        string jsonArray = string.Format("[{0}, {1}]", GetAccessTokenJson(), GetTableNameJson<T>());

        using (UnityWebRequest www = WebRequestUtilities.BuildWebRequest(functionUrl, UnityWebRequest.kHttpVerbPOST, jsonArray))
        {
            yield return www.SendWebRequest();

            var response = new CallbackResponse<List<T>>();

            if (WebRequestUtilities.IsWWWError(www))
            {
                Debug.Log("Error: " + www.error + " Response code: " + www.responseCode);
                WebRequestUtilities.BuildResponseObjectOnFailure(response, www);
            }
            else if (www.downloadHandler != null) // all OK.
            {
                //let's get the new object that was created
                try
                {
                    var array = JsonHelper.GetJsonArray<T>(www.downloadHandler.text);
                    var listToReturn = new List<T>(array);
                    response.Result = listToReturn;
                }
                catch (Exception ex)
                {
                    Debug.Log("Exception!: " + ex.ToString());
                    response.Status = CallBackResult.DeserializationFailure;
                    response.Exception = ex;
                }
            }
            onGetAllEntriesCompleted(response);
        }
    }

    private IEnumerator InsertCoroutine<T>(T instance, Action<CallbackResponse<T>> onInsertCompleted)
        where T : EasyTablesObjectBase
    {
        string functionUrl = url + "/api/Insert?code=" + hostKey;

        // Server expects a json arrary with the format:
        // [{"access_token":"value"},{"tableName":"value"},{instanceJson}]
        string instanceJson = JsonUtility.ToJson(instance);
        string jsonArray = string.Format("[{0}, {1}, {2}]", GetAccessTokenJson(), GetTableNameJson<T>(), instanceJson);

        using (UnityWebRequest www = WebRequestUtilities.BuildWebRequest(functionUrl, UnityWebRequest.kHttpVerbPOST, jsonArray))
        {
            yield return www.SendWebRequest();

            var response = new CallbackResponse<T>();

            if (WebRequestUtilities.IsWWWError(www))
            {
                Debug.Log("Error: " + www.error);
                WebRequestUtilities.BuildResponseObjectOnFailure(response, www);
            }
            else if (www.downloadHandler != null) // all OK.
            {
                //let's get the new object that was created
                try
                {
                    Debug.Log(www.downloadHandler.text);
                    T newObject = JsonUtility.FromJson<T>(www.downloadHandler.text);
                    Debug.Log("Got this back from the server: " + newObject.ToString());
                    response.Status = CallBackResult.Success;
                    response.Result = newObject;
                }
                catch (Exception ex)
                {
                    Debug.Log("Exception!: " + ex.ToString());
                    response.Status = CallBackResult.DeserializationFailure;
                    response.Exception = ex;
                }
            }
            onInsertCompleted(response);
        }
    }

    private string GetTableNameJson<T>()
    {
        return "{\"tableName\": \"" + typeof(T).ToString() + "\"}";
    }

    private string GetAccessTokenJson()
    {
        return "{\"access_token\": \"" + FacebookLogin.Instance.AccessToken + "\"}";
    }
}