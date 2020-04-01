// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.DotNet.Arsub.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Microsoft.DotNet.Arsub.Helpers
{
    /// <summary>
    /// Reads and writes the settings file
    /// </summary>
    internal class LocalSettings : IConfigurableCommandLineOptions
    {
        public string Repo { get; set; }
        public string GitHubPat { get; set; }

        /// <summary>
        /// Saves the settings in the settings files
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public int SaveSettingsFile(ILogger logger)
        {
            string settings = JsonConvert.SerializeObject(this);
            return EncodedFile.Create(Constants.SettingsFileName, settings, logger);
        }

        public static LocalSettings LoadSettingsFile()
        {
            try
            {
                string settings = EncodedFile.Read(Constants.SettingsFileName);
                return JsonConvert.DeserializeObject<LocalSettings>(settings);
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is FileNotFoundException)
            {
                return new LocalSettings();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load the arsub settings file, may be corrupted", ex);
            }
        }

        /// <summary>
        /// Change options by combination of the command line
        /// options and the user's arsub settings file.
        /// </summary>
        /// <param name="options">Command line options</param>
        /// <returns>arsub settings for use in remote commands</returns>
        /// <remarks>The command line takes precedence over the arsub settings file.</remarks>
        public static void ApplySettings(ConfigurableCommandLineOptions options, ILogger logger)
        {
            LocalSettings localSettings = null;
            localSettings = LoadSettingsFile();

            // Override if non-empty on command line
            options.GitHubPat = OverrideIfSet(localSettings.GitHubPat, options.GitHubPat);
            options.Repo = OverrideIfSet(localSettings.Repo, options.Repo);
        }

        private static string OverrideIfSet(string localSettings, string commandLineSetting)
        {
            return string.IsNullOrEmpty(commandLineSetting) ? localSettings : commandLineSetting;
        }
    }
}
