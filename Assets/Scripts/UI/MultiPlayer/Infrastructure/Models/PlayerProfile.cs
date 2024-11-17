using System;

public class PlayerProfile
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public DateTime LastLogin { get; set; }
    public int AvatarId { get; set; }
    public int FrameId { get; set; }
    public int Games { get; set; }
    public int Wins { get; set; }
    public int Loses { get; set; }
    public int Mmr { get; set; }
    public int Rank { get; set; }
    public bool Premium { get; set; }
    public int Gold { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
}
