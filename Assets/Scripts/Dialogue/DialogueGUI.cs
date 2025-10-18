using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueGUI : MonoBehaviour
{
    [SerializeField] private GameObject _gui;
    [SerializeField] private TMP_Text _textField;
    [SerializeField] private float _printingSpeed = 1f;

    private Coroutine _currentPrintingCoroutine = null;

    private void OnValidate()
    {
        _gui.SetActive(false);
    }

    public void GUIOpen()
    {
        _gui.SetActive(true);
        _textField.text = string.Empty;
        Debug.Log("Dialogue GUI opened.");
    }

    public void GUIClose()
    {
        _textField.text = string.Empty;
        _gui.SetActive(false);
        Debug.Log("Dialogue GUI closed.");
    }

    public bool StartPrinting(string sentence)
    {
        if (_currentPrintingCoroutine is not null)
        {
            StopCoroutine(_currentPrintingCoroutine);
            _currentPrintingCoroutine = null;
            return false;
        }
        else _currentPrintingCoroutine = StartCoroutine(printingSentence(sentence));
        return true;
    }

    public void setText(string setnetce) { _textField.text = setnetce; }

    private IEnumerator printingSentence(string sentence)
    {
        _textField.text = string.Empty;

        for (int i = 0; i < sentence.Length; ++i)
        {
            _textField.text += sentence[i];
            yield return new WaitForSeconds(_printingSpeed);
        }
    }
}
