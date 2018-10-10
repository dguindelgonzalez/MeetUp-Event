
using UnityEngine;

public class MainSceneManager : MonoBehaviour {

    #region Private Fields
    private static ApplicationStateType applicationState;
    #endregion

    #region Public Properties
    public static MainSceneManager instance;
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
