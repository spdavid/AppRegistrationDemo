using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsExample.Helpers
{
    class ContextHelper
    {
        public static ClientContext GetSPContext(string siteUrl)
        {
            X509Certificate2 cert2 = null;
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);
              
                var col = store.Certificates.Find(X509FindType.FindByThumbprint, "81A0B0E514991743C363F255139E32874D1AAC13", false);
                if (col == null || col.Count == 0)
                {
                    return null;
                }
                cert2 = col[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                store.Close();
            }

            AuthenticationManager authmanager = new AuthenticationManager();

            var ctx = authmanager.GetAzureADAppOnlyAuthenticatedContext(siteUrl, ConfigurationManager.AppSettings["ClientId"], ConfigurationManager.AppSettings["tenant"], cert2);
            return ctx;
        }


    }
}
