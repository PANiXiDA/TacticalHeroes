namespace Assets.Scripts.Infrastructure.Models
{
    public class Avatar
    {
        public int Id { get; set; }
        public string S3Path { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }

        public Avatar(int id, string s3Path, string name, string description, string fileName)
        {
            Id = id;
            S3Path = s3Path;
            Name = name;
            Description = description;
            FileName = fileName;
        }
    }
}
