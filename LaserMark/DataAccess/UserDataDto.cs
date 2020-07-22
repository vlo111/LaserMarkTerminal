namespace LaserMark.DataAccess
{
    public class UserDataDto
    {
        public long Id { get; set; }

        public long Sequence { get; set; }

        public string Token { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Url { get; set; }

        public string BgImage { get; set; }

        public string EzdImage { get; set; }

        public string FullImage { get; set; }

        public long BgImagePosX { get; set; }

        public long BgImagePosY { get; set; }

        public long EzdImagePosX { get; set; }

        public long EzdImagePosY { get; set; }
    }
}
