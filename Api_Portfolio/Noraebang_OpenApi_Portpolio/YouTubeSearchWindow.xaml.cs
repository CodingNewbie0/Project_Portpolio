using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MahApps.Metro.Controls;
using Naver_OpenApi_Portpolio.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace Naver_OpenApi_Portpolio
{
    /// <summary>
    /// YouTubeSearchWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class YouTubeSearchWindow : MetroWindow
    {
        List<YoutubeItem> youtubeItems = null; // 검색결과 담을 리스트

        public YouTubeSearchWindow()
        {
            InitializeComponent();
        }

        // 부모에서 데이터를 가져올려면
        public YouTubeSearchWindow(string searchName) : this()
        {
            LblSearchName.Content = $"{searchName} 정보";
        }

        //부모에서 객체를 통채로 전달받기가능
        private YouTubeSearchWindow(YoutubeSongItem search) : this()
        {
            LblSearchName.Content = $"{search.Title} 정보";
        }

        // 화면 로드 완료후에 YoutubeAPI 실행
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            youtubeItems = new List<YoutubeItem>(); // 초기화
            SearchYoutubeApi();
        }

        private async void SearchYoutubeApi()
        {
            await LoadDataCollection();
        }

        private async Task LoadDataCollection()
        {
            var youtubeService = new YouTubeService(
                new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyBEf58abDrviKCIEy7nOE6Aujjwt9MC9QM",
                    ApplicationName = this.GetType().ToString()
                });

            var req = youtubeService.Search.List("snippet");
            req.Q = LblSearchName.Content.ToString();
            req.MaxResults = 10;

            var res = await req.ExecuteAsync(); // 검색결과를 받아옴

            Debug.WriteLine("----------------유튜브검색결과----------------");
            foreach (var item in res.Items)
            {
                Debug.WriteLine(item.Snippet.Title);
                if (item.Id.Kind.Equals("youtube#video"))
                {
                    YoutubeItem youtube = new YoutubeItem()
                    {
                        Title = item.Snippet.Title,
                        ChannelTitle = item.Snippet.ChannelTitle,
                        URL = $"https://www.youtube.com/watch?v={item.Id.VideoId}" // 유튜브플레이 링크
                    };

                    youtube.Thumbnail = new BitmapImage(new Uri(item.Snippet.Thumbnails.Default__.Url,
                                                        UriKind.RelativeOrAbsolute));
                    youtubeItems.Add(youtube);
                }
            }
        }

        private void LsvResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LsvResult.SelectedItem is YoutubeItem)
            {
                var video = LsvResult.SelectedItem as YoutubeItem;
                BrsYoutube.Address = video.URL;
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BrsYoutube.Address = string.Empty; // 웹브라우저 주소 클리어
            BrsYoutube.Dispose();
        }
    }
}
