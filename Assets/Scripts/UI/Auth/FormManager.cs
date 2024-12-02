using UnityEngine;

public class FormsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _authForm, _registrationForm, _resetPasswordForm, _confirmAccountForm;

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
        _registrationForm.SetActive(false);
    }

    private void GoToRegistrationForm()
    {
        _registrationForm.SetActive(true);
        _authForm.SetActive(false);
    }

    public void ShowResetPasswordForm()
    {
        _resetPasswordForm.SetActive(true);
    }

    public void HideResetPasswordForm()
    {
        _resetPasswordForm.SetActive(false);
    }

    public void ShowConfirmAccountForm()
    {
        _confirmAccountForm.SetActive(true);
    }

    public void HideConfirmAccountForm()
    {
        _confirmAccountForm.SetActive(false);
    }
}
