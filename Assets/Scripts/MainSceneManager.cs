
using Assets.BotDirectLine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class MainSceneManager : MonoBehaviour
{

    #region Private Fields
    private static ApplicationStateType applicationState;
    private const string DirectLineKey = "yPS_3_qNtp8.cwA.Doo.pIyO5bOUHHCEJGGTjfpK-0oFXCtpyIUDpaLG_EKG4nI";
    private DictationRecognizer dictationRecognizer;
    private TextToSpeech textToSpeech;
    private string conversationId;
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
        VisionState = 2,
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

        textToSpeech = gameObject.AddComponent<TextToSpeech>();
        textToSpeech.Voice = TextToSpeechVoice.Mark;

        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;
    }

    void Update()
    {
        if (textToSpeech.IsSpeaking())
        {
            SceneAnimatorController.SetInteger("ChangeStateAnimation", 1);
        }
        else
        {
            SceneAnimatorController.SetInteger("ChangeStateAnimation", 0);
        }
    }

    #endregion

    #region Public Methods
    public void OnVoiceCommand(string command)
    {
        switch (command)
        {
            case "Bot":
                if (ApplicationState != ApplicationStateType.BotState)
                {
                    SceneAnimatorController.SetTrigger("ChatbotState");

                    BotDirectLineManager.Initialize(DirectLineKey);
                    BotDirectLineManager.Instance.BotResponse += OnBotResponse;
                    BotDirectLineManager.Instance.StartConversationAsync().Wait();
                }

                ApplicationState = ApplicationStateType.BotState;

                if (dictationRecognizer.Status != SpeechSystemStatus.Stopped)
                {
                    dictationRecognizer.Stop();
                }

                break;

            case "Vision":
                if (ApplicationState != ApplicationStateType.VisionState)
                {
                    SceneAnimatorController.SetTrigger("IronVisionState");
                    ApplicationState = ApplicationStateType.VisionState;

                    if (dictationRecognizer.Status != SpeechSystemStatus.Stopped)
                    {
                        dictationRecognizer.Stop();
                    }
                }

                break;

            case "STT":
                if (applicationState == ApplicationStateType.BotState)
                {
                    PhraseRecognitionSystem.Shutdown();

                    dictationRecognizer.Start();
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
            case ApplicationStateType.VisionState:
                break;
        }
    }

    private void OnBotResponse(object sender, Assets.BotDirectLine.BotResponseEventArgs e)
    {
        Debug.Log($"OnBotResponse: {e.ToString()}");

        switch (e.EventType)
        {
            case EventTypes.ConversationStarted:
                if (!string.IsNullOrWhiteSpace(e.ConversationId))
                {
                    // Store the ID
                    textToSpeech.SpeakSsml("<?xml version=\"1.0\"?><speak speed=\"80%\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.w3.org/2001/10/synthesis http://www.w3.org/TR/speech-synthesis/synthesis.xsd\" xml:lang=\"en-US\">Bot connection established!</speak>");
                    conversationId = e.ConversationId;
                }
                else
                {
                    textToSpeech.SpeakSsml("<?xml version=\"1.0\"?><speak speed=\"80%\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.w3.org/2001/10/synthesis http://www.w3.org/TR/speech-synthesis/synthesis.xsd\" xml:lang=\"en-US\">Error while connecting to Bot!</speak>");
                }
                break;
            case EventTypes.MessageSent:
                if (!string.IsNullOrEmpty(conversationId))
                {
                    // Get the bot's response(s)
                    BotDirectLineManager.Instance.GetMessagesAsync(conversationId).Wait();
                }

                break;
            case EventTypes.MessageReceived:
                // Handle the received message(s)
                if (!string.IsNullOrWhiteSpace(conversationId))
                {
                    var messageActivity = e.Messages.LastOrDefault();
                    Debug.Log(messageActivity.Text);
                    textToSpeech.SpeakSsml("<?xml version=\"1.0\"?><speak speed=\"80%\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.w3.org/2001/10/synthesis http://www.w3.org/TR/speech-synthesis/synthesis.xsd\" xml:lang=\"en-US\"> " + messageActivity.Text + "</speak>");
                }
                break;
            case EventTypes.Error:
                // Handle the error
                break;
        }
    }

    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        Debug.Log($"DictationRecognizer_DictationError: {error}");
    }

    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        Debug.Log($"DictationRecognizer_DictationComplete: {cause.ToString()}");

        if (cause != DictationCompletionCause.Complete)
        {
            this.dictationRecognizer.Start();
            Debug.Log("dictationRecognizer.Start");
        }
    }

    private void DictationRecognizer_DictationHypothesis(string text)
    {
        Debug.Log($"DictationRecognizer_DictationHypothesis: {text}");
    }

    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log($"DictationRecognizer_DictationResult: {text}confidence: {confidence.ToString()}");

        if (confidence == ConfidenceLevel.Rejected || confidence == ConfidenceLevel.Low)
        {
            textToSpeech.SpeakSsml("<?xml version=\"1.0\"?><speak speed=\"80%\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.w3.org/2001/10/synthesis http://www.w3.org/TR/speech-synthesis/synthesis.xsd\" xml:lang=\"en-US\">Sorry, but I don't understand you.</speak>");
        }

        if (!string.IsNullOrWhiteSpace(conversationId))
        {
            BotDirectLineManager.Instance.SendMessageAsync(conversationId, "Cain", text).Wait();
        }
    }

    #endregion
}
