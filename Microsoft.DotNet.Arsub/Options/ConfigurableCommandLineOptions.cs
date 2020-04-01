// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommandLine;
using Microsoft.DotNet.Arsub.Operations;
using System;

namespace Microsoft.DotNet.Arsub.Options
{
    abstract class ConfigurableCommandLineOptions : IConfigurableCommandLineOptions, ICommandLineOptions
    {
        [Option("repo", HelpText = "GitHub repository id if format {owner}/{name}")]
        public string Repo { get; set; }

        [Option("github-pat", HelpText = "Personal access token used to authenticate GitHub.")]
        public string GitHubPat { get; set; }

        public abstract Operation GetOperation();

        public void Verify()
        {
            if (string.IsNullOrEmpty(Repo))
                throw new ArgumentException("Missing argument", "repo");
            if (string.IsNullOrEmpty(GitHubPat))
                throw new ArgumentException("Missing argument", "github - pat");
        }
    }
}
