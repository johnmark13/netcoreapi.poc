using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Certificate")]
    public class CertificateController : Controller
    {
        [HttpGet]
        public JsonResult GetCertList()
        {
            var certs = new List<SimpleStore>();

            using (var store = new X509Store(StoreLocation.CurrentUser))
            {
                var ss = new SimpleStore(GetCertsForStore(store), "CU");
                certs.Add(ss);
            }

            certs.Add(GetStore(StoreName.My, StoreLocation.LocalMachine, "My LM"));
            certs.Add(GetStore(StoreName.My, StoreLocation.CurrentUser, "My CU"));
            certs.Add(GetStore(StoreName.CertificateAuthority, StoreLocation.LocalMachine, "CA LM"));
            certs.Add(GetStore(StoreName.CertificateAuthority, StoreLocation.CurrentUser, "CA CU"));
            certs.Add(GetStore(StoreName.TrustedPeople, StoreLocation.LocalMachine, "TP LM"));
            certs.Add(GetStore(StoreName.TrustedPeople, StoreLocation.CurrentUser, "TP CU"));

            return new JsonResult(certs);
        }

        private SimpleStore GetStore(StoreName name, StoreLocation location, string sn)
        {
            using (var store = new X509Store(name, location))
            {
                var ss = new SimpleStore(GetCertsForStore(store), sn);
                return ss;
            }
        }

        private IEnumerable<SimpleCert> GetCertsForStore(X509Store store)
        {
            var certs = new List<SimpleCert>();

            store.Open(OpenFlags.ReadOnly);
            foreach (var cert in store.Certificates)
            {
                certs.Add(new SimpleCert(cert.Subject, cert.FriendlyName));
            }

            return certs;
        }
    }

    class SimpleStore
    {
        public IEnumerable<SimpleCert> _certs { get; }
        public string _name { get; }

        public SimpleStore(IEnumerable<SimpleCert> certs, string name)
        {
            _certs = certs;
            _name = name;
        }
    }

    class SimpleCert
    {
        public string _subject { get; }
        public string _name { get; }

        public SimpleCert(string subject, string name)
        {
            _subject = subject;
            _name = name;

        }
    }
}