using System.Collections;
using System.Collections.Generic;
using System.IO;
using Siccity.GLTFUtility;
using UnityEditor.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MainController : MonoBehaviour
{
    [SerializeField] private XRReferenceImageLibrary _xrReferenceImageLibrary;
    private ARTrackedImageManager _trackedImageManager;

    public Texture2D testImage;
    void Start()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
        
        GameObject model = null;
        if (File.Exists(FilesPath.modelPath))
             model = Importer.LoadFromFile(FilesPath.modelPath);
        // Texture2D image = LoadPNG(FilesPath.imagePath);
        // Texture2D image = LoadPNG(Application.dataPath+"/Resources"+"/targetImage.png");
        Texture2D image = Resources.Load<Texture2D>("targetImage");
        // Texture2D image = FilesPath._Texture2D;
        
        if (!model || !image)
        {
            SceneManager.LoadScene(0);
            return;
        }
        Debug.Log(image.name +" : "+image.format+" : " + image.height );
        // Debug.Log(image2.name +" : "+image2.format+" : " + image2.height );

        _trackedImageManager.trackedImagePrefab = model;
        _xrReferenceImageLibrary.SetTexture(0, image, true);
        // testImage = image2;
    }
    
    public Texture2D LoadPNG(string filePath) {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath)) 	{
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(352, 452,TextureFormat.RGBA64, true);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            testImage = tex;
        }
        return tex;
    }
}
