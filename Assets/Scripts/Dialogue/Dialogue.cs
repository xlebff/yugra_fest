using UnityEngine;
public enum DialogugeConfig { GUI, VOICE, BOTH }

public class Dialogue : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private DialogugeConfig _config;
    [SerializeField] private MonoBehaviour[] _nextActions;

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

    private void Start() => XRInputManager.Instance.OnSecondaryButtonPressed += ShowNextDialog;

    private void OnEnable()
    {
        MovingManager.Instance.MoveDisabling();
        if (isPrintable()) _gui.GUIOpen();
        ShowNextDialog();
    }

    private void OnDisable() 
    {
        if (isPrintable()) _gui.GUIClose();

        Destroy(_gui);
        Destroy(_voice);

        if (MovingManager.Instance is not null) MovingManager.Instance.MoveEnabling();
        if (XRInputManager.Instance is not null) XRInputManager.Instance.OnSecondaryButtonPressed -= ShowNextDialog;

        foreach (MonoBehaviour action in _nextActions) action.enabled = true;
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

    private bool isPrintable() => _config == DialogugeConfig.BOTH || _config == DialogugeConfig.GUI;

    private bool isVoice() => _config == DialogugeConfig.VOICE || _config == DialogugeConfig.BOTH;

    public string getLastSentence() => this._sentences[--_currentIndex];
}
