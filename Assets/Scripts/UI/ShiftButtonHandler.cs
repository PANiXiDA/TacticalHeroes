using Assets.Scripts.Enumerations;
using Assets.Scripts.Managers;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShiftButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject _shiftBtn;

    public static ShiftButtonHandler Instance;

    public bool isLeftShiftPressed;

    private Image shiftBtnImage;

    private Sprite shiftBtn;

    private Sprite shiftOffBtn;

    private bool UIShiftBtnPressed;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        shiftBtn = Resources.Load<Sprite>("UI/ShiftBtn");
        shiftOffBtn = Resources.Load<Sprite>("UI/ShiftOffBtn");
        shiftBtnImage = _shiftBtn.GetComponent<Image>();
        UIShiftBtnPressed = false;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentFaction == Faction.Hero)
        {
            if (!UIShiftBtnPressed)
            {
                isLeftShiftPressed = Input.GetButton("LeftShift");
                if (isLeftShiftPressed)
                {
                    if (shiftBtnImage.sprite != shiftOffBtn)
                    {
                        shiftBtnImage.sprite = shiftOffBtn;
                    }
                }
                else
                {
                    if (shiftBtnImage.sprite != shiftBtn)
                    {
                        shiftBtnImage.sprite = shiftBtn;
                    }
                }
            }
            else
            {
                isLeftShiftPressed = Input.GetButton("LeftShift");
                if (isLeftShiftPressed)
                {
                    UIShiftBtnPressed = false;
                    isLeftShiftPressed = false;
                    shiftBtnImage.sprite = shiftBtn;
                }
            }
        }
        else
        {
            UIShiftBtnPressed = false;
            isLeftShiftPressed = false;
            shiftBtnImage.sprite = shiftBtn;
        }
    }
    public void PressShift()
    {
        if (GameManager.Instance.CurrentFaction == Faction.Hero)
        {
            if (shiftBtnImage.sprite != shiftOffBtn)
            {
                UIShiftBtnPressed = true;
                isLeftShiftPressed = true;
                shiftBtnImage.sprite = shiftOffBtn;
            }
            else
            {
                UIShiftBtnPressed = false;
                isLeftShiftPressed = false;
                shiftBtnImage.sprite = shiftBtn;
            }
        }
        else
        {
            UIShiftBtnPressed = false;
            isLeftShiftPressed = false;
            shiftBtnImage.sprite = shiftBtn;
        }
    }
}
