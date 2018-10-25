
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{

    #region Private Fields
    private static ApplicationStateType applicationState;
    #endregion

    #region Public Properties
    public static MainSceneManager instance;
    public Material Traza01Mat;
    public Material Traza02Mat;
    public GameObject Cube;
    public Animator SceneAnimatorController;
    public enum ApplicationStateType
    {
        IdleState = 0,
        BotState = 1,
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
    #endregion

    #region Unity Defaults Methods
    void Awake()
    {
        // allows this instance to behave like a singleton
        instance = this;
        ApplicationState = ApplicationStateType.IdleState;
    }

    void Update()
    {
    }
    #endregion

    #region Public Methods
    public void OnVoiceCommand(string command)
    {
        switch (command)
        {
            case "Bot":
                if (ApplicationState == ApplicationStateType.CustomVisionState)
                {
                SceneAnimatorController.SetTrigger("ChatbotState");
                }
                Cube.GetComponent<Renderer>().material = Traza01Mat;
                ApplicationState = ApplicationStateType.BotState;
                break;

            case "Computer":
                if (ApplicationState != ApplicationStateType.CustomVisionState)
                {
                    SceneAnimatorController.SetTrigger("IronVisionState");
                    Cube.GetComponent<Renderer>().material = Traza02Mat;
                    ApplicationState = ApplicationStateType.CustomVisionState;
                }
                break;

            default:
                Debug.LogError("No Reconocido el comando");
                break;
        }
    }
    #endregion

    #region Private Methods
    private static void ChangeView(ApplicationStateType value)
    {
        switch (value)
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
