# Esia.NET
.NET библиотека для подключения к ЕСИА по протоколу OAuth 2.0 и OpenId Connect 1.0, сервисы REST.

Информация об ЕСИА на сайте Минкомсвязь России: [Единая система идентификации и аутентификации (ЕСИА)](http://minsvyaz.ru/ru/activity/directions/13/#section-description)

ESIA OAuth 2.0 client to russian government services for .NET framework.

## Documentation
Здесь представлена документация по Esia.NET для .NET Core 3.1 и выше. Nuget-пакеты версии 1.0 и выше.

Для классического .NET Framework документация и реализация находится в ветке [dot-net-classic](https://github.com/xeltan/EsiaNET/tree/dot-net-classic). Также используйте Nuget-пакеты версии 0.1. 

#### Установка и использование
Необходимо установить Nuget-пакет EsiaNET.AspNetCore.Authentication:
```
PM> Install-Package EsiaNET.AspNetCore.Authentication
```
Версия пакета 6.0.0 поддерживает .NET Core 3.1, .NET 5 и .NET 6.

Далее добавить ЕСИА провайдер в Startup.ConfigureServices.
Это внешний провайдер авторизации через ЕСИА в ASP.NET Core, подобно авторизации через Google, Facebook, Twitter.
 При добавлении требуется указать параметры подключения к ЕСИА и сведения о системе-клиенте. Многие параметры имеют значения по умолчанию.
```C#
services.AddAuthentication()
        .AddEsia(options =>
        {
            options.ClientId = "YOUR_CLIENT_SYSTEM_ID";   // Идентификатор вашей систему. Обязателен
            options.AuthorizationEndpoint = EsiaConsts.EsiaAuthUrl;   // Адрес перенаправления на страницу предоставления прав доступа в ЕСИА - либо тестовый, либо рабочий. По умолчанию - рабочий https://esia.gosuslugi.ru/aas/oauth2/ac
            options.TokenEndpoint = EsiaConsts.EsiaTokenUrl;     // https-адрес ЕСИА для получения маркера доступа - либо тестовый, либо рабочий. По умолчанию - рабочий https://esia.gosuslugi.ru/aas/oauth2/te
            
            // Провайдер для подписания запроса к ЕСИА и проверки токена. Обязателен
            // Комментарии ниже
            options.SignProvider = new SignProvider();
            options.VerifyTokenSignature = false; // Не проверять подпись возвращемого токена, по умолчанию - false
            options.SaveTokens = true; // Сохранить токен в AuthenticationProperties, см. официальную документацию по AspNet Core

            // Требуемый scope. Обязателен
            options.Scope.Add("openid");
            options.Scope.Add("fullname");
            options.Scope.Add("id_doc");
            
            // Параметры доспупа к REST ЕСИА. Необязательно. По умолчанию использует параметры продуктовой среды.
            // Для тестовой поменяйте EsiaRestOptions.RestUri на EsiaConsts.EsiaRestTestUrl
            options.RestOptions = new EsiaRestOptions();

            // Event handlers
            options.Events.OnCreatingTicket = c =>
            {
                var accessToken = c.AccessToken;

                return Task.CompletedTask;
            };
        });

```
Теперь вы можете пройти процедуру аутентификации через ЕСИА (External login).
После входа, с помощью маркера доступа можно обращаться к REST ЕСИА.
```C#
var esiaClient = new EsiaClient(new EsiaRestOptions(), token);
```
Теперь вы можете получить данные из REST сервисов ЕСИА.

#### Получение данных из ЕСИА
```C#
// Получим данные о пользователе
var personInfo = await esiaClient.GetPersonInfoAsync();

// Пользователь не подтвержден - выводим ошибку
if ( !personInfo.Trusted ) throw new Exception("Данные не подтверждены");

// Получаем контакты
var contacts = await esiaClient.GetPersonContactsAsync();

// Получаем документы пользователя
var docs = await esiaClient.GetPersonDocsAsync();

// Получаем адреса пользователя
var addrs = await esiaClient.GetPersonAddrsAsync();

// Получаем транспортные средства
var vehicles = await esiaClient.GetPersonVehiclesAsync();

// Получаем детей
var kids = await esiaClient.GetPersonKidsAsync();

foreach ( var child in kids )
{
    // Получаем документы ребенка
    var childDoc = await esiaClient.GetPersonChildDocsAsync(child.Id);
}
```
Универсальный метод для получения данных из ЕСИА
```C#
async Task<HttpResponseMessage> SendAsync(HttpMethod method,
    string requestUri,
    IList<KeyValuePair<string, string>> requestParams = null,
    IList<KeyValuePair<string, string>> headers = null)
```
Данный метод позволяет выполнить http запрос на сервисы ЕСИА (requestUri) с параметрами requestParams (передаются как FormUrlEncodedContent) и дополнительными headers.

Дополнительные методы:
```C#
async Task<string> GetAsync(string requestUri) // Запрос GET, параметры в uri
async Task<string> PostAsync(string requestUri,
    IList<KeyValuePair<string, string>> requestParams = null)   // Запрос POST, параметры FormUrlEncodedContent
```
### Провайдер для подписания запроса к ЕСИА

Провайдер обязателен. Должен реализовать интерфейс ISignProvider.

Существует DefaultSignProvider, который реализует подпись и проверку через сертификат клиент-системы ЕСИА.
```C#
// Провайдер для получения сертификата системы-клиента. Обязан вернуть сертификат. В данном примере сертификат ищется на локальной машине по серийному номеру. Обязателен
    SignProvider = EsiaAuthenticationOptions.CreateSignProvider(() =>
    {
        // Будем искать сертификат в личном хранилище на локальной машине
        X509Store storeMy = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        storeMy.Open(OpenFlags.OpenExistingOnly);
        X509Certificate2Collection certColl = storeMy.Certificates.Find(X509FindType.FindBySerialNumber, "SERIAL_ID", false);

        storeMy.Close();

        return certColl[0];
    },
        // Action должен вернуть сертификат ЕСИА тестовый или рабочий. В данном примере ищем сертификат по его пути, указанном в конфигурационном файле
        () => new X509Certificate2(<путь к сертификату ЕСИА>))
```
#### ГОСТ криптография
В связи с тем что реализовать использование ГОСТ криптографии в .NET Core крайне проблематично, в примерах провайдер реализован в виде отдельного .NET Framework веб-приложения.
Вы можете использовать свою реализацию. Смотрите пример.
DefaultSignProvider в текущем виде работает только в Windows и классическом .NET Framework.

Также необходимо иметь специализированное сертифицированное ПО для работы с ГОСТ криптографией.
## Examples
### [ASP.NET Core ESIA authorization example](https://github.com/xeltan/EsiaNET/tree/master/examples)

Пример интеграции Esia.NET и ASP.NET Core для реализации авторизации через ЕСИА и получения данных.
Вставьте свои данные для работы примера. В файле EsiaNET.Mvc\Startup.cs - данные клиент-системы ЕСИА. В файле SignerApp\Controllers\SignController.cs - серийный номер вашего ГОСТ сертификата.
Как писалось рание из-за проблем ГОСТ-криптографии в .NET Core, подпись и проверка реализованы в отдельном .Net Framework приложении SignerApp. Для работы примера оба приложения должны быть запущены.
Стартовый проект - EsiaNET.Mvc.

## Support
Добро пожаловать в Issues или почту lettapost@gmail.com.
