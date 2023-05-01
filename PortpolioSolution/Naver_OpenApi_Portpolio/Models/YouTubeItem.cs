using System.Windows.Media.Imaging;

namespace Naver_OpenApi_Portpolio.Models
{
    internal class YoutubeItem
    {
        public string Title { get; set; }
        public string URL { get; set; }
        public string ChannelTitle { get; set; }
        public BitmapImage Thumbnail { get; set; }
    }
}
