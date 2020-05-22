using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 壁纸发布页面
    /// </summary>
    public sealed partial class WallpaperPublishPage : Page
    {
        private WallpaperTypeListViewModel WallpaperTypeLVM { get; set; }
        private ImageViewModel ImageVM { get; set; }
        private string ImageCloudPath { get; set; }
        private int UserId { get; set; }
        private bool StoryGridVisible { get; set; }

        public WallpaperPublishPage()
        {
            WallpaperTypeLVM = new WallpaperTypeListViewModel();
            ImageVM = new ImageViewModel();
            this.InitializeComponent();
        }

        /// <summary>
        /// 点击上传图片按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UploadPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            StorageFile file = await picker.PickSingleFileAsync();//选择图片
            var fileSize = file == null ? 0.0 : (await file.GetBasicPropertiesAsync()).Size;
            if (fileSize > 0 && fileSize <= 10485760)//图片大小限制
            {
                ImageVM.Image = await ImageHelper.FromFileAsync(file);
                using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                {
                    using (var stream = await file.OpenReadAsync())
                    {
                        var url = HttpClientHelper.baseUrl
                        + "upload/picture/" + UserId + "/wallpaper";
                        var form = new HttpMultipartFormDataContent();
                        var fileContent = new HttpStreamContent(stream);
                        fileContent.Headers.ContentDisposition = new HttpContentDispositionHeaderValue("form-data")
                        {
                            Name = "fileToUpload",
                            FileName = file.Name
                        };
                        form.Add(fileContent);
                        var resp = await client.PostAsync(new Uri(url), form);//上传图片
                        if (resp.IsSuccessStatusCode)
                        {
                            ImageCloudPath = await resp.Content.ReadAsStringAsync();
                            //UploadPictureButton.Visibility = Visibility.Collapsed;
                            UploadImage.Visibility = Visibility.Visible;
                            UploadErrorMsgTextBlock.Visibility = Visibility.Collapsed;
                            UploadErrorMsgTextBlock.Text = string.Empty;
                            UploadPictureButton.Content = "重新选择";
                        }
                        else
                        {
                            UploadErrorMsgTextBlock.Text = "错误信息：" + Environment.NewLine;
                            UploadErrorMsgTextBlock.Text += "· 图片上传失败" + Environment.NewLine;
                        }
                    }
                }
            }
            else
            {
                UploadErrorMsgTextBlock.Text = "错误信息：" + Environment.NewLine;
                UploadErrorMsgTextBlock.Text += "· 获取图片失败或图片大于10M" + Environment.NewLine;
            }
            if (UploadErrorMsgTextBlock.Text.Contains("·"))
            {
                UploadErrorMsgTextBlock.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 点击投稿按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var wallpaper = new T_Wallpaper();
            wallpaper.W_PublisherID = UserId;
            PublishErrorMsgTextBlock.Text = "错误信息：" + Environment.NewLine;
            //检查输入是否正确
            if (ImageCloudPath == null || ImageCloudPath == string.Empty)
            {
                PublishErrorMsgTextBlock.Text += "· 未上传图片" + Environment.NewLine;
            }
            else
            {
                wallpaper.W_Location = ImageCloudPath;
            }
            if (TitleTextBox.Text != string.Empty)
            {
                wallpaper.W_Title = TitleTextBox.Text;
            }
            else
            {
                PublishErrorMsgTextBlock.Text += "· 未输入标题" + Environment.NewLine;
            }
            if (TypeComboBox.SelectedIndex != -1)
            {
                wallpaper.W_Type = (short)TypeComboBox.SelectedValue;
            }
            else
            {
                PublishErrorMsgTextBlock.Text += "· 未选择分区" + Environment.NewLine;
            }
            if (TagTextBox.Text != string.Empty)
            {
                wallpaper.W_Tag = TagTextBox.Text;
            }
            else
            {
                PublishErrorMsgTextBlock.Text += "· 未输入标签" + Environment.NewLine;
            }
            if (StoryTextBox.Text != string.Empty)
            {
                wallpaper.W_Story = StoryTextBox.Text;
            }
            else
            {
                PublishErrorMsgTextBlock.Text += "· 未输入图语" + Environment.NewLine;
            }
            if (PublishErrorMsgTextBlock.Text.Contains("·"))
            {
                PublishErrorMsgTextBlock.Visibility = Visibility.Visible;
                return;
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "wallpaper";
                var content = new HttpStringContent(JObject.FromObject(wallpaper).ToString());
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                var resp = await client.PostAsync(new Uri(url), content);//发布壁纸
                if (resp.IsSuccessStatusCode)
                {
                    var contentDialog = new ContentDialog
                    {
                        Title = "发布成功",
                        Content = "壁纸发布成功",
                        PrimaryButtonText = "关闭"
                    };
                    contentDialog.PrimaryButtonClick += (_sender, _e) =>
                    {
                        if (MainPage.PageFrame.CanGoBack)
                        {
                            MainPage.PageFrame.GoBack();
                        }
                        contentDialog.Hide();
                    };
                    await contentDialog.ShowAsync();
                }
                else
                {
                    PublishErrorMsgTextBlock.Text += "· 发布失败" + Environment.NewLine;
                    PublishErrorMsgTextBlock.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// 点击输入完成按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarkdownInputConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //隐藏Markdown输入
            ImageStackPanel.Visibility = Visibility.Visible;
            StoryGrid.Visibility = Visibility.Collapsed;
            InputStackPanel.Visibility = Visibility.Visible;
            MarkdownGrid.Visibility = Visibility.Collapsed;
            MarkdownInputConfirmButton.Visibility = Visibility.Collapsed;
            StoryGridVisible = false;
        }

        /// <summary>
        /// 图语输入框获取焦点的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoryTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //显示Markdown输入
            StoryGridVisible = true;
            ImageStackPanel.Visibility = Visibility.Collapsed;
            StoryGrid.Visibility = Visibility.Visible;
            InputStackPanel.Visibility = Visibility.Collapsed;
            MarkdownGrid.Visibility = Visibility.Visible;
            MarkdownInputConfirmButton.Visibility = Visibility.Visible;
            var start = StoryTextBox.SelectionStart;
            var length = StoryTextBox.SelectionLength;
            HiddenStoryTextBox.Focus(FocusState.Keyboard);
            HiddenStoryTextBox.SelectionStart = start;
            HiddenStoryTextBox.SelectionLength = length;
        }

        //private void CloseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (StoryGridVisible)
        //    {
        //        ImageStackPanel.Visibility = Visibility.Visible;
        //        StoryGrid.Visibility = Visibility.Collapsed;
        //        InputStackPanel.Visibility = Visibility.Visible;
        //        MarkdownGrid.Visibility = Visibility.Collapsed;
        //        StoryGridVisible = false;
        //        return;
        //    }
        //    if (MainPage.PageFrame.CanGoBack)
        //    {
        //        MainPage.PageFrame.GoBack();
        //    }
        //}

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                UserId = (int)e.Parameter;
                StoryGridVisible = false;
                ImageStackPanel.Visibility = Visibility.Visible;
                StoryGrid.Visibility = Visibility.Collapsed;
                InputStackPanel.Visibility = Visibility.Visible;
                MarkdownGrid.Visibility = Visibility.Collapsed;
            }
            UploadErrorMsgTextBlock.Visibility = Visibility.Collapsed;
            PublishErrorMsgTextBlock.Visibility = Visibility.Collapsed;
            UploadImage.Visibility = Visibility.Collapsed;
            UploadPictureButton.Visibility = Visibility.Visible;
            UploadPictureButton.Content = "上传图片";
            await WallpaperTypeLVM.GetWallpaperTypesAsync();
            base.OnNavigatedTo(e);
        }
    }
}
