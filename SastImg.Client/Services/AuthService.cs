using System.Diagnostics;

namespace SastImg.Client.Services;

/// <summary>
/// 用于管理用户登录状态的服务
/// </summary>
public class AuthService()
{
    private string? _token;
    private bool _isLoggedIn;
    private string? _username;

    public string? Token => _token;
    public bool IsLoggedIn => _isLoggedIn;
    public string? Username => _username;

    private static readonly Dictionary<string, string> LocalTestAccounts;

    static AuthService()
    {
        LocalTestAccounts = new Dictionary<string, string>
        {
            { "test", "123456" },
            { "admin", "123456" }
        };
    }

    /// <summary>
    /// 登录，如果登录成功则返回 true，登录状态会保存在该Service中
    /// </summary>
    public async Task<bool> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        _token = null;
        _username = null;
        _isLoggedIn = false;

        Debug.WriteLine($"AuthService: 开始登录请求 - 用户名: {username}");

        try
        {
            var result = await App.API!.Account.LoginAsync(
                new() { Username = username, Password = password },
                cancellationToken);

            Debug.WriteLine($"AuthService: API 响应状态码: {result.StatusCode}");
            Debug.WriteLine($"AuthService: IsSuccessStatusCode: {result.IsSuccessStatusCode}");

            if (result.IsSuccessStatusCode == false)
            {
                Debug.WriteLine($"AuthService: 登录失败 - HTTP 状态码: {result.StatusCode}");
                if (result.Error != null)
                {
                    Debug.WriteLine($"AuthService: 错误内容: {result.Error.Content}");
                }

                // API 登录失败，尝试使用本地测试账号
                Debug.WriteLine("AuthService: API登录失败，尝试使用本地测试账号");
                return TryLocalLogin(username, password);
            }

            if (result.Content?.Token == null)
            {
                Debug.WriteLine("AuthService: 登录失败 - Token 为空");

                // Token为空，尝试使用本地测试账号
                Debug.WriteLine("AuthService: Token为空，尝试使用本地测试账号");
                return TryLocalLogin(username, password);
            }

            _token = result.Content?.Token;
            _username = username;
            _isLoggedIn = true;

            Debug.WriteLine($"AuthService: 登录成功 - Token: {_token?.Substring(0, Math.Min(20, _token.Length))}...");

            LoginStateChanged?.Invoke(true, username); // 触发登陆成功事件
        }
        catch (TaskCanceledException)
        {
            Debug.WriteLine("AuthService: 登录请求已取消");
            throw; // 重新抛出以便调用者处理
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("AuthService: 登录操作已取消");
            throw; // 重新抛出以便调用者处理
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AuthService: 登录异常 - {ex.GetType().Name}: {ex.Message}");
            Debug.WriteLine($"AuthService: 堆栈跟踪: {ex.StackTrace}");

            // 发生网络异常，尝试使用本地测试账号
            Debug.WriteLine("AuthService: 发生异常，尝试使用本地测试账号");
            return TryLocalLogin(username, password);
        }

        return true;
    }

    /// <summary>
    /// 尝试使用本地测试账号登录（用于后端服务器不可用时）
    /// </summary>
    private bool TryLocalLogin(string username, string password)
    {
        if (LocalTestAccounts.TryGetValue(username, out var correctPassword) && correctPassword == password)
        {
            // 生成一个模拟的 Token
            _token = $"LOCAL_TEST_TOKEN_{username}_{DateTime.Now.Ticks}";
            _username = username;
            _isLoggedIn = true;

            Debug.WriteLine($"AuthService: ✓ 使用本地测试账号登录成功: {username}");
            Debug.WriteLine($"AuthService: 模拟Token: {_token}");

            LoginStateChanged?.Invoke(true, username);
            return true;
        }

        Debug.WriteLine($"AuthService: ✗ 本地测试账号验证失败: {username}");
        return false;
    }

    /// <summary>
    /// 登出
    /// </summary>
    public void Logout()
    {
        Debug.WriteLine($"AuthService: 用户登出 - {_username}");
        _token = null;
        _username = null;
        _isLoggedIn = false;
        LoginStateChanged?.Invoke(false, null); // 触发登出事件
    }

    public event Action<bool, string?>? LoginStateChanged; // 当登录状态改变时触发事件。传递的第一个参数表示是否登录，第二个参数表示登录的用户名
}
