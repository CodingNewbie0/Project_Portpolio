using System;

namespace Naver_OpenApi_Portpolio.Models
{
    internal class YouTubeItem
    {
        public bool Adult { get; set; }
        public int Id { get; set; }
        public string Original_Language { get; set; }
        public string Title { get; set; }
        public string Original_Title { get; set; }
        public string OverView { get; set; }
        public string Release_Date { get; set; }
        public double Vote_Average { get; set; }
        public double Popularity { get; set; }
        public string Poster_Path { get; set; }
        public DateTime Reg_Date { get; set; }
    }
}
