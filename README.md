# Esia.NET
.NET библиотека для подключения к ЕСИА по протоколу OAuth 2.0 и OpenId Connect 1.0, сервисы REST.

Информация об ЕСИА на сайте Минкомсвязь России: [Единая система идентификации и аутентификации (ЕСИА)](http://minsvyaz.ru/ru/activity/directions/13/#section-description)

ESIA OAuth 2.0 client to russian government services for .NET framework.

## Documentation
Coming soon

## Examples
### [ASP.NET Identity ESIA authorization example](https://github.com/xeltan/EsiaNET/tree/master/examples/ESIA.AspNetIdentityExample)

Пример интеграции Esia.NET и ASP.NET Identity для реализации авторизации через ЕСИА и получения данных.

Приложение создано из стандартного шаблона ASP.NET MVC Visual Studio. Вся настройка ЕСИА находится в файлах App_Start/Esia.cs и App_Start/Startup.Auth.cs, получение данных - в файле Controllers/HomeController.cs. Для запуска примера необходимо установить необходимые параметры вашей системы-клиента в файле App_Start/Esia.cs - это идентификатор системы и сертификат, а также сертификат ЕСИА (тестовый или рабочий).

## Support
Добро пожаловать в Issues или почту lettapost@gmail.com.
Информация здесь наполняется и скоро будет содержать более полную картину.
