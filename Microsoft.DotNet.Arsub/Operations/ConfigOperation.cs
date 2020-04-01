// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.DotNet.Arsub.Options;
using Newtonsoft.Json;
using Microsoft.DotNet.Arsub.Helpers;

namespace Microsoft.DotNet.Arsub.Operations
{
    internal class ConfigOperation : Operation
    {
        ConfigOptions _options;
        public ConfigOperation(ConfigOptions options) : base(options)
        {
            _options = options;
        }

        public override Task<int> ExecuteAsync()
        {
            var localSettings = LocalSettings.LoadSettingsFile();

            if (_options.Show)
            {
                Console.WriteLine("Current config values:");
                Console.WriteLine($"repo        => {localSettings.Repo ?? "(not configured)"}");
                Console.WriteLine($"github-pat  => {(localSettings.GitHubPat is null ? "(not configured)" : "***" + localSettings.GitHubPat[^3..])}");
            }
            else
            {
                var toBeChanged = _options as IConfigurableCommandLineOptions;

                MergeSettings(localSettings, toBeChanged);

                localSettings.SaveSettingsFile(Logger);

                Console.WriteLine("Requested config values was sucesfully saved");
            }

            return Task.FromResult(Constants.SuccessCode);
        }

        public void MergeSettings(IConfigurableCommandLineOptions localSettings, IConfigurableCommandLineOptions changedOptions)
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var changed = JsonConvert.SerializeObject(changedOptions, serializerSettings);
            JsonConvert.PopulateObject(changed, localSettings, serializerSettings);
        }
    }
}
