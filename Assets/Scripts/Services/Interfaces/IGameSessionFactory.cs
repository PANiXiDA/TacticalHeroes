using Assets.Scripts.Infrastructure.Models;

namespace Assets.Scripts.Services.Interfaces
{
    public interface IGameSessionsFactory
    {
        GameSession CreateDefault();
    }
}
