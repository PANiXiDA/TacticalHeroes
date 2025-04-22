namespace Assets.Scripts.Infrastructure.Responses.AvatarsService
{
    public class GetAvailableAvatarsResponse
    {
        public int Id { get; set; }
        public string S3Path { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NecessaryMmr { get; set; }
        public int NecessaryGames { get; set; }
        public int NecessaryWins { get; set; }
        public bool Available { get; set; }
        public string FileName { get; set; }
    }
}
