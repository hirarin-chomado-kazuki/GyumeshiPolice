using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Windows.ApplicationModel;

namespace GyumeshiPolice
{
    public class AppConfig
    {
        private readonly IConfigurationRoot _configurationRoot;

        public AppConfig()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifes‌​tResourceNames()
                .FirstOrDefault(x => x.EndsWith("appsettings.json"));

            if (resourceName != null)
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    var builder = new ConfigurationBuilder().AddJsonStream(stream);
                    _configurationRoot = builder.Build();
                }
            }
        }

        private T GetSection<T>(string key) => _configurationRoot.GetSection(key).Get<T>();

        public CustomVisionSettings CustomVisionSettings => GetSection<CustomVisionSettings>(nameof(CustomVisionSettings));
    }

    public class CustomVisionSettings
    {
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
        public string ProjectId { get; set; }
        public string PublishedName { get; set; }
    }
}
