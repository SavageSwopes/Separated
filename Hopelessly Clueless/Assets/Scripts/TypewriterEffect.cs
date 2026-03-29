using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_Text))]
public class NewMonoBehaviourScript : MonoBehaviour
{
    private TMP_Text _textBox;

    [Header("Text String")]
    [TextArea(5,15)]
    [SerializeField] private string testText;

    private int _currentVisibleCharacterIndex;
    private Coroutine _typeWriterCoroutine;

    private WaitForSeconds _simpleDelay;
    private WaitForSeconds _interpunctuationDelay;

    [Header("Typewriter Settings")]
    [SerializeField] private float characterPerSecond = 20;
    [SerializeField] private float interpunctuationDelay = 0.5f;
   
    public bool CurrentlySkipping { get; private set; }
    private WaitForSeconds _skipDelay;

    [Header("Skip Options")]
    [SerializeField] private bool quickSkip;
    [SerializeField][Min(1)] private int skipSpeedup = 5;

    [Header("Events")]
    public UnityEvent onTextFinished;

    [Header("Auto Clear Settings")]
    [SerializeField] private float timeBeforeClearing = 2f;

    public static event Action OnTextFinished;
    public static event Action<char> CharacterRevealed;

    private void Awake()
    {
        _textBox = GetComponent<TMP_Text>();

        _simpleDelay = new WaitForSeconds(1 / characterPerSecond);
        _interpunctuationDelay = new WaitForSeconds(interpunctuationDelay);
        _skipDelay = new WaitForSeconds(1 / (characterPerSecond * skipSpeedup));
    }

    private void Start()
    {
        SetText(testText);
    }

    public void SetText(string text)
    {
        _textBox.text = text;

        _textBox.ForceMeshUpdate();

        _textBox.maxVisibleCharacters = 0;
        _currentVisibleCharacterIndex = 0; 

        _typeWriterCoroutine = StartCoroutine(Typewriter());
    }

    public void OnRightClick()
    {
        if (_textBox.maxVisibleCharacters < _textBox.textInfo.characterCount)
        {
            Skip();
        }
    }

    void Skip()
    {
        if (CurrentlySkipping) return;
        CurrentlySkipping = true;

        if (!quickSkip)
        {
            StartCoroutine(SkipSpeedupReset());
        }
    }

    private IEnumerator SkipSpeedupReset()
    {
        
        yield return new WaitUntil(() => _textBox.maxVisibleCharacters >= _textBox.textInfo.characterCount);
        CurrentlySkipping = false;
    }

    private IEnumerator Typewriter()
    {
        TMP_TextInfo textInfo = _textBox.textInfo;

        while (_currentVisibleCharacterIndex < textInfo.characterCount)
        {
            char character = textInfo.characterInfo[_currentVisibleCharacterIndex].character;
            _textBox.maxVisibleCharacters++;

            if (!CurrentlySkipping && (character == '?' || character == ',' || character == '.' || character == ':' || character == ';' || character == '!' || character == '-'))
            {
                yield return _interpunctuationDelay;
            }
            else
            {
                yield return CurrentlySkipping ? _skipDelay : _simpleDelay;
            }
            _currentVisibleCharacterIndex++;
        }

        //Tells the rest of the game so the player can move again.
        onTextFinished?.Invoke();

        yield return new WaitForSeconds(timeBeforeClearing);

        _textBox.text = "";
        _textBox.maxVisibleCharacters = 0;
    }
}