// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommandLine;
using Microsoft.DotNet.Arsub.Options;
using System;
using System.Collections.Generic;

internal abstract class SubscribeUnsubscribeToExistingIssuesSharedOptions : CommandLineOptions
{
    [Option("label", Required = true, HelpText = "Label ")]
    public IEnumerable<string> Labels { get; set; }

    [Option("since", HelpText = "Only issues updated after this value")]
    public DateTimeOffset? Since { get; set; }

    [Option("before", HelpText = "Only issues updated before this value")]
    public DateTimeOffset? Before { get; set; }
}