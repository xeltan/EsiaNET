using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SignerApp.Controllers
{
    public class VerifyRequest
    {
        public string Alg { get; set; }

        public string Message { get; set; }

        public string Signature { get; set; }
    }

    public class VerifyController : Controller
    {
        public ActionResult Index(VerifyRequest request)
        {
            // This is not work. I suggest to turn off VerifyTokenSignature
            if ( request.Alg.ToUpperInvariant() == "GOST3410_2012_256" )
            {
                var message = Convert.FromBase64String(request.Message);
                var signature = Convert.FromBase64String(request.Signature);
                var contentInfo = new ContentInfo(new Oid("1.2.643.7.1.1.1.1"), message);
                var signedCms = new SignedCms(contentInfo, true);

                try
                {
                    signedCms.Decode(signature);
                    signedCms.CheckSignature(true);
                }
                catch ( CryptographicException e )
                {
                    return Content("false");
                }

                return Content("true");
            }

            return Content("false");
        }
    }
}