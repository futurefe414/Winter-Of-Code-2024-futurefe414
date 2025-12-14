using System;
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
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.WriteAsString,
        };
        jsonSerializerOptions.Converters.Add(new Int32Converter());
        jsonSerializerOptions.Converters.Add(new Int64Converter());

        var refitSettings = new RefitSettings
        {
            AuthorizationHeaderValueGetter = (_, _) => Task.FromResult(App.AuthService.Token ?? ""),
            ContentSerializer = new SystemTextJsonContentSerializer(jsonSerializerOptions),
        };

        Account = RestService.For<IAccountApi>(endpointUrl, refitSettings);
        Image = RestService.For<IImageApi>(endpointUrl, refitSettings);
        Album = RestService.For<IAlbumApi>(endpointUrl, refitSettings);
        Category = RestService.For<ICategoryApi>(endpointUrl, refitSettings);
        Tag = RestService.For<ITagApi>(endpointUrl, refitSettings);
        User = RestService.For<IUserApi>(endpointUrl, refitSettings);
    }
}
