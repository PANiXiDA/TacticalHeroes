namespace Assets.Scripts.Infrastructure.Requests.PlayersService
{
    public class UpdateAvatarRequest
    {
        public int AvatarId { get; set; }

        public UpdateAvatarRequest(int avatarId)
        {
            AvatarId = avatarId;
        }
    }
}
