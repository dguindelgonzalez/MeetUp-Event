using Assets.Scripts;
using Assets.Scripts.MR_And_Computer_Vision;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MainSceneManager : MonoBehaviour {

    #region Private Fields
    private static ApplicationStateType applicationState;

    // you must insert your service key here!    
    private string authorizationKey = "0282728a679949fcac91cc8c066f462f";
    private const string ocpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";
    private string visionAnalysisEndpoint = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Tags";   // This is where you need to update your endpoint, if you set your location to something other than west-us.
    internal byte[] imageBytes;
    internal string imagePath;
    private static ApplicationStateType _applicationState;
    #endregion

    #region Public Properties
    public static MainSceneManager instance;
    public enum ApplicationStateType
    {
        IdleState = 0,
        SpeakState = 1,
        BotState = 2,
        CustomVisionState = 2,
    }

    public static ApplicationStateType ApplicationState
    {
        get { return applicationState; }
        set
        {
            applicationState = value;
            ChangeView(value);
        }
    }
    public Animator SceneAnimatorController;
    #endregion

    #region Unity Defaults Methods
    void Awake () {
        // allows this instance to behave like a singleton
        instance = this;
        ApplicationState = ApplicationStateType.IdleState;
    }
	
	void Update () {
		
	}
    #endregion

    #region Public Methods
    /// <summary>
    /// Call the Computer Vision Service to submit the image.
    /// </summary>
    public IEnumerator AnalyseLastImageCaptured()
    {
        WWWForm webForm = new WWWForm();
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(visionAnalysisEndpoint, webForm))
        {
            // gets a byte array out of the saved image
            imageBytes = GetImageAsByteArray(imagePath);
            unityWebRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            unityWebRequest.SetRequestHeader(ocpApimSubscriptionKeyHeader, authorizationKey);

            // the download handler will help receiving the analysis from Azure
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

            // the upload handler will help uploading the byte array with the request
            unityWebRequest.uploadHandler = new UploadHandlerRaw(imageBytes);
            unityWebRequest.uploadHandler.contentType = "application/octet-stream";

            yield return unityWebRequest.SendWebRequest();

            long responseCode = unityWebRequest.responseCode;

            try
            {
                string jsonResponse = null;
                jsonResponse = unityWebRequest.downloadHandler.text;

                // The response will be in Json format
                // therefore it needs to be deserialized into the classes AnalysedObject and TagData
                AnalysedObject analysedObject = new AnalysedObject();
                analysedObject = JsonUtility.FromJson<AnalysedObject>(jsonResponse);

                if (analysedObject.tags == null)
                {
                    Debug.Log("analysedObject.tagData is null");
                }
                else
                {
                    Dictionary<string, float> tagsDictionary = new Dictionary<string, float>();

                    foreach (TagData td in analysedObject.tags)
                    {
                        TagData tag = td as TagData;
                        tagsDictionary.Add(tag.name, tag.confidence);
                    }

                    ResultsLabel.instance.SetTagsToLastLabel(tagsDictionary);
                }
            }
            catch (Exception exception)
            {
                Debug.Log("Json exception.Message: " + exception.Message);
            }

            yield return null;
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Returns the contents of the specified file as a byte array.
    /// </summary>
    private static byte[] GetImageAsByteArray(string imageFilePath)
    {
        FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader = new BinaryReader(fileStream);
        return binaryReader.ReadBytes((int)fileStream.Length);
    }

    private static void ChangeView(ApplicationStateType value)
    {

        switch(value)
        {
            case ApplicationStateType.IdleState:
                break;
            case ApplicationStateType.BotState:
                break;
            case ApplicationStateType.CustomVisionState:
                break;
        }
    }
    #endregion
}
