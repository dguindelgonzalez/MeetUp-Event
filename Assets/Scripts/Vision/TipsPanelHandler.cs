using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class TipsPanelHandler : MonoBehaviour,IFocusable, IInputClickHandler
{
    #region Public Properties
    public GameObject TipPanel;
    public Material rendButton;
    public Material onExitMaterial;
    public Material hoverMaterial;
    #endregion

    #region Public Methods
    public void OnFocusEnter()
    {
        onExitMaterial = this.gameObject.GetComponent<Renderer>().material;
        this.gameObject.GetComponent<Renderer>().material = hoverMaterial;
    }

    public void OnFocusExit()
    {
        this.gameObject.GetComponent<Renderer>().material = onExitMaterial;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        MainSceneManager.ApplicationState = MainSceneManager.ApplicationStateType.VisionState;

        Destroy(TipPanel);
    }
    #endregion
}
