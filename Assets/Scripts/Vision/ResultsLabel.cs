using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsLabel : MonoBehaviour
{
    #region Public Properties
    public static ResultsLabel instance;
    public GameObject cursor;
    public Transform labelPrefab;
    [HideInInspector]
    public Transform lastLabelPlaced;
    [HideInInspector]
    public TextMesh lastLabelPlacedText;
    public Text ComputerText;
    public Text CustomText;
    #endregion

    #region Unity Default Methods
    private void Awake()
    {
        // allows this instance to behave like a singleton
        instance = this;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Instantiate a Label in the appropriate location relative to the Main Camera.
    /// </summary>
    public void CreateLabel()
    {
        ComputerText.text = "Contacting Cumputer Vision";
        CustomText.text = "Contacting Custom Vision";
    }

    /// <summary>
    /// Set the Tags as Text of the last Label created. 
    /// </summary>
    public void SetTagsToComputertLabel(Dictionary<string, float> tagsDictionary)
    {
        ComputerText.text = "Computer vision: \n";

        foreach (KeyValuePair<string, float> tag in tagsDictionary)
        {
            ComputerText.text += $"{tag.Key} Confidence: {tag.Value.ToString("0.00 \n")}";
        }
    }

    public void SetTagsToCustomLabel(Dictionary<string, double> tagsDictionary)
    {
        if (tagsDictionary != null)
        {
            CustomText.text = "Custom Vision: \n";

            foreach (KeyValuePair<string, double> tag in tagsDictionary)
            {
                CustomText.text += $"{tag.Key} Accuracy: { tag.Value.ToString("0.00 \n")}";
            }
        }
        else
        {
            CustomText.text = "Not objects recognized";
        }
    }
    #endregion

}
