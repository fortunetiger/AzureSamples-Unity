using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections;

public class Leaderboard : MonoBehaviour 
{
    [SerializeField]
    private GameObject rowPrefab;

    [SerializeField]
    private Text loadingText;

    public const int SizeOfHighScoreList = 10;

    private List<HighScoreInfo> highScoreList;

    private void Start()
    {
        StartCoroutine(DownloadAndDisplayHighScores());
    }

    private IEnumerator DownloadAndDisplayHighScores()
    {
        bool finishedDownloading = false;

        EasyTablesClient.Instance.GetAllEntries<HighScoreInfo>
            (
                response =>
                {
                    if (response.Status == CallBackResult.Success)
                    {
                        Debug.Log("Downloaded high scores.");
                        highScoreList = response.Result
                                    .OrderBy(item => item.Time)
                                    .Take(SizeOfHighScoreList).ToList();
                    }
                    else
                    {
                        Debug.Log("Error downloading high scores."
                            + response.Exception.Message);
                    }
                    finishedDownloading = true;
                }
            );

        while (!finishedDownloading)
        {
            yield return null;
        }
        loadingText.gameObject.SetActive(false);
        GenerateHighScoreRowObjects();
    }

    private void GenerateHighScoreRowObjects()
    {
        if (highScoreList == null)
        {
            ShowEmptyLeaderboardMessage();
        }
        else if (highScoreList.Count == 0)
            ShowEmptyLeaderboardMessage();
        else
        {
            foreach (var item in highScoreList)
            {
                var row = Instantiate(rowPrefab, this.transform).GetComponent<LeaderboardRow>();
                row.HighScoreInfo = item;
            }
        }
    }

    private void ShowEmptyLeaderboardMessage()
    {
        loadingText.text = "The leaderboard is empty!";
    }


    // TODO: support deleting
    //public static async Task DeleteAllEntriesAsync()
    //{
    //   Debug.Log("Deleting leaderboard data...");

    //    var fullHighScoreList = await DownloadHighScoresAsync(false);

    //    foreach (var item in fullHighScoreList)
    //    {
    //        try
    //        {
    //            await HighScoreTable.DeleteAsync(item);
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.Log("Error deleting leaderboard data: " + e.Message);
    //        }
    //    }
    //}

    //private static async Task<List<HighScoreInfo>> DownloadHighScoresAsync(bool onlyTopEntries)
    //{
    //    List<HighScoreInfo> highScoreList;

    //    Debug.Log("Downloading high score data from Azure...");

    //    for (int i = 0; i < numberOfAttemptsToLoadData; i++)
    //    {
    //        try
    //        {
    //            Debug.Log("Connecting... attempt " + (i + 1));

    //            if (onlyTopEntries)
    //            {
    //                highScoreList = await HighScoreTable
    //                    .OrderBy(item => item.Time)
    //                    .Take(SizeOfHighScoreList)
    //                    .ToListAsync();
    //            }
    //            else
    //            {
    //                highScoreList = await HighScoreTable.ToListAsync();
    //            }

    //            Debug.Log("Done downloading high score data.");
    //            return highScoreList;
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.Log("Error connecting: " + e.Message);
    //        }

    //        if (i == numberOfAttemptsToLoadData - 1)
    //            Debug.Log("Connection failed. Check logs, try again later.");
    //        else
    //            await Task.Delay(500);
    //    }

    //    // If we can't successfully download a list from the server,
    //    // just make a new one to fail more gracefully.
    //    return highScoreList = new List<HighScoreInfo>();
    //}

    //private async void Start()
    //{
    //    var HighScoreList = await GetTopHighScoresAsync();

    //    if (HighScoreList.Count == 0)
    //    {
    //        ShowEmptyLeaderboardMessage();
    //    }
    //    else
    //    {
    //        loadingText.gameObject.SetActive(false);

    //        GenerateHighScoreRowObjects();
    //    }
    //}
}
