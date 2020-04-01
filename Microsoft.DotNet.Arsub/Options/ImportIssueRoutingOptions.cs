// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommandLine;
using Microsoft.DotNet.Arsub.Operations;

namespace Microsoft.DotNet.Arsub.Options
{
    [Verb("import-issue-routings", HelpText = "Import issue routings from reposity docs/area-owners.md as present in dotnet/runtime and output them as json")]
    internal class ImportIssueRoutingOptions : CommandLineOptions
    {
        [Option("path", HelpText = "Path to area-owners.md file if format {branch}/{path}/{filename}")]
        public string Path { get; set; }

        public override Operation GetOperation()
        {
            return new ImportIssueRoutingOperation(this);
        }
    }
}
