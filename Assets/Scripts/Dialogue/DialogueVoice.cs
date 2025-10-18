using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DialogueVoice : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private Coroutine _currentVoiceCoroutine;

    private void OnValidate() => _audioSource = _audioSource ?? GetComponent<AudioSource>();

    public void StartTalking(AudioClip clip)
    {
        _audioSource.clip = clip;

        if (_currentVoiceCoroutine is not null) StopCoroutine(_currentVoiceCoroutine);

        _currentVoiceCoroutine = StartCoroutine(talking());
    }

    private IEnumerator talking()
    {
        _audioSource.Play();
        yield return _audioSource.clip.length;
        _currentVoiceCoroutine = null;
    }
}
