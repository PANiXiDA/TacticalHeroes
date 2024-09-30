using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Helpers;
using TMPro;
using Assets.Scripts.Enumerations;
using Assets.Scripts.Managers;
using Unity.Mathematics;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using System.Threading;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField]
    private GameObject _tileInfoPanel, _unitInfoPanel, _chatPanel, _endBattlePanel, _surrenderPanel, _exitBtn,
        _waitBtn, _defBtn, _shiftBtn, _timer, _factionChoosingPanel, _playerHeroPortret, _enemyHeroPortret;
    [SerializeField] private RectTransform _ATBIcons;
    private CancellationTokenSource _cancellationTokenSource;

    private void Awake()
    {
        Instance = this;
    }
    public void ShowOrientationOfAttack(Tile tile)
    {
        var enemyPos = GridManager.Instance.GetTileCoordinate(tile);
        var cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float normalizedAngle = Mathf.Atan2(cursorPos.y - enemyPos.y, cursorPos.x - enemyPos.x) * Mathf.Rad2Deg;
        normalizedAngle = (normalizedAngle + 360) % 360;

        var selectedHero = UnitManager.Instance.SelectedHero;
        var tilesForMove = UnitManager.Instance.GetTilesForMove(selectedHero);

        if (selectedHero != null)
        {
            if (UnitManager.Instance.IsRangeAttackPossible(selectedHero))
            {
                ShowArrowAttack(enemyPos, selectedHero);
            }
            else
            {
                ShowSwordAttack(enemyPos, normalizedAngle, tilesForMove);
            }
        }
    }
    private void ShowSwordAttack(Vector2 enemyPos, float normalizedAngle, Dictionary<Vector2, Tile> tilesForMove)
    {
        var swordsList = Resources.LoadAll<GameObject>("Swords").ToList();
        var swordPositions = new Dictionary<string, (Vector2 position, float angleRangeStart, float angleRangeEnd)>
        {
            {"Bottom", (new Vector2(enemyPos.x, enemyPos.y + 0.75f), 67.5f, 112.5f)},
            {"BottomLeft", (new Vector2(enemyPos.x + 0.75f, enemyPos.y + 0.75f), 22.5f, 67.5f)},
            {"Left", (new Vector2(enemyPos.x + 0.75f, enemyPos.y), 0f, 22.5f)},
            {"TopLeft", (new Vector2(enemyPos.x + 0.75f, enemyPos.y - 0.75f), 292.5f, 337.5f)},
            {"Top", (new Vector2(enemyPos.x, enemyPos.y - 0.75f), 247.5f, 292.5f)},
            {"TopRight", (new Vector2(enemyPos.x - 0.75f, enemyPos.y - 0.75f), 202.5f, 247.5f)},
            {"Right", (new Vector2(enemyPos.x - 0.75f, enemyPos.y), 157.5f, 202.5f)},
            {"BottomRight", (new Vector2(enemyPos.x - 0.75f, enemyPos.y + 0.75f), 112.5f, 157.5f)},
        };

        foreach (var (swordName, (position, angleStart, angleEnd)) in swordPositions)
        {
            var tilePos = new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            var tile = GridManager.Instance.GetTileAtPosition(tilePos);

            if (tile != null &&
                normalizedAngle >= angleStart && normalizedAngle < angleEnd &&
                GameObject.Find($"{swordName}(Clone)") is null &&
                (tilesForMove.ContainsKey(tilePos) || tile.OccupiedUnit == UnitManager.Instance.SelectedHero))
            {
                DeleteSwords();
                DeleteArrows();
                var sword = swordsList.Find(e => e.name == swordName);
                Instantiate(sword, position, Quaternion.identity);
            }
        }
    }
    private void ShowArrowAttack(Vector2 enemyPos, BaseUnit selectedHero)
    {
        var arrowsList = Resources.LoadAll<GameObject>("Arrows").ToList();

        var distance = math.ceil(math.sqrt(math.pow(enemyPos.x - selectedHero.OccupiedTile.Position.x, 2) +
            math.pow(enemyPos.y - selectedHero.OccupiedTile.Position.y, 2)));

        var tilePos = new Vector2(enemyPos.x - 1, enemyPos.y);
        var tile = GridManager.Instance.GetTileAtPosition(tilePos);

        Tile.Instance.DeleteHighlightForAttack();
        DeleteSwords();

        if (tile != null && GameObject.Find("Arrow(Clone)") is null && selectedHero.UnitRange >= distance)
        {
            DeleteArrows();
            var arrow = arrowsList.Find(e => e.name == "Arrow");
            Instantiate(arrow, new Vector3(tilePos.x + 0.25f, tilePos.y), Quaternion.identity);
        }
        else if (tile != null && GameObject.Find("BreakArrow(Clone)") is null && selectedHero.UnitRange < distance)
        {
            DeleteArrows();
            var arrow = arrowsList.Find(e => e.name == "BreakArrow");
            Instantiate(arrow, new Vector3(tilePos.x + 0.15f, tilePos.y), Quaternion.identity);
        }
    }
    public void DeleteSwords()
    {
        var swordsList = Resources.LoadAll<GameObject>("Swords").ToList();
        foreach (var sword in swordsList)
        {
            if (GameObject.Find(sword.name + "(Clone)") is not null)
            {
                Destroy(GameObject.Find(sword.name + "(Clone)"));
            }
        }
    }
    public void DeleteArrows()
    {
        var arrowsList = Resources.LoadAll<GameObject>("Arrows").ToList();
        foreach (var arrow in arrowsList)
        {
            if (GameObject.Find(arrow.name + "(Clone)") is not null)
            {
                Destroy(GameObject.Find(arrow.name + "(Clone)"));
            }
        }
    }
    public void DisplayDamageWithDeathCountInChat(BaseUnit attacker, BaseUnit defender, int damage, int countDeaths)
    {
        var attackerColor = attacker.Side == Side.Player ? "red" : "blue";
        var defenderColor = defender.Side == Side.Player ? "red" : "blue";
        _chatPanel.GetComponentInChildren<TextMeshProUGUI>().text += $"<color={attackerColor}>{attacker.UnitName}</color> нанес {damage} урона по <color={defenderColor}>{defender.UnitName}</color>." +
            $"{(countDeaths > 0 ? $" Погибло {countDeaths}.\n" : $"\n")}";

        Scrollbar scrollbar = _chatPanel.GetComponentInChildren<Scrollbar>(true);
        AutoScrollToBottom(scrollbar).Forget();
    }
    public void AddMessageToChat(string message)
    {
        _chatPanel.GetComponentInChildren<TextMeshProUGUI>().text += $"{message}\n";

        Scrollbar scrollbar = _chatPanel.GetComponentInChildren<Scrollbar>(true);
        AutoScrollToBottom(scrollbar).Forget();
    }
    public void SendMessageToChat()
    {
        TMP_InputField inputField = _chatPanel.GetComponentInChildren<TMP_InputField>();
        _chatPanel.GetComponentInChildren<TextMeshProUGUI>().text += $"<color=red>[Человеческий разум]</color>: {inputField.text}\n";
        inputField.text = "";

        Scrollbar scrollbar = _chatPanel.GetComponentInChildren<Scrollbar>(true);
        AutoScrollToBottom(scrollbar).Forget();
    }
    private async UniTaskVoid AutoScrollToBottom(Scrollbar scrollbar)
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        await UniTask.DelayFrame(2);

        scrollbar.value = 0f;
    }

    public void ShowUnitsPortraits()
    {
        foreach (Transform child in _ATBIcons)
        {
            Destroy(child.gameObject);
        }

        foreach (var ATBunit in TurnManager.Instance.ATB)
        {
            CreatePortrait(ATBunit.Value);
        }
    }

    public void ClearExistingIcons(BaseUnit unit)
    {
        foreach (Transform child in _ATBIcons)
        {
            if (child.name == unit.UnitName)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CreatePortrait(BaseUnit ATBunit)
    {
        string unitName = ATBunit.name.Replace("(Clone)", "");

        GameObject portrait = new GameObject(ATBunit.UnitName);
        portrait.transform.SetParent(_ATBIcons, false);

        Image image = portrait.AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>($"Icons/{ATBunit.Faction}/Tier{ATBunit.Tier}/{unitName}");

        CreateUnitCountText(portrait.transform, ATBunit.UnitCount);
        CreateContour(portrait.transform, ATBunit.Side);
    }
    public void UpdatePortraits(List<BaseUnit> newUnitsInATB)
    {
        if (_ATBIcons.childCount > 0)
        {
            Destroy(_ATBIcons.GetChild(0).gameObject);
        }

        foreach (var newUnit in newUnitsInATB)
        {
            CreatePortrait(newUnit);
        }
    }
    public void UpdatePortraitsInfo(BaseUnit unit)
    {
        if (unit.UnitCount > 0)
        {
            foreach (Transform child in _ATBIcons)
            {
                if (child.name == unit.UnitName)
                {
                    Transform unitCountChild = child.Find("UnitCount");
                    unitCountChild.GetComponent<TextMeshProUGUI>().text = unit.UnitCount.ToString();
                }
            }
        }
    }
    public void DeletePortrait(BaseUnit unit)
    {
        foreach (Transform child in _ATBIcons)
        {
            if (child.name == unit.UnitName)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    public void CreateUnitCountText(Transform parent, int unitCount)
    {
        GameObject unitCountObject = new GameObject("UnitCount");
        unitCountObject.transform.SetParent(parent, false);

        TextMeshProUGUI unitCountTextMeshPro = unitCountObject.AddComponent<TextMeshProUGUI>();
        unitCountTextMeshPro.rectTransform.localPosition = new Vector3(-3, -60, 0);
        unitCountTextMeshPro.rectTransform.sizeDelta = new Vector2(170, 40);
        unitCountTextMeshPro.text = unitCount.ToString();
        unitCountTextMeshPro.fontSize = 48;
        unitCountTextMeshPro.alignment = TextAlignmentOptions.Right;
        unitCountTextMeshPro.color = Color.yellow;
    }

    private void CreateContour(Transform parent, Side faction)
    {
        var sortingLayerID = SortingLayer.NameToID("Menu");

        GameObject contour = new GameObject("Contour");
        contour.transform.SetParent(parent, false);
        contour.transform.localPosition = new Vector3(180, -180, 0);
        contour.transform.localScale = new Vector3(170, 170, 1);

        SpriteRenderer spriteRenderer = contour.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("UI/Contour");
        spriteRenderer.sortingOrder = 1;
        spriteRenderer.sortingLayerID = sortingLayerID;
        spriteRenderer.color = faction == Side.Player ? Color.red : Color.blue;
    }
    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            HideTileInfoPanels();
            return;
        }

        UpdateTileInfoPanel(tile);
    }
    public void ShowUnitInfo(Tile tile)
    {
        if (tile == null)
        {
            return;
        }
        else if (tile.OccupiedUnit != null)
        {
            UpdateUnitInfoPanel(tile.OccupiedUnit);
        }
        else
        {
            _unitInfoPanel.SetActive(false);
        }
    }
    private void HideTileInfoPanels()
    {
        _tileInfoPanel.SetActive(false);
    }
    private void UpdateTileInfoPanel(Tile tile)
    {
        var tileCoordinate = GridManager.Instance.GetTileCoordinate(tile);
        _tileInfoPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"x = {tileCoordinate.x}\ny = {tileCoordinate.y}";
        _tileInfoPanel.SetActive(true);
    }
    private void UpdateUnitInfoPanel(BaseUnit unit)
    {
        string abilitiesText = string.Join(". ", unit.abilities.Select(ability => EnumHelper.GetDescription(ability)));

        SetUnitInfoText("UnitName", $"{unit.UnitName} [{unit.UnitCount}]");
        SetUnitInfoText("AttackValue", unit.UnitAttack.ToString());
        SetUnitInfoText("DefenceValue", $"{unit.UnitDefence} {(unit.UnitAdditionalDefence > 0 ? $"(+{unit.UnitAdditionalDefence})" : "")}");
        SetUnitInfoText("HealthValue", $"{unit.UnitCurrentHealth}/{unit.UnitFullHealth}");
        SetUnitInfoText("ArrowsValue", unit.UnitArrows != null ? unit.UnitArrows.ToString() : "-");
        SetUnitInfoText("RangeValue", unit.UnitRange != null ? unit.UnitRange.ToString() : "-");
        SetUnitInfoText("DamageValue", $"{unit.UnitMinDamage} - {unit.UnitMaxDamage}");
        SetUnitInfoText("SpeedValue", $"{unit.UnitSpeed}");
        SetUnitInfoText("InitiativeValue", unit.UnitInitiative.ToString().Replace(',', '.'));
        SetUnitInfoText("MoraleValue", unit.UnitMorale.ToString());
        SetUnitInfoText("LuckValue", unit.UnitLuck.ToString());
        SetUnitInfoText("Abilities", abilitiesText);

        _unitInfoPanel.SetActive(true);
    }
    private void SetUnitInfoText(string parameterName, string textValue)
    {
        _unitInfoPanel.GetComponentsInChildren<TextMeshPro>(true)
            .Where(item => item.name == parameterName)
            .FirstOrDefault()
            .text = textValue;
    }
    public void SetPanelTexts(GameObject panel, string battleResultText, string winSideInfoText, string loseSideInfoText)
    {
        var texts = panel.GetComponentsInChildren<TextMeshPro>(true);

        texts.FirstOrDefault(item => item.name == "BattleResultText").text = battleResultText;
        texts.FirstOrDefault(item => item.name == "WinSideInfoText").text = winSideInfoText;
        texts.FirstOrDefault(item => item.name == "LoseSideInfoText").text = loseSideInfoText;

        panel.SetActive(true);
    }

    public void WinPanel()
    {
        _exitBtn.SetActive(false);
        SetPanelTexts(
            _endBattlePanel,
            "Победа",
            "<color=#FF6666>Человеческий разум</color> победил!",
            "<color=#10CEEB>Искусственный интеллект</color> сегодня потерпел поражение!"
        );
    }

    public void LosePanel()
    {
        _exitBtn.SetActive(false);
        SetPanelTexts(
            _endBattlePanel,
            "Поражение",
            "<color=#FF6666>Искусственный интеллект</color> победил!",
            "<color=#10CEEB>Человеческий разум</color> сегодня потерпел поражение!"
        );
    }

    public void RoryWinPanel()
    {
        _exitBtn.SetActive(false);
        SetPanelTexts(
            _endBattlePanel,
            "Поражение",
            "<color=#FF6666>Любимое существо</color> победила!",
            "<color=#10CEEB>Миша</color> сегодня опять играет один :("
        );
    }

    public void ShowSurrenderPanel()
    {
        if (GameManager.Instance.PlayerFaction != null)
        {
            _surrenderPanel.SetActive(true);
        }
    }
    public void CloseSurrenderPanel()
    {
        _surrenderPanel.SetActive(false);
    }
    public void DeselectButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void SetTimer(BaseUnit unit)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        TextMeshProUGUI textMeshProUGUI = _timer.GetComponentInChildren<TextMeshProUGUI>();
        textMeshProUGUI.text = "30";
        TurnManager.Instance.TimeCounter(textMeshProUGUI, unit, _cancellationTokenSource.Token).Forget();
    }
    public void FactionChoosingPanelSetActive(bool active)
    {
        _factionChoosingPanel.SetActive(active);
        if (!active)
        {
            Destroy(_factionChoosingPanel);
        }
    }

    public void SetHeroPortrets()
    {
        Sprite playerHeroPortret = Resources.Load<Sprite>($"HeroPortrets/Misha");
        Sprite enemyHeroPortret = Resources.Load<Sprite>($"HeroPortrets/Rory");

        _playerHeroPortret.GetComponentInChildren<SpriteRenderer>().sprite = playerHeroPortret;
        _playerHeroPortret.SetActive(true);

        _enemyHeroPortret.GetComponentInChildren<SpriteRenderer>().sprite = enemyHeroPortret;
        _enemyHeroPortret.SetActive(true);
    }
}
