// ESIA.AspNetIdentityExample version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using EsiaNET;

namespace ESIA.AspNetIdentityExample
{
    /// <summary>
    /// Класс для создания параметров подключения к ЕСИА, используется и в ASP.NET Identity и в EsiaClient
    /// Это один из вариантов использования, вы не обязаны следовать этому примеру
    /// </summary>
    public class Esia
    {
        public static EsiaOptions GetOptions()
        {
            return new EsiaOptions
            {
                ClientId = "CLIENT_ID",    // Ваш идентификатор системы-клиента
                RedirectUri = EsiaConsts.EsiaAuthTestUrl,   // Адрес перенаправления на страницу предоставления прав доступа в ЕСИА - либо тестовый, либо рабочий
                TokenUri = EsiaConsts.EsiaTokenTestUrl,     // https-адрес ЕСИА для получения маркера доступа - либо тестовый, либо рабочий
                RestUri = EsiaConsts.EsiaRestTestUrl,       // Адрес REST-сервиса ЕСИА для получения данных - либо тестовый, либо рабочий
                Scope = "http://esia.gosuslugi.ru/usr_inf", // Область доступа, т.е. запрашиваемые права. Можно указать несколько через пробел

                // Провайдер для получения сертификата системы-клиента. Обязан вернуть сертификат. В данном примере сертификат ищется на локальной машине по серийному номеру
                SignProvider = EsiaOptions.CreateSignProvider(() =>
                {
                    // Будем искать сертификат в личном хранилище на локальной машине
                    X509Store storeMy = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                    storeMy.Open(OpenFlags.OpenExistingOnly);
                    X509Certificate2Collection certColl = storeMy.Certificates.Find(X509FindType.FindBySerialNumber, WebConfigurationManager.AppSettings["AppCertSerial"], false);

                    storeMy.Close();

                    return certColl[0];
                },
                    // Action должен вернуть сертификат ЕСИА тестовый или рабочий. В данном примере ищем сертификат по его пути, указанном в конфигурационном файле
                    () => new X509Certificate2(WebConfigurationManager.AppSettings["EsiaCertFile"]))
            };
        }
    }
}