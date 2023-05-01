using MahApps.Metro.Controls;
using Naver_OpenApi_Portpolio.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Data;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;

namespace Naver_OpenApi_Portpolio
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool isFavorite = false; // false -> openApi 검색해온결과, true -> 즐겨찾기 보기
        private object array;

        public MainWindow()
        {
            InitializeComponent();
        }

        // 노래검색
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
        // 엔터로 검색
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
            string openApiUri = $"https://api.manana.kr/karaoke.json"; // 노래방 검색 URL
            string result = string.Empty; // 결과값

            // api 실행할 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // 노래방 api 요청
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
            var jsonResult = JArray.Parse(result); // string -> json

            var youtubeItems = new List<YSI>(); // json에서 넘어온 배열을 담을 장소
            foreach (var val in jsonResult)
            {
                var youtubeItem = new YSI()
                {
                    Brand = Convert.ToString(val["brand"]),
                    No = Convert.ToInt32(val["no"]),
                    Title = Convert.ToString(val["title"]),
                    Singer = Convert.ToString(val["singer"]),
                    Composer = Convert.ToString(val["composer"]),
                    Lyricist = Convert.ToString(val["lyricist"]),
                    Release = Convert.ToDateTime(val["release"])
                };
                youtubeItems.Add(youtubeItem);
            }

            this.DataContext = youtubeItems;
            isFavorite = false; // 즐겨찾기가 아님.
            StsResult.Content = $"노래방 OpenAPI 조회 {youtubeItems.Count} 건 조회 완료";
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtYouTubeName.Focus(); // 텍스트박스에 포커스 셋
        }

        #region < 이벤트 메서드 >

        // 장르별검색 (미완)
        private async void BtnGenreSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtYouTubeName.Text))
            {
                await Commons.ShowMessageAsync("검색", "검색어를 입력하세요.");
                return;
            }

            try
            {
                SearchYouTube(TxtYouTubeName.Text);
                //object s = array.sort();
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"오류 발생 : {ex.Message}");
            }
        }

        // 국가별검색 (미완)
        private async void BtnFromSearch_Click(object sender, RoutedEventArgs e)
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

        // 랜덤곡추천 (미완)
        private async void BtnRandomSong_Click(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(TxtYouTubeName.Text))
            //{
            //    await Commons.ShowMessageAsync("검색", "검색어를 입력하세요.");
            //    return;
            //}

            //try
            //{
            //    SearchYouTube(TxtYouTubeName.Text);
            //}
            //catch (Exception ex)
            //{
            //    await Commons.ShowMessageAsync("오류", $"오류 발생 : {ex.Message}");
            //}
        }

        // 즐겨찾기 목록
        private async void BtnBestSong_Click(object sender, RoutedEventArgs e)
        {
            List<YoutubeSongItem> list = new List<YoutubeSongItem>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = @"SELECT Brand
                                       , No
                                       , Title
                                       , Singer
                                       , Composer
                                       , Lyricist
                                    FROM YoutubeSongItem
                                   ORDER BY Id ASC";
                    var insRes = 0;
                    foreach (YoutubeSongItem item in list)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Brand", item.Brand);
                        cmd.Parameters.AddWithValue("@No", item.No);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Singer", item.Singer);
                        cmd.Parameters.AddWithValue("@Composer", item.Composer);
                        cmd.Parameters.AddWithValue("@Lyricist", item.Lyricist);

                        insRes += cmd.ExecuteNonQuery();
                    }
                
                    this.DataContext = list;
                    isFavorite = true;
                    StsResult.Content = $"즐겨찾기 {list.Count} 건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB조회 오류{ex.Message}");
            }
        }

        // 즐겨찾기 추가
        private async void BtnAddBestSong_Click(object sender, RoutedEventArgs e)
        {
            List<YoutubeSongItem> list = new List<YoutubeSongItem>();

            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 노래를 선택하세요(복수선택 가능)");
                return;
            }

            if (isFavorite)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 노래입니다.");
                return;
            }

            #region < MySQL 연결 >
            try
            {
                // DB 연결확인
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = @"INSERT INTO youtubesongitem
                                                (Brand,
                                                No,
                                                Title,
                                                Singer,
                                                Composer,
                                                Lyricist)
                                                VALUES
                                                (@Brand,
                                                @No,
                                                @Title,
                                                @Singer,
                                                @Composer,
                                                @Lyricist,
                                                @Release)";

                    var insRes = 0;
                    foreach (YoutubeSongItem item in list)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Brand", item.Brand);
                        cmd.Parameters.AddWithValue("@No", item.No);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Singer", item.Singer);
                        cmd.Parameters.AddWithValue("@Composer", item.Composer);
                        cmd.Parameters.AddWithValue("@Lyricist", item.Lyricist);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    if (list.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장성공");
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장오류 관리자에게 문의하세요.");
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장 오류{ex.Message}");
            }
            #endregion
        }

        // 즐겨찾기 삭제
        private async void BtnDelBestSong_Click(object sender, RoutedEventArgs e)
        {
            if (isFavorite == false)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기만 삭제할 수 있습니다.");
                return;
            }
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "삭제할 노래를 선택하세요.");
                return;
            }

            try // 삭제
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = "DELETE FROM YoutubeItem WHERE Brand = @Brand";
                    var delRes = 0;

                    foreach (YoutubeSongItem item in GrdResult.SelectedItems)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Brand", item.Brand);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제성공");
                        StsResult.Content = $"즐겨찾기 {delRes} 건 삭제완료";
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제 일부성공"); // 발생할일이 거의 없음

                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB삭제 오류{ex.Message}");
            }

            BtnBestSong_Click(sender, e); // 즐겨찾기 보기 이벤트핸들러를 한번 실행
        }


        #endregion

    }
}
