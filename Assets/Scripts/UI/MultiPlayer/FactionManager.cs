using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public TMP_Dropdown _factions;

    private void Start()
    {
        _factions.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {
        PickFaction(index);
    }

    public void PickFaction(int factionIndex)
    {
        switch (factionIndex)
        {
            case 0:
                Debug.Log("�������: LightCastle");
                break;
            case 1:
                Debug.Log("�������: Citadel");
                break;
            default:
                Debug.Log("����������� �������");
                break;
        }
    }
}
