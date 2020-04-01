// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.DotNet.Arsub.Options;

namespace Microsoft.DotNet.Arsub.Operations
{
    internal class UnsubscribeToExistingIssuesOperation : SubscribeToExistingIssuesOperation
    {
        UnsubscribeToExistingIssuesOptions _options;
        public UnsubscribeToExistingIssuesOperation(UnsubscribeToExistingIssuesOptions options)
            : base(options)
        {
            _options = options;
        }

        protected override string SubscriptionStatusStrategy => "UNSUBSCRIBED";
    }
}
