using UnityEngine;

public class FormManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _authForm, _registrationForm, _messageForm;

    public void GoToAuthForm()
    {
        _authForm.SetActive(true);
        _registrationForm.SetActive(false);
    }

    public void GoToRegistrationForm()
    {
        _registrationForm.SetActive(true);
        _authForm.SetActive(false);
    }

    public void ShowMessage()
    {
        _messageForm.SetActive(true);
    }

    public void HideMessage()
    {
        _messageForm.SetActive(false);
    }
}
