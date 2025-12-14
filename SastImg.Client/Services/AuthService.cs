using System;
using System.Threading;
using System.Threading.Tasks;

namespace SastImg.Client.Services;

/// <summary>
/// 用于管理用户登录状态的服务
/// </summary>
public class AuthService ( )
{
    private string? _token;
    private bool _isLoggedIn;
    private string? _username;

    public string? Token => _token;
    public bool IsLoggedIn => _isLoggedIn;
    public string? Username => _username;

    /// <summary>
    /// 登录，如果登录成功则返回 true，登录状态会保存在该Service中
    /// </summary>
    public async Task<bool> LoginAsync (string username, string password, CancellationToken cancellationToken = default)
    {
        _token = null;
        _username = null;
        _isLoggedIn = false;

        try
        {
            System.Diagnostics.Debug.WriteLine($"AuthService: 开始登录请求 - 用户名: {username}");
            
            var result = await App.API!.Account.LoginAsync(new() { Username = username, Password = password }, cancellationToken);

            System.Diagnostics.Debug.WriteLine($"AuthService: API 响应状态码: {result.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"AuthService: IsSuccessStatusCode: {result.IsSuccessStatusCode}");

            if ( result.IsSuccessStatusCode == false )
            {
                System.Diagnostics.Debug.WriteLine($"AuthService: 登录失败 - HTTP 状态码: {result.StatusCode}");
                if (result.Error != null)
                {
                    System.Diagnostics.Debug.WriteLine($"AuthService: 错误内容: {result.Error.Content}");
                }
                return false;
            }
            
            if ( result.Content?.Token == null )
            {
                System.Diagnostics.Debug.WriteLine("AuthService: 登录失败 - Token 为空");
                return false;
            }

            _token = result.Content?.Token;
            _username = username;
            _isLoggedIn = true;
            
            System.Diagnostics.Debug.WriteLine($"AuthService: 登录成功 - Token: {_token?.Substring(0, Math.Min(20, _token.Length))}...");
            
            LoginStateChanged?.Invoke(true, username); // 触发登陆成功事件
        }
        catch (TaskCanceledException)
        {
            System.Diagnostics.Debug.WriteLine("AuthService: 登录请求已取消");
            throw; // 重新抛出以便调用者处理
        }
        catch (OperationCanceledException)
        {
            System.Diagnostics.Debug.WriteLine("AuthService: 登录操作已取消");
            throw; // 重新抛出以便调用者处理
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AuthService: 登录异常 - {ex.GetType().Name}: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"AuthService: 堆栈跟踪: {ex.StackTrace}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 登出
    /// </summary>
    public void Logout ( )
    {
        _token = null;
        _isLoggedIn = false;
        _isLoggedIn = false;
        LoginStateChanged?.Invoke(false, null); // 触发登出事件
    }

    public event Action<bool,string?>? LoginStateChanged; // 当登录状态改变时触发事件。传递的第一个参数表示是否登录，第二个参数表示登录的用户名
}
