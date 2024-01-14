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
    [SerializeField]private GameObject _startUI;
    private int _downloadSuccess;

    
    public void Reload()
    {
        
        _loadingUI.SetActive(true);
        _failedUI.SetActive(false);
        _startUI.SetActive(false);
        StartCoroutine(DownloadFiles());
    }
    void FailedToConnect()
    {
        _loadingUI.SetActive(false);
        _failedUI.SetActive(true);
    }

    IEnumerator DownloadFiles()
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
        www.timeout = 10;
        yield return www.SendWebRequest();
         
        _loadingUI.SetActive(true);
        _failedUI.SetActive(false);

        if(www.result == UnityWebRequest.Result.Success)
        {
            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
            WriteImageOnDisk(Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero));
            _downloadSuccess++;
        }
        else
        {
            FailedToConnect();
            Debug.LogError("Error: " + www.error);
        }
    }
    private IEnumerator LoadModelFromWeb() {
 
        UnityWebRequest request = new UnityWebRequest(_modelUrl);
        string savePath = Path.Combine(Application.persistentDataPath, "targetModel.glb");
        request.downloadHandler = new DownloadHandlerFile(savePath);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            FilesPath.modelPath = savePath;
            while (!File.Exists(FilesPath.imagePath) || !File.Exists(FilesPath.modelPath))
                yield return new WaitForSeconds(0.5f);
            _downloadSuccess++;
        }
        else
        {
            FailedToConnect();
            Debug.LogError("Error: " + request.error);
        }

    }
    private void WriteImageOnDisk(Sprite _sprite)
    {
        byte[] textureBytes = _sprite.texture.EncodeToPNG();
        string savePath = Path.Combine(Application.persistentDataPath, "targetImage.png");
        File.WriteAllBytes(savePath, textureBytes);
        FilesPath.imagePath = savePath;
    }
}
