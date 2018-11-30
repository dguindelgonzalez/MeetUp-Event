using Assets.Scripts;
using Assets.Scripts.MR_And_Computer_Vision;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class VisionManager : MonoBehaviour
{

    #region Private Fields
    // you must insert your service key here!    
    private string computerVisionAuthorizationKey = Keys.ComputerVisionAuthoritationKey;
    private const string ocpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";
    private string computerVisionAnalysisEndpoint = Keys.ComputerVisionEndpoint;
    private string customVisionAuthorizationKey = Keys.CustomVisionAuthoritationKey;
    private string customVisionAnalysisEndpoint = Keys.CustomVisionEndpoint;
    internal byte[] imageBytes;
    internal string imagePath;
    #endregion

    #region Public Properties
    public static VisionManager instance;
    public float probabilityAccuracyCustomPrediction = 60;
    #endregion

    #region Unity3D Default Methods
    private void Awake()
    {
        // allows this instance to behave like a singleton
        instance = this;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Call the Computer Vision Service to submit the image.
    /// </summary>
    public IEnumerator AnalyseComputerImageCaptured()
    {
        WWWForm webForm = new WWWForm();
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(computerVisionAnalysisEndpoint, webForm))
        {
            // gets a byte array out of the saved image
            imageBytes = GetImageAsByteArray(imagePath);
            unityWebRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            unityWebRequest.SetRequestHeader(ocpApimSubscriptionKeyHeader, computerVisionAuthorizationKey);

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

                    ResultsLabel.instance.SetTagsToComputertLabel(tagsDictionary);
                }
            }
            catch (Exception exception)
            {
                Debug.Log("Json exception.Message: " + exception.Message);
            }

            yield return null;
        }
    }


    /// <summary>
    /// Call the Computer Vision Service to submit the image.
    /// </summary>
    public IEnumerator AnalyseCustomtImageCaptured()
    {
        WWWForm webForm = new WWWForm();
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(customVisionAnalysisEndpoint, webForm))
        {
            // Gets a byte array out of the saved image
            imageBytes = GetImageAsByteArray(imagePath);

            unityWebRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            unityWebRequest.SetRequestHeader("Prediction-Key", customVisionAuthorizationKey);

            // The upload handler will help uploading the byte array with the request
            unityWebRequest.uploadHandler = new UploadHandlerRaw(imageBytes);
            unityWebRequest.uploadHandler.contentType = "application/octet-stream";

            // The download handler will help receiving the analysis from Azure
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

            // Send the request
            yield return unityWebRequest.SendWebRequest();

            string jsonResponse = unityWebRequest.downloadHandler.text;

            // The response will be in JSON format, therefore it needs to be deserialized    

            // The following lines refers to a class that you will build in later Chapters
            // Wait until then to uncomment these lines

            AnalysisObject analysisObject = new AnalysisObject();
            analysisObject = JsonConvert.DeserializeObject<AnalysisObject>(jsonResponse);

            if (analysisObject != null)
            {
                Dictionary<string, double> tagsDictionary = new Dictionary<string, double>();

                foreach (Prediction td in analysisObject.Predictions)
                {
                    var accuracyRequest = td.Probability * 100.00;

                    if (tagsDictionary.ContainsKey(td.TagName))
                    {
                        if (tagsDictionary[td.TagName] < td.Probability)
                        {
                            tagsDictionary[td.TagName] = td.Probability;
                        }
                    }
                    else
                    {
                        tagsDictionary.Add(td.TagName, td.Probability);
                    }
                }

                ResultsLabel.instance.SetTagsToCustomLabel(tagsDictionary);
            }
            //SceneOrganiser.Instance.SetTagsToLastLabel(analysisObject);
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
    #endregion

}
