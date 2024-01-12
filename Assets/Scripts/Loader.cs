using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Siccity.GLTFUtility;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

static class FilesPath
{
    public static string imagePath;
    public static string modelPath;
}
public class Loader : MonoBehaviour
{
    [SerializeField]private string _imageUrl;
    [SerializeField]private string _modelUrl;
    [SerializeField]private GameObject _loadingUI;
    [SerializeField]private GameObject _failedUI;
    private Action<bool> _connectionCallback;
    private int _downloadSuccess;

    // Start is called before the first frame update
    void Start()
    {
        _connectionCallback += b =>
        {
            switch (b)
            {
                case true :
                    StartCoroutine(SuccessToConnect());
                    break;
                case false :
                    FailedToConnect();
                    break;
            } 
        };
        StartCoroutine(CheckInternetConnection(_connectionCallback));
    }

    public static IEnumerator CheckInternetConnection(Action<bool> syncResult)
    {
        const string echoServer = "https://google.com";

        bool result;
        using (var request = UnityWebRequest.Head(echoServer))
        {
            request.timeout = 3;
            yield return request.SendWebRequest();
            result = request.result == UnityWebRequest.Result.Success;
        }
        syncResult.Invoke(result);
    }
    public void Reload()
    {
        StartCoroutine(CheckInternetConnection(_connectionCallback));
    }
    void FailedToConnect()
    {
        _loadingUI.SetActive(false);
        _failedUI.SetActive(true);
    }

    IEnumerator SuccessToConnect()
    {
        yield return LoadTextureFromWeb();
        yield return LoadModelFromWeb();
        if (_downloadSuccess >= 2)
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
            FailedToConnect();
    }
    IEnumerator LoadTextureFromWeb()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(_imageUrl);
        yield return www.SendWebRequest();
         
        _loadingUI.SetActive(true);
        _failedUI.SetActive(false);
        if (www.result == UnityWebRequest.Result.ConnectionError 
            || www.result == UnityWebRequest.Result.DataProcessingError 
            || www.result == UnityWebRequest.Result.ProtocolError)
        {
            FailedToConnect();
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
            WriteImageOnDisk(Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero));
            _downloadSuccess++;
        }
    }
    private IEnumerator LoadModelFromWeb() {
 
        UnityWebRequest request = new UnityWebRequest(_modelUrl);
        string savePath = string.Format("{0}/{1}.glb", Application.streamingAssetsPath, "targetModel");
        request.downloadHandler = new DownloadHandlerFile(savePath);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError 
            || request.result == UnityWebRequest.Result.DataProcessingError 
            || request.result == UnityWebRequest.Result.ProtocolError)
        {
            FailedToConnect();
            Debug.LogError("Error: " + request.error);
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            FilesPath.modelPath = savePath;
            // GameObject model = Importer.LoadFromFile(savePath);
            while (!File.Exists(FilesPath.imagePath) || !File.Exists(FilesPath.modelPath))
                yield return new WaitForSeconds(0.5f);
            _downloadSuccess++;
        }

    }
    private void WriteImageOnDisk(Sprite _sprite)
    {
        byte[] textureBytes = _sprite.texture.EncodeToPNG();
        string savePath = string.Format("{0}/{1}.png", Application.dataPath+"/Resources", "targetImage");
        File.WriteAllBytes(savePath, textureBytes);
        FilesPath.imagePath = savePath;
    }
}
