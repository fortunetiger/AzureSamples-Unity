using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Heatmap : MonoBehaviour
{
    [SerializeField]
    private GameObject markerPrefab;

    [SerializeField]
    int numberOfAttempts = 3;

    List<CrashInfo> crashesFromServer;

    private void Start()
    {
        InitializeCrashList();
    }


    private void InitializeCrashList()
    {
        Debug.Log("Downloading crash data from Azure...");
        EasyTablesClient.Instance.GetAllEntries<CrashInfo>
        (
            serverResponse =>
            {
                if (serverResponse.Status == CallBackResult.Success)
                {
                    Debug.Log("All crashes downloaded.");
                    crashesFromServer = serverResponse.Result;
                    SpawnMarkersFromList();
                }
                else
                {
                    Debug.Log(serverResponse.Exception.Message);
                }
            }
        );       
    }
    private void SpawnMarkersFromList()
    {
        foreach (var item in crashesFromServer)
        {
            GameObject marker = GameObject.Instantiate(markerPrefab);
            marker.transform.position = new Vector3 { x = item.x, y = item.y, z = item.z };
        }
    }


    // TODO: implement delete...
    //public async void DeleteCrashDataAsync()
    //{
    //    Debug.Log("Deleting crash data...");
    //    foreach (var item in crashesFromServer)
    //    {
    //        try
    //        {
    //            await crashesTable.DeleteAsync(item);
    //        }
    //        catch (System.Exception e)
    //        {
    //            Debug.Log("Error deleting crash data: " + e.Message);
    //        }
    //        Debug.Log("Done deleting crash data.");
    //    }
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}

}
