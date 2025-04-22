namespace Assets.Scripts.Infrastructure.Requests.PlayersService
{
    public class UpdateFrameRequest
    {
        public int FrameId { get; set; }

        public UpdateFrameRequest(int frameId)
        {
            FrameId = frameId;
        }
    }
}
