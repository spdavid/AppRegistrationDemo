using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.SharePoint.Client;

namespace AzureFunctionsExample
{
    public static class TimerFunction
    {
        [FunctionName("TimerFunction")]
        public static void Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            using (ClientContext ctx = Helpers.ContextHelper.GetSPContext("https://zalo.sharepoint.com/sites/spsne"))
            {
                ctx.Load(ctx.Web);
                ctx.ExecuteQuery();

                log.Info("my Site is : " + ctx.Web.Title);
            }

        }
    }
}
