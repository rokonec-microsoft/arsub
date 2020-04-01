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
    [Verb("unsubscribe", HelpText = "Unsubscribe from existing issues with labels")]
    internal class UnsubscribeToExistingIssuesOptions : SubscribeToExistingIssuesOptions
    {
        [Usage(ApplicationAlias = "arsub")]
        public static new IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Unsubscsribe from label", new SubscribeToExistingIssuesOptions {
                    Repo = "dot-net/runtime",
                    Labels = new[] { "area-GC", "area-roslyn" },
                    Since = DateTime.UtcNow.AddDays(-14),
                    Before = DateTime.UtcNow.AddDays(-7),
                });
            }
        }

        public override Operation GetOperation()
        {
            return new UnsubscribeToExistingIssuesOperation(this);
        }
    }
}