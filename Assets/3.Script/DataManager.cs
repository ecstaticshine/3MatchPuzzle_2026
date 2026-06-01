using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public List<StageData> stageDataList = null;

    public int selectedStage = 1;
    public int clearedStage = 0;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Load();
            StartCoroutine(LoadStage());
        }
        else
        {
            Destroy(this);  
        }
    }

    public void Save()
    {
        PlayerPrefs.SetInt("ClearedStage", clearedStage);
    }

    public void Load()
    {
        clearedStage = PlayerPrefs.GetInt("ClearedStage",0);
    }

    [System.Serializable]
    public class StageDataWrapper
    {
        public List<StageData> stages;
    }

    public IEnumerator LoadStage()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "stage.json");

        UnityWebRequest request = UnityWebRequest.Get(path);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Stage load failed: " + request.error);
            yield break;
        }

        string json = request.downloadHandler.text;

        stageDataList = JsonUtility.FromJson<StageDataWrapper>(json).stages;
        Debug.Log("스테이지 수: " + stageDataList.Count);
    }
}
