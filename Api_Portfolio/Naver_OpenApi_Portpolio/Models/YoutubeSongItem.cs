using System;

namespace Naver_OpenApi_Portpolio.Models
{
    internal class YoutubeSongItem
    {
        public string Brand { get; set; } // 노래방 상호
        public int No { get; set; } // 노래번호
        public string Title { get; set; } // 노래제목
        public string Singer { get; set; } // 가수이름
        public string Composer { get; set; } // 작곡가
        public string Lyricist { get; set; } // 작사가
        public DateTime Release { get; set; } // 업데이트시간
    }
}
