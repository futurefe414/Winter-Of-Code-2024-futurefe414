using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using SastImg.Client.Service.API;
using SastImg.Client.Helpers;

namespace SastImg.Client.Services;

/// <summary>
/// SAST Image的API。执行所有操作都可以通过这个类来进行。
/// 包含多个属性，每个属性对应一组API。
/// </summary>
public class SastImgAPI
{
    public IAccountApi Account { get; private set; }
    public IImageApi Image { get; private set; }
    public IAlbumApi Album { get; private set; }
    public ICategoryApi Category { get; private set; }
    public ITagApi Tag { get; private set; }
    public IUserApi User { get; private set; }

    public SastImgAPI (string endpointUrl)
    {
        Debug.WriteLine($"[SastImgAPI] 初始化API客户端，端点: {endpointUrl}");

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.WriteAsString,
            // 添加UTF-8编码支持
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        jsonSerializerOptions.Converters.Add(new Int32Converter());
        jsonSerializerOptions.Converters.Add(new Int64Converter());

        var refitSettings = new RefitSettings
        {
            AuthorizationHeaderValueGetter = (_, _) => Task.FromResult(App.AuthService.Token ?? ""),
            ContentSerializer = new SystemTextJsonContentSerializer(jsonSerializerOptions),
        };

        // 使用正确的端口5265
        var apiBaseUrl = "http://sastwoc2024.shirasagi.space:5265/";
        
        Debug.WriteLine($"[SastImgAPI] 创建API接口，基础URL: {apiBaseUrl}");
        
        Account = RestService.For<IAccountApi>(apiBaseUrl, refitSettings);
        Image = RestService.For<IImageApi>(apiBaseUrl, refitSettings);
        Album = RestService.For<IAlbumApi>(apiBaseUrl, refitSettings);
        Category = RestService.For<ICategoryApi>(apiBaseUrl, refitSettings);
        Tag = RestService.For<ITagApi>(apiBaseUrl, refitSettings);
        User = RestService.For<IUserApi>(apiBaseUrl, refitSettings);
        
        Debug.WriteLine("[SastImgAPI] API客户端初始化完成");
    }
}
