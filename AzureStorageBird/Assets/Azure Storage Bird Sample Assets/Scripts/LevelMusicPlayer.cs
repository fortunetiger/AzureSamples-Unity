using System;
using System.Collections;
using System.IO;
using UnityEngine;

/// <summary>
/// This class plays music downloaded at runtime from Azure Blob Storage.
/// New tracks are played as the player progresses through levels by subscribing to the
/// GameControl.StartedNewLevel event.
/// </summary>
public class LevelMusicPlayer : MonoBehaviour 
{
    public static event Action LoadingAudioClipsFinished;
    // These track names are added to the array in a specific order of progressing intensity
    // (based on BPM and my own judgement) to reflect increasing level challenge.
    // See https://assetstore.unity.com/packages/audio/music/metal-mayhem-music-pack-19233
    // for more info.
    public static readonly string[] MusicFileNamesInLevelOrder =
        { "Track 6.ogg", "Track 7.ogg", "Track 4.ogg", "Track 3.ogg",
          "Track 1.ogg", "Track 8.ogg", "Track 9.ogg", "Track 2.ogg",
          "Track 5.ogg", "Track 10.ogg" };
    private AudioSource audioSource;
    private AudioClip[] audioClips;

    // Use this for initialization
    private void Awake () 
	{
        DontDestroyOnLoad(this.gameObject);
        audioClips = new AudioClip[MusicFileNamesInLevelOrder.Length];
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// This coroutine preloads the audio clips to avoid a stutter from loading during gameplay.
    /// Raises the LoadingAudioClipsFinished event when complete.
    /// </summary>
    private IEnumerator PreloadClips()
    {
        for (int i = 0; i < MusicFileNamesInLevelOrder.Length; i++)
        {
            var path = Path.Combine(TitleSceneBehavior.DestinationPath, MusicFileNamesInLevelOrder[i]);
            WWW www = new WWW($"file://{path}");
            yield return www;

            if (string.IsNullOrEmpty(www.error) == false)
            {
                Debug.Log($"WWW error: {www.error}");
            }
            var audioClip = www.GetAudioClip(false, false);
            audioClips[i] = audioClip;
        }
        LoadingAudioClipsFinished?.Invoke();
    }

    /// <summary>
    ///  Plays a new audio clip in the audioClips array
    ///  based on the index of the new level.
    /// </summary>
    /// <param name="levelIndex">The new level's index.</param>
    private void PlayAudioClipForNewLevel(int levelIndex)
    {
        if (audioClips.Length > levelIndex)
        {
            audioSource.clip = audioClips[levelIndex];
            audioSource.Play();
        }
    }

    #region Event handlers
    /// <summary>
    /// GameControl.StartedNewLevel event handler.
    /// </summary>
    /// <param name="levelIndex">The event passes us the current levelIndex.</param>
    private void OnStartedNewLevel(int levelIndex)
    {
        PlayAudioClipForNewLevel(levelIndex);
    }

    /// <summary>
    /// GameControl.GameOver event handler.
    /// </summary>
    private void OnGameOver()
    {
        audioSource.Stop();
    }

    /// <summary>
    /// TitleSceneBehavior.DownloadingMusicFilesFinished event handler.
    /// We need to wait for the files to be done downloading, then begin preloading.
    /// </summary>
    private void OnDownloadingMusicFilesFinished()
    {
        StartCoroutine(PreloadClips());
    }
    #endregion

    #region Event subscription / unsubscription
    private void OnEnable()
    {
        GameControl.StartedNewLevel += OnStartedNewLevel;
        GameControl.GameOver += OnGameOver;
        TitleSceneBehavior.DownloadingMusicFilesFinished += OnDownloadingMusicFilesFinished;
    }

    private void OnDisable()
    {
        GameControl.StartedNewLevel -= OnStartedNewLevel;
        GameControl.GameOver -= OnGameOver;
        TitleSceneBehavior.DownloadingMusicFilesFinished += OnDownloadingMusicFilesFinished;
    }
    #endregion
}
