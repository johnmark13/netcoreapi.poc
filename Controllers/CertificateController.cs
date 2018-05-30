using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TodoApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Certificate")]
    public class CertificateController : Controller
    {
        private readonly IOptions<StatusSettings> _settings;

        public CertificateController(IOptions<StatusSettings> settings)
        {
            _settings = settings;
        }

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
            certs.Add(GetStore(StoreName.TrustedPublisher, StoreLocation.LocalMachine, "TPub LM"));
            certs.Add(GetStore(StoreName.TrustedPublisher, StoreLocation.CurrentUser, "TPub CU"));

            try
            {
                X509Certificate2 cert = new X509Certificate2("/opt/app-root/certs/DP3.pfx", _settings.Value.CertPassword);

                var lc = new SimpleCert(cert.FriendlyName, cert.Subject);
                certs.Add(new SimpleStore(new List<SimpleCert>() { lc }, "Local"));
            }
            catch (Exception e)
            {
                certs.Add(new SimpleStore(new List<SimpleCert>(), "Local: " + e.Message) { failure = true });
            }
            return new JsonResult(certs);
        }

        private SimpleStore GetStore(StoreName name, StoreLocation location, string sn)
        {
            try
            {
                using (var store = new X509Store(name, location))
                {
                    var ss = new SimpleStore(GetCertsForStore(store), sn);
                    return ss;
                }
            }
            catch (System.Security.Cryptography.CryptographicException ce)
            {
                var ss = new SimpleStore(new List<SimpleCert>(), sn);
                ss.failure = true;
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

        public bool failure { get; set; }

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