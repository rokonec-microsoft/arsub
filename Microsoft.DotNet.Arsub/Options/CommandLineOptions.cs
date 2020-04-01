// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommandLine;

namespace Microsoft.DotNet.Arsub.Options
{

    abstract class CommandLineOptions : ConfigurableCommandLineOptions, ILogLevelOptions
    {
        [Option("verbose", HelpText = "Turn on verbose output.")]
        public bool Verbose { get; set; }

        [Option("debug", HelpText = "Turn on debug output.")]
        public bool Debug { get; set; }

        [Option("what-if", HelpText = "Do not change anything - just simulate output on console.")]
        public bool WhatIf { get; set; }

        [Option("output-format", Default = OutputType.text, HelpText = "Desired output type of arsub. Valid values are 'json', 'text' and 'markdown'. Case sensitive.")]
        public OutputType OutputFormat { get; set; }
    }
}
