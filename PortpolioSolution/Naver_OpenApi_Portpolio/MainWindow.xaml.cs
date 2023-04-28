using MahApps.Metro.Controls;
using MySqlX.XDevAPI.Relational;
using Naver_OpenApi_Portpolio.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Naver_OpenApi_Portpolio
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool isFavorite = false; // false -> openApi 검색해온결과, true -> 즐겨찾기 보기

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnYouTube_Click(object sender, RoutedEventArgs e)
        {
            await Commons.ShowMessageAsync("목록선택", "해당 사이트로 이동합니다.");
        }

        private async void BtnSearchYouTube_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtYouTubeName.Text))
            {
                await Commons.ShowMessageAsync("검색", "검색어를 입력하세요.");
                return;
            }

            try
            {
                SearchYouTube(TxtYouTubeName.Text);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"오류 발생 : {ex.Message}");
            }

        }

        private void TxtYouTubeName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearchYouTube_Click(sender, e);
            }
        }


        // 실제 검색메서드
        private async void SearchYouTube(string YouTubeName)
        {
            string tmdb_apiKey = "내 TMDB 키";
            string encoding_YouTubeName = HttpUtility.UrlEncode(YouTubeName, Encoding.UTF8);
            string openApiUri = $"https://api.theYouTubedb.org/3/search/YouTube?api_key={tmdb_apiKey}" +
                                $"&language=ko-KR&page=1&include_adult=false&query={encoding_YouTubeName}"; // 영화 검색 URL
            string result = string.Empty; // 결과값

            // api 실행할 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // TMDB api 요청
            try
            {
                req = WebRequest.Create(openApiUri); // URL을 넣어서 객체 생성
                res = await req.GetResponseAsync(); // 요청한 결과를 응답에 할당
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd(); // json결과 텍스트로 저장

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            // result를 json으로 변경
            var jsonResult = JObject.Parse(result); // string -> json

            var total = Convert.ToInt32(jsonResult["total_results"]); // 전체 검색결과 수
            //await Commons.ShowMessageAsync("검색결과", total.ToString());
            var items = jsonResult["results"];
            // items를 데이터그리드에 표시
            var json_array = items as JArray;

            var YouTubeItems = new List<YouTubeItem>(); // json에서 넘어온 배열을 담을 장소
            foreach (var val in json_array)
            {
                var YouTubeItem = new YouTubeItem()
                {
                    Adult = Convert.ToBoolean(val["adult"]),
                    Id = Convert.ToInt32(val["id"]),
                    Original_Language = Convert.ToString(val["original_language"]),
                    Original_Title = Convert.ToString(val["original_title"]),
                    OverView = Convert.ToString(val["overview"]),
                    Popularity = Convert.ToDouble(val["popularity"]),
                    Poster_Path = Convert.ToString(val["poster_path"]),
                    Release_Date = Convert.ToString(val["release_date"]),
                    Title = Convert.ToString(val["title"]),
                    Vote_Average = Convert.ToDouble(val["vote_average"])
                };
                YouTubeItems.Add(YouTubeItem);
            }

            this.DataContext = YouTubeItems;
            isFavorite = false; // 즐겨찾기가 아님.
            StsResult.Content = $"OpenAPI 조회 {YouTubeItems.Count} 건 조회 완료";
        }
    }
}
