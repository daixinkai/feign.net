using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Feign.TestsUWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Feign.Standalone.FeignClients.AddFeignClients(options =>
            {
                options.FeignClientPipeline.SendingRequest += FeignClientPipeline_SendingRequest;
                options.FeignClientPipeline.ErrorRequest += FeignClientPipeline_ErrorRequest;
            });
            _imageTestService = Feign.Standalone.FeignClients.Get<IImageTestService>();
        }

        private void FeignClientPipeline_ErrorRequest(object sender, Feign.IErrorRequestEventArgs<object> e)
        {

        }

        private void FeignClientPipeline_SendingRequest(object sender, Feign.ISendingRequestEventArgs<object> e)
        {

        }

        IImageTestService _imageTestService;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //using (HttpClient httpClient = new HttpClient())
            //{
            //    //var response = httpClient.GetAsync("http://www.baidu.com").ConfigureAwait(false).GetAwaiter().GetResult();
            //    var r = Task.Run(() => httpClient.GetAsync("http://www.baidu.com").Result).Result;
            //}
            //string html = await _testService.GetIndex();
            //this.textBox.Text = html;
        }

        //private async void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    //string html = await _testService.GetIndex();
        //    Stream stream = await _imageTestService.GetImage("feed/42a98226cffc1e1739cc574c5bd0b905738de927.png?token=e8ff4505d58a0ed2123758b747417a08&s=7750E5338FE24D221CE090DA030050B0");
        //    BitmapImage bi = this.image1.Source as BitmapImage ?? new BitmapImage();
        //    await bi.SetSourceAsync(stream.AsRandomAccessStream());
        //    this.image1.Source = bi;
        //}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //string html = await _testService.GetIndex();
            Stream stream = _imageTestService.GetImage("feed/42a98226cffc1e1739cc574c5bd0b905738de927.png?token=e8ff4505d58a0ed2123758b747417a08&s=7750E5338FE24D221CE090DA030050B0").Result;
            BitmapImage bi = this.image1.Source as BitmapImage ?? new BitmapImage();
            bi.SetSource(stream.AsRandomAccessStream());
            this.image1.Source = bi;
        }

    }
}
