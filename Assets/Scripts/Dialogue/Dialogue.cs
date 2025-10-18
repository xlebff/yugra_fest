using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public enum DialogugeConfig { GUI, VOICE, BOTH }

public class Dialogue : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private DialogugeConfig _config;

    [Header("Voice")]
    [SerializeField] private AudioClip[] _voiceLines;

    [Header("GUI")]
    [SerializeField] private string[] _sentences;

    private DialogueGUI _gui;
    private DialogueVoice _voice;

    private int _currentIndex = 0;

    private void OnValidate() 
    {
        if ((_sentences.Length == 0) && isPrintable())
            Debug.LogErrorFormat("Empty dialogue text.");
        if ((_voiceLines.Length == 0) && isVoice())
            Debug.LogErrorFormat("Empty voice lines.");

        _gui = GetComponent<DialogueGUI>();
        _voice = GetComponent<DialogueVoice>();

        Debug.Log("Dialogue setup fineshed.");

        this.enabled = false;
    }

    private void Start() 
    { 
        XRInputManager.Instance.OnSecondaryButtonPressed += ShowNextDialog;
    }

    private void OnEnable()
    {
        if (isPrintable()) _gui.GUIOpen();
        ShowNextDialog();
    }

    private void OnDisable() 
    {
        if (isPrintable()) _gui.GUIClose();
        Destroy(_gui);
        Destroy(_voice);
        MoveEnabling();
        if (XRInputManager.Instance != null) XRInputManager.Instance.OnSecondaryButtonPressed -= ShowNextDialog;
    }

    private void MoveEnabling()
    {
        var locomotionSystem = FindObjectOfType<LocomotionSystem>();
        var continuousMove = FindObjectOfType<ContinuousMoveProviderBase>();
        var snapTurn = FindObjectOfType<SnapTurnProviderBase>();

        if (locomotionSystem != null) locomotionSystem.enabled = true;
        if (continuousMove != null) continuousMove.enabled = true;
        if (snapTurn != null) snapTurn.enabled = true;

        Debug.Log("Moving has been enabled.");
    }

    public void ShowNextDialog()
    {
        if (_currentIndex >= _sentences.Length)
        {
            Debug.Log("End of sentences.");
            this.enabled = false;
            return;
        }

        StartDialog();
        ++_currentIndex;
    }

    private void StartDialog()
    {
        switch (_config)
        {
            case DialogugeConfig.BOTH:
                if (_gui.StartPrinting(_sentences[_currentIndex]))
                    _voice.StartTalking(_voiceLines[_currentIndex]);
                else
                    _gui.setText(getLastSentence());
                break;
            case DialogugeConfig.GUI:
                if (!_gui.StartPrinting(_sentences[_currentIndex]))
                    _gui.setText(getLastSentence());
                break;
            case DialogugeConfig.VOICE:
                _voice.StartTalking(_voiceLines[_currentIndex]);
                break;
        }
    }

    private bool isPrintable()
    {
        return _config == DialogugeConfig.BOTH || _config == DialogugeConfig.GUI;
    }

    private bool isVoice()
    {
        return _config == DialogugeConfig.VOICE || _config == DialogugeConfig.BOTH;
    }

    public string getLastSentence() { return this._sentences[--_currentIndex]; }
}
