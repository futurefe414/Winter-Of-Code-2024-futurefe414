using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SastImg.Client.Service.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SastImg.Client.Views
{
    public partial class TestViewModel : ObservableObject
    {
        public ObservableCollection<ImageDto> Images { get; } = [];

        [ObservableProperty]
        private ImageDto selectedImage;

        [ObservableProperty]
        private string testUsername = "test";

        [ObservableProperty]
        private string testPassword = "123456";

        [ObservableProperty]
        private string loginResult = "";

        [RelayCommand]
        public async Task LoadImagesAsync()
        {
            await GetAllImagesAsync();
        }

        [RelayCommand]
        public async Task TestLoginAsync()
        {
            Debug.WriteLine($"[TestViewModel] 测试登录: {TestUsername}");
            LoginResult = "正在登录...";
            
            try
            {
                bool success = await App.AuthService.LoginAsync(TestUsername, TestPassword);
                
                if (success)
                {
                    LoginResult = $"✓ 登录成功！用户: {TestUsername}\nToken: {App.AuthService.Token?.Substring(0, Math.Min(30, App.AuthService.Token.Length))}...";
                    Debug.WriteLine($"[TestViewModel] 登录成功");
                }
                else
                {
                    LoginResult = $"✗ 登录失败！用户: {TestUsername}\n请检查输出窗口查看详细错误信息";
                    Debug.WriteLine($"[TestViewModel] 登录失败");
                }
            }
            catch (Exception ex)
            {
                LoginResult = $"✗ 登录异常: {ex.Message}";
                Debug.WriteLine($"[TestViewModel] 登录异常: {ex}");
            }
        }

        [RelayCommand]
        public async Task TestAdminLoginAsync()
        {
            TestUsername = "admin";
            TestPassword = "123456";
            await TestLoginAsync();
        }

        [RelayCommand]
        public async Task TestUserLoginAsync()
        {
            TestUsername = "test";
            TestPassword = "123456";
            await TestLoginAsync();
        }

        public async Task<bool> GetAllImagesAsync()
        {
            Images.Clear();
            var imagesRequest = await App.API!.Image.GetImagesAsync(null, null, null);
            if (!imagesRequest.IsSuccessful) return false; // 如果获取失败，返回 false

            foreach (var image in imagesRequest.Content)
            {
                Images.Add(image);
            }
            return true;
        }
    }
}

