using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lean.Common;
using Lean.Touch;
using Siccity.GLTFUtility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MainController : MonoBehaviour
{
    [SerializeField]private XRReferenceImageLibrary _xrReferenceImageLibrary;
    private ARTrackedImageManager trackImageManager;
    private MutableRuntimeReferenceImageLibrary _mutableLibrary;
    private GameObject _spawnedModel = null;
    [SerializeField]private GameObject _target;

    void Awake()
    {
        trackImageManager = gameObject.GetComponent<ARTrackedImageManager>();
        trackImageManager.referenceLibrary = trackImageManager.CreateRuntimeLibrary(_xrReferenceImageLibrary);

        trackImageManager.enabled = true;
        _mutableLibrary = trackImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;


        GameObject model = null;
        if (File.Exists(FilesPath.modelPath))
        {
            model = Importer.LoadFromFile(FilesPath.modelPath);
            model.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            model.tag = "Cake";
            model.AddComponent<BoxCollider>();
            var leanSel = model.AddComponent<LeanSelectable>();
            model.AddComponent<LeanTwistRotateAxis>();
            model.AddComponent<LeanPinchScale>();
        }

        Texture2D image = null;
        if (File.Exists(FilesPath.imagePath))
        {
            image = LoadPNG(FilesPath.imagePath);
        }

        if (!model || !image)
        {
            SceneManager.LoadScene(0);
            return;
        }

        model.transform.SetParent(_target.transform);
        trackImageManager.trackedImagePrefab = _target;
        try
        {
            _mutableLibrary.ScheduleAddImageWithValidationJob(image, "target", 0.1f);
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR : "+e);
        }
    }

    public Texture2D LoadPNG(string filePath) {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath)) 	{
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(352, 452,TextureFormat.RGBA32, false);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}
