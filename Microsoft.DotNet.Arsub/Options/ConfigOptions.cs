// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommandLine;
using Microsoft.DotNet.Arsub.Operations;

namespace Microsoft.DotNet.Arsub.Options
{
    [Verb("config", HelpText = "Configure and persist as some default values")]
    internal class ConfigOptions : ILogLevelOptions, ICommandLineOptions, IConfigurableCommandLineOptions
    {
        [Option("repo",  HelpText = "GitHub repository id if format {owner}/{name}")]
        public string Repo { get; set; }

        [Option("github-pat", HelpText = "Personal access token used to authenticate GitHub.")]
        public string GitHubPat { get; set; }

        [Option("verbose", HelpText = "Turn on verbose output.")]
        public bool Verbose { get; set; }

        [Option("debug", HelpText = "Turn on debug output.")]
        public bool Debug { get; set; }

        [Option("show", HelpText = "Do not change anything, just show current values")]
        public bool Show { get; set; }

        public Operation GetOperation()
        {
            return new ConfigOperation(this);
        }

        public void Verify()
        {
            // all combination of arguments are allowed
        }
    }
}
