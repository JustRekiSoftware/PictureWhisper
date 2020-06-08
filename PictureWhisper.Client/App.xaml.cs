using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Domain.Concrete;
using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Entites;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PictureWhisper.Client
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            try
            {
                using (var db = new LocalDBContext())
                {
                    db.DbPath = SQLiteHelper.DbPath;
                    db.Database.Migrate();//当数据库不存在时创建数据库
                }
            }
            catch (NotSupportedException)
            {
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;//关闭标题栏的返回按钮
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                    HttpClientHelper.StartUpdateAccessTokenTask();//启动AccessToken自动更新

                    var result = SQLiteHelper.GetSigninInfo();//获取已登录用户
                    if (result == null || result.SI_Status != (short)Status.正常)
                    {
                        rootFrame.Navigate(typeof(SigninPage), e.Arguments);//没有已登录用户或用户状态错误则要求重新登录
                    }
                    else//自动登录
                    {
                        using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                        {
                            //登录
                            var url = HttpClientHelper.baseUrl + "user/signin/"
                                + result.SI_Email + "/" + result.SI_Password;
                            var resp = await client.GetAsync(new Uri(url));
                            if (resp.IsSuccessStatusCode)//登录成功
                            {
                                var userSigninDto = JObject.Parse(await resp.Content.ReadAsStringAsync())
                                    .ToObject<UserSigninDto>();
                                result.SI_Avatar = userSigninDto.U_Avatar;
                                result.SI_Type = userSigninDto.U_Type;
                                if (userSigninDto.U_Status != (short)Status.正常)//用户状态不正常，则要求重新登录
                                {
                                    result.SI_Status = userSigninDto.U_Status;
                                    rootFrame.Navigate(typeof(SigninPage), false);
                                }
                                if (result.SI_Type == (short)UserType.注册用户)
                                {
                                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                                }
                                else if (result.SI_Type == (short)UserType.管理员)//管理员跳转到管理员主页面
                                {
                                    rootFrame.Navigate(typeof(AdminMainPage), e.Arguments);
                                }
                                else//壁纸审核人员和举报处理人员跳转到审核主页面
                                {
                                    rootFrame.Navigate(typeof(ReviewMainPage), e.Arguments);
                                }
                            }
                            else//登录失败
                            {
                                rootFrame.Navigate(typeof(SigninPage), false);
                            }
                            await SQLiteHelper.UpdateSigninInfoAsync(result);//更新用户登录信息
                        }
                    }
                    var settingInfo = SQLiteHelper.GetSettingInfo();
                    if (settingInfo == null)//当设置信息为空时，添加默认设置信息
                    {
                        settingInfo = new T_SettingInfo();
                        settingInfo.STI_AutoSetWallpaper = false;
                        settingInfo.STI_LastCheckMessageDate = DateTime.Now;
                        await SQLiteHelper.AddSettingInfoAsync(settingInfo);
                    }
                }
                // Ensure the current window is active
                Window.Current.Activate();
                Window.Current.Closed += async (sender, args) =>
                {
                    if (NotifyHelper.connected)
                    {
                        await NotifyHelper.SignOutAsync();//关闭应用并且消息提示已连接时，向服务端发送注销请求
                    }
                    if (ReviewHelper.connected)
                    {
                        await ReviewHelper.SignOutAsync();//关闭应用并且审核处理已连接时，向服务端发送注销请求
                    }
                };
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
