using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SignerApp.Controllers
{
    public class SignController : Controller
    {
        public ActionResult Index(string msg)
        {
            byte[] bytes = Convert.FromBase64String(msg);
            var contentInfo = new ContentInfo(bytes);
            var signedCms = new SignedCms(contentInfo, true);
            var storeMy = new X509Store(StoreName.My, StoreLocation.LocalMachine);

            storeMy.Open(OpenFlags.OpenExistingOnly);

            var certColl = storeMy.Certificates.Find(X509FindType.FindBySerialNumber,
                "<serial number>", false);

            storeMy.Close();

            var cmsSigner = new CmsSigner(certColl[0]);

            try
            {
                signedCms.ComputeSignature(cmsSigner);
            }
            catch ( Exception e )
            {
                throw;
            }

            var signature = signedCms.Encode();

            return Content(Convert.ToBase64String(signature));
        }
    }
}