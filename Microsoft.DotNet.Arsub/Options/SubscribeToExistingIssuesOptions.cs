// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommandLine;
using CommandLine.Text;
using Microsoft.DotNet.Arsub.Operations;
using System;
using System.Collections.Generic;

namespace Microsoft.DotNet.Arsub.Options
{
    [Verb("subscribe", HelpText = "Subscribe to existing issues with label")]
    internal class SubscribeToExistingIssuesOptions : SubscribeUnsubscribeToExistingIssuesSharedOptions
    {
        [Usage(ApplicationAlias = "arsub")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Subscsribe to label", new SubscribeToExistingIssuesOptions {
                    Repo = "dot-net/runtime",
                    Labels = new[] { "area-GC", "area-roslyn" },
                    Since = DateTime.UtcNow.AddDays(-14),
                    Before = DateTime.UtcNow.AddDays(-7),
                });
            }
        }

        public override Operation GetOperation()
        {
            return new SubscribeToExistingIssuesOperation(this);
        }
    }
}
