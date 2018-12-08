using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace SignalRProgressBar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                           .ConfigureAppConfiguration((ctx, builder) =>
                           {
                               var keyVaultEndpoint = GetKeyVaultEndpoint();
                               if (!string.IsNullOrEmpty(keyVaultEndpoint))
                               {
                                   var azureServiceTokenProvider = new AzureServiceTokenProvider();
                                   var keyVaultClient = new KeyVaultClient(
                                       new KeyVaultClient.AuthenticationCallback(
                                           azureServiceTokenProvider.KeyVaultTokenCallback));
                                   builder.AddAzureKeyVault(
                                       keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                               }
                           }
                ).UseStartup<Startup>();

        private static string GetKeyVaultEndpoint() => "https://signalrprogressbar-0-kv.vault.azure.net";
    }
}
