using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FormsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _authForm, _registrationForm, _messageForm, _resetPasswordForm, _resetPasswordBtn;

    public static FormsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void GoToAuthForm()
    {
        _authForm.SetActive(true);
        _resetPasswordBtn.SetActive(true);

        _registrationForm.SetActive(false);
    }

    private void GoToRegistrationForm()
    {
        _registrationForm.SetActive(true);

        _authForm.SetActive(false);
        _resetPasswordBtn.SetActive(false);
    }

    public void ShowMessage(string text)
    {
        _resetPasswordBtn.SetActive(false);

        _messageForm.GetComponentInChildren<TextMeshProUGUI>().text = text;
        _messageForm.SetActive(true);
    }

    public void HideMessage()
    {
        _messageForm.SetActive(false);

        if (_authForm.activeSelf)
        {
            _resetPasswordBtn.SetActive(true);
        }
    }

    public void ShowResetPasswordForm()
    {
        _resetPasswordForm.SetActive(true);

        _resetPasswordBtn.SetActive(false);
    }

    public void HideResetPasswordForm()
    {
        _resetPasswordForm.SetActive(false);

        _resetPasswordBtn.SetActive(true);
    }
}
