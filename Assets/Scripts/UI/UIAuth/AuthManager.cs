using Cysharp.Threading.Tasks;
using System.Text;
using TMPro;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class AuthManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _emailAuth, _passwordAuth;

    [SerializeField]
    private TMP_InputField _nicknameRegistration, _emailRegistration, _passwordRegistration, _repeatPasswordRegistration;

    [SerializeField]
    private TMP_InputField _emailRecoveryPassword;

    private FormsManager _formsManager;

    [Inject]
    public void Construct()
    {
    }

    private void Start()
    {
        _formsManager = FormsManager.Instance;
    }

    public async void OnLoginClicked()
    {

    }

    public class Registration
    {
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }

        public Registration(string nickName, string email, string password, string repeatPassword)
        {
            NickName = nickName;
            Email = email;
            Password = password;
            RepeatPassword = repeatPassword;
        }
    }

    public async void OnRegistrationClicked()
    {
        string url = "https://tacticalheroesdev.ru/api/v1/auth/sign-up";

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

        var registration = new Registration(
            _nicknameRegistration.text,
            _emailRegistration.text,
            _passwordRegistration.text,
            _repeatPasswordRegistration.text);

        string request = JsonConvert.SerializeObject(registration);

        await PostRequest(url, request);
    }

    private async UniTask PostRequest(string url, string jsonBody)
    {
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Запрос успешно выполнен: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Ошибка: " + request.error);
            }
        }
    }

    public async void RecoveryPassword()
    {
        bool recoveryPasswordSuccessful = false;

        if (recoveryPasswordSuccessful)
        {
            _formsManager.ShowMessage("A message with instructions on password recovery has been sent to your email.");
        }
    }
}
