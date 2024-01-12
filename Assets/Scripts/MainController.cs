using System.Collections;
using System.Collections.Generic;
using System.IO;
using Siccity.GLTFUtility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
// using UnityEditor.XR.ARSubsystems;

public class MainController : MonoBehaviour
{
    [SerializeField]private XRReferenceImageLibrary _xrReferenceImageLibrary;
    // private ARTrackedImageManager _trackedImageManager;
    private MutableRuntimeReferenceImageLibrary _mutableLibrary;
    

    void Awake()
    {
        // _trackedImageManager = GetComponent<ARTrackedImageManager>();
        var trackImageManager = gameObject.AddComponent<ARTrackedImageManager>();
        trackImageManager.referenceLibrary = trackImageManager.CreateRuntimeLibrary(_xrReferenceImageLibrary);
        trackImageManager.maxNumberOfMovingImages = 3;
        // trackImageManager.trackedImagePrefab = prefabOnTrack;

        trackImageManager.enabled = true;

        // trackImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        
        // var runtimeReferenceImageLibrary = trackImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;
        _mutableLibrary = trackImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;
        // _mutableLibrary = trackImageManager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;
        // RuntimeReferenceImageLibrary runtimeLibrary = _trackedImageManager.CreateRuntimeLibrary();
        // _xrReferenceImageLibrary = runtimeLibrary as MutableRuntimeReferenceImageLibrary;

        GameObject model = null;
        if (File.Exists(FilesPath.modelPath))
             model = Importer.LoadFromFile(FilesPath.modelPath);
        Texture2D image = LoadPNG(FilesPath.imagePath);
        // Texture2D image = LoadPNG(Application.dataPath+"/Resources"+"/targetImage.png");
        // Texture2D image = Resources.Load<Texture2D>("targetImage");
        // Texture2D image = FilesPath._Texture2D;
        
        if (!model || !image)
        {
            SceneManager.LoadScene(0);
            return;
        }
        Debug.Log(image.name +" : "+image.format+" : " + image.height );
        // Debug.Log(image2.name +" : "+image2.format+" : " + image2.height );

        trackImageManager.trackedImagePrefab = model;
        _mutableLibrary.ScheduleAddImageWithValidationJob(image, "target", 0.1f);
    }
    
    public Texture2D LoadPNG(string filePath) {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath)) 	{
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(352, 452,TextureFormat.RGBA64, true);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}
