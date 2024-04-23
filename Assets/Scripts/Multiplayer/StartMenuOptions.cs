
using DilmerGames.Core.Singletons;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuOptions : Singleton<StartMenuOptions>
{
    [SerializeField]
    GameObject networkManagerPrefab;

    [SerializeField]
    TMPro.TextMeshProUGUI errorText;

    [SerializeField]
    GameObject buttonContainer;

    private void Start()
    {
        if (NetworkOptions.Instance == null)
        {
            Instantiate(networkManagerPrefab);
        }
    }


    public void CreatePublic()
    {
        NetworkOptions.Instance.CreatePublicRoom();
        StartLoadingRoutine();
    }
    public void CreatePrivate()
    {
        NetworkOptions.Instance.CreatePrivateRoom();
        StartLoadingRoutine();
    }
    public void JoinRandom()
    {
        NetworkOptions.Instance.FindRandomHost();
        StartLoadingRoutine();
    }

    public void StartSoloDemo()
    {
        SceneManager.LoadScene("SingleplayerScene", LoadSceneMode.Single);
        StartLoadingRoutine();
    }


    public void MoveMenu(Transform wallTransform)
    {
        transform.position = wallTransform.position;
        transform.rotation = Quaternion.LookRotation(-wallTransform.forward);
    }

    public void StartErrorRoutine(ErrorType errorType)
    {
        if(errorRoutine != null) { StopCoroutine(errorRoutine); errorRoutine = null; }
        if (loadingRoutine != null) { StopCoroutine(loadingRoutine); loadingRoutine = null; }

        StartCoroutine(errorRoutine = ErrorRoutine(errorType));
    }

    public enum ErrorType { Error, NoLobbies}

    IEnumerator errorRoutine;
    IEnumerator ErrorRoutine(ErrorType errorType)
    {
        string errorMessage = ""; 
        switch (errorType) 
        { 
            case ErrorType.NoLobbies: errorMessage = "No Open Lobbies Found"; break;
            case ErrorType.Error: errorMessage = "Error"; break;
        }
        errorText.text = errorMessage;
        errorText.gameObject.SetActive(true);
        buttonContainer.SetActive(false);
        yield return new WaitForSeconds(3);
        errorText.gameObject.SetActive(false);
        buttonContainer.SetActive(true);

        errorRoutine = null;
        yield return null;
    }

    public void StartLoadingRoutine()
    {
        if(loadingRoutine != null) { StopCoroutine(loadingRoutine); loadingRoutine = null; }

        if (errorRoutine != null) { StopCoroutine(errorRoutine); errorRoutine = null; }

        StartCoroutine(loadingRoutine = LoadingRoutine());
    }

    IEnumerator loadingRoutine;
    IEnumerator LoadingRoutine()
    {
        errorText.text = "Loading...";
        errorText.gameObject.SetActive(true);
        buttonContainer.SetActive(false);

        bool loading = true;
        while (loading)
        {
            yield return null;
        }

        errorText.gameObject.SetActive(false);
        buttonContainer.SetActive(true);

        errorRoutine = null;
        yield return null;
    }
}
