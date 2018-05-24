using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Collections;

public class RecordHighScore : MonoBehaviour 
{
    [SerializeField]
    private InputField nameInputField;

    [SerializeField]
    private CanvasGroup enterNamePopup;

    private List<HighScoreInfo> highScores;
    private string playerName = string.Empty;

    private void Start()
    {
        ShowEnterNamePopup(false);
        DownloadHighScoreList();
    }

    private void DownloadHighScoreList()
    {
        EasyTablesClient.Instance.GetAllEntries<HighScoreInfo>
            (
                serverResponse =>
                {
                    if (serverResponse.Status == CallBackResult.Success)
                    {
                        Debug.Log("Downloaded high scores.");
                        highScores = serverResponse.Result
                                    .OrderBy(item => item.Time)
                                    .Take(Leaderboard.SizeOfHighScoreList).ToList();
                    }
                    else
                    {
                        Debug.Log("Error downloading high scores." 
                            + serverResponse.Exception.Message);
                    }
                }
            );
    }

    private void ShowEnterNamePopup(bool shouldShow)
    {
        enterNamePopup.alpha = shouldShow ? 1 : 0;
        enterNamePopup.interactable = shouldShow;
    }

    public void SubmitButtonClicked()
    {
        playerName = nameInputField.text;
    }

    private void OnAfterMostRecentScoreSet(float newScore)
    {
        StartCoroutine(EnterHighScoreFlow(newScore));
    }

    private IEnumerator EnterHighScoreFlow(float newScore)
    {
        // If the high score list is null,
        // from a bad connection etc.,
        // we just want to skip all this.
        if (highScores != null)
        {
            bool isNewHighScore = CheckForNewHighScore(newScore);

            if (isNewHighScore)
            {
                Debug.Log("New High Score!");
                yield return StartCoroutine(GetPlayerName());
                yield return StartCoroutine(UploadNewHighScore(newScore));
            }
            else
            {
                Debug.Log("No new high score.");
            }
        }
    }

    private IEnumerator GetPlayerName()
    {
        // Wait a bit before showing the popup.
        // This just helps the player experience feel
        // less jarring.
        yield return new WaitForSeconds(2);

        ShowEnterNamePopup(true);

        // Wait until the player enters a name and clicks submit.
        // OnSubmitButtonClicked will set the playerName.
        while (playerName == string.Empty)
        {
            yield return new WaitForEndOfFrame();
        }

        ShowEnterNamePopup(false);
    }

    private bool CheckForNewHighScore(float newScore)
    {
        Debug.Log("Checking for a new high score...");

        bool isHighScoreListFull = highScores.Count >= Leaderboard.SizeOfHighScoreList;
        var lowerScores = highScores.Where(x => x.Time > newScore);

        return lowerScores.Count() > 0 || !isHighScoreListFull;
    }

    //private async Task UploadNewHighScoreAsync(float newScore)
    //{
    //    var newHighScoreInfo = new HighScoreInfo { Name = playerName, Time = newScore };

    //    try
    //    {
    //        Debug.Log("Uploading high score data to Azure...");

    //        await Leaderboard.HighScoreTable.InsertAsync(newHighScoreInfo);

    //        Debug.Log("Finished uploading high score data.");
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.Log("Error uploading high score data: " + e.Message);
    //    }
    //}

    private IEnumerator UploadNewHighScore(float newScore)
    {
        bool completed = false;
        var newHighScoreInfo = new HighScoreInfo { Name = playerName, Time = newScore };
        EasyTablesClient.Instance.Insert<HighScoreInfo>(newHighScoreInfo,
                    serverResponse =>
                    {
                        if (serverResponse.Status == CallBackResult.Success)
                        {
                            string result = "High score insert completed";
                            Debug.Log(result);
                        }
                        else
                        {
                            Debug.Log("High score insert failed... " 
                                + serverResponse.Exception.Message);
                        }
                        completed = true;
                    }
                );
        while (!completed)
        {
            yield return null;
        }
    }

    private void OnEnable()
    {
        LapTimer.AfterMostRecentScoreSet += OnAfterMostRecentScoreSet;
    }

    private void OnDisable()
    {
        LapTimer.AfterMostRecentScoreSet -= OnAfterMostRecentScoreSet;
    }

}
