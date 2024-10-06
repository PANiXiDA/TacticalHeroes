using Assets.Scripts.Integrations.Firebase.Interfaces;
using TMPro;
using UnityEngine;
using Zenject;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _emailAuth, _passwordAuth;

    [SerializeField]
    private TMP_InputField _nicknameRegistration, _emailRegistration, _passwordRegistration, _repeatPasswordRegistration;

    [SerializeField]
    private TMP_InputField _emailRecoveryPassword;

    private IFirebaseAuthService _auth;
    private IFirebaseRegistrationService _registration;
    private IFirestoreUserService _userData;

    private FormsManager _formsManager;

    [Inject]
    public void Construct(
        IFirebaseAuthService auth,
        IFirebaseRegistrationService registration,
        IFirestoreUserService userData)
    {
        _auth = auth;
        _registration = registration;
        _userData = userData;
    }

    private void Start()
    {
        _formsManager = FormsManager.Instance;
    }

    public async void OnLoginClicked()
    {
        try
        {
            var user = await _auth.SignIn(_emailAuth.text, _passwordAuth.text);
            SaveUserDataLocally(user);
            SceneManager.LoadScene(3);
        }
        catch (FirebaseException ex)
        {
            switch ((AuthError)ex.ErrorCode)
            {
                case AuthError.Failure:
                    _formsManager.ShowMessage("Wrong email or password!");
                    break;
                case AuthError.UserDisabled:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.MissingEmail:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.MissingPassword:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.UnverifiedEmail:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.InvalidEmail:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.WrongPassword:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.OperationNotAllowed:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.NetworkRequestFailed:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                default:
                    Debug.LogError($"{ex.Message}");
                    break;
            }
        }
    }

    private void SaveUserDataLocally(FirebaseUser user)
    {
        PlayerPrefs.SetString("UserId", user.UserId);
        PlayerPrefs.SetString("NickName", user.DisplayName);
        PlayerPrefs.SetString("ImageUrl", user.PhotoUrl != null ? user.PhotoUrl.ToString() : "");
    }

    public async void OnRegistrationClicked()
    {
        bool registrationSuccessful = false;

        if (string.IsNullOrEmpty(_nicknameRegistration.text))
        {
            _formsManager.ShowMessage("Enter your nickname!");
            return;
        }
        if (_passwordRegistration.text != _repeatPasswordRegistration.text)
        {
            _formsManager.ShowMessage("Passwords must match!");
            return;
        }

        //try
        //{
        //    Debug.Log("AuthManager start integration with FireStore...");
        //    var user = await _userData.GetUserByNicknameAsync(_nicknameRegistration.text);
        //    Debug.Log("AuthManager end integration with FireStore...");
        //    if (user != null)
        //    {
        //        _formsManager.ShowMessage("A user with this nickname has already been registered!");
        //        return;
        //    }
        //}
        //catch (FirestoreException ex)
        //{
        //    Debug.LogError(ex.Message);
        //    return;
        //}

        string userIdFirebase = string.Empty;
        try
        {
            userIdFirebase = await _registration.Register(_emailRegistration.text, _passwordRegistration.text,
                _nicknameRegistration.text);
            registrationSuccessful = true;
        }
        catch (FirebaseException ex)
        {
            switch ((AuthError)ex.ErrorCode)
            {
                case AuthError.MissingEmail:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.MissingPassword:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.EmailAlreadyInUse:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.InvalidEmail:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.WeakPassword:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.OperationNotAllowed:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.NetworkRequestFailed:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                default:
                    Debug.LogError($"{ex.Message}");
                    break;
            }
        }

        if (registrationSuccessful)
        {
            //var userFirestore = new SaveUserRequest()
            //{
            //    UserAuthId = userIdFirebase,
            //    Nickname = _nicknameRegistration.text,
            //    Email = _emailRegistration.text,
            //    IsBlocked = false,
            //    IsEmailConfirmed = false,
            //    CreatedAt = Timestamp.GetCurrentTimestamp(),
            //    UpdatedAt = Timestamp.GetCurrentTimestamp()
            //};
            //try
            //{
            //    await _userData.SaveUserAsync(userFirestore);
            //}
            //catch (FirestoreException ex)
            //{
            //    Debug.LogError(ex.Message);
            //}
            _formsManager.ShowMessage("Registration was successful. You will receive a confirmation message by email.");
        }
    }

    public async void RecoveryPassword()
    {
        bool recoveryPasswordSuccessful = false;
        try
        {
            await _auth.RecoveryPassword(_emailRecoveryPassword.text);
            recoveryPasswordSuccessful = true;
        }
        catch (FirebaseException ex)
        {
            switch ((AuthError)ex.ErrorCode)
            {
                case AuthError.MissingEmail:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.InvalidEmail:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.OperationNotAllowed:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                case AuthError.NetworkRequestFailed:
                    _formsManager.ShowMessage(ex.Message);
                    break;
                default:
                    Debug.LogError($"{ex.Message}");
                    break;
            }
        }
        if (recoveryPasswordSuccessful)
        {
            _formsManager.ShowMessage("A message with instructions on password recovery has been sent to your email.");
        }
    }
}
