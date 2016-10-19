using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EsiaNET;

namespace ESIA.AspNetIdentityExample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var esiaToken = Session["esiaToken"];

            // Если есть маркер доступа, то переадресуем на страницу данных ЕСИА
            if ( esiaToken != null )
            {
                return RedirectToAction("EsiaPage");
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // Получаем данные из ЕСИА
        public async Task<ActionResult> EsiaPage()
        {
            var esiaToken = Session["esiaToken"];

            if ( esiaToken != null )
            {
                // Создаем ЕСИА клиента с маркером доступа для получения данных
                var esiaClient = new EsiaClient(Esia.GetOptions(), (EsiaToken)esiaToken);

                // ВАЖНО! Указанная в параметрах ЕСИА (Esia.GetOptions()) область доступа может не предоставить необходимые данные. В данном примере используется полный доступ
                // Получим данные о пользователе
                var personInfo = await esiaClient.GetPersonInfoAsync();

                // Пользователь не подтвержден - выводим ошибку
                if ( !personInfo.Trusted ) return View("Error");

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

                // Отобразим данные на форме
                ViewBag.Person = personInfo;
                ViewBag.Contacts = contacts;
                ViewBag.Docs = docs;
                ViewBag.Addrs = addrs;
                ViewBag.Vehicles = vehicles;
                ViewBag.Kids = kids;
            }
            else return View("Error");

            return View();
        }
    }
}