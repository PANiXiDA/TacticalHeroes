using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Infrastructure.Responses.MatchmakingeService;
using UnityEngine;
using Avatar = Assets.Scripts.Infrastructure.Models.Avatar;

[CreateAssetMenu(fileName = "MatchSettings", menuName = "Scriptable Objects/MatchSettings")]
public class MatchSettings : ScriptableObject
{
    public string GameId { get; set; }
    public int OpponentId { get; set; }
    public string OpponentNickname { get; set; }
    public int Mmr { get; set; }
    public int Rank { get; set; }
    public int Level { get; set; }
    public Avatar Avatar { get; set; }
    public Frame Frame { get; set; }

    public void Initialize(MatchmakingResponse matchSettings)
    {
        GameId = matchSettings.GameId;
        OpponentId = matchSettings.OpponentId;
        OpponentNickname = matchSettings.OpponentNickname;
        Mmr = matchSettings.Mmr;
        Rank = matchSettings.Rank;
        Level = matchSettings.Level;
        Avatar = matchSettings.Avatar;
        Frame = matchSettings.Frame;
    }
}
