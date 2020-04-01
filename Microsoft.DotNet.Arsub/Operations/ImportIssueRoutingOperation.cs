// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.DotNet.Arsub.Options;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Microsoft.DotNet.Arsub.Operations
{
    internal class ImportIssueRoutingOperation : Operation
    {
        ImportIssueRoutingOptions _options;
        public ImportIssueRoutingOperation(ImportIssueRoutingOptions options)
            : base(options)
        {
            _options = options;
        }

        public override async Task<int> ExecuteAsync()
        {
            // get file from GitHub
            string areaOwnersContent;
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync($"https://raw.githubusercontent.com/{_options.Repo}/{_options.Path}"))
                {
                    areaOwnersContent = await response.Content.ReadAsStringAsync();
                }

                // get configs from fabric-bot
                var parsed = new List<FabricBotIssueRoutingLabelsAndMentions>();

                // syntax: | label-name | @owner[ @owner]... |
                Regex labelSubscriptionPattern = new Regex(@"^\|\s+(?'label'[^,|]+)\s+\|(\s*@(?'users'[\w-/]+))+\s*\|.*", RegexOptions.Singleline);
                using (StringReader sr = new StringReader(areaOwnersContent))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var match = labelSubscriptionPattern.Match(line);
                        if (match.Success)
                        {
                            var label = match.Groups["label"].Captures[0].Value;
                            var subscribers = new List<string>();

                            foreach (Capture capture in match.Groups["users"].Captures)
                            {
                                if (capture == null)
                                    continue;

                                subscribers.Add(capture.Value);
                            }

                            if (subscribers.Any())
                            {
                                parsed.Add(new FabricBotIssueRoutingLabelsAndMentions(new[] { label }, subscribers.ToArray()));
                            }
                        }
                    }
                }

                Console.WriteLine(JsonConvert.SerializeObject(parsed, Formatting.Indented));
            }

            return Constants.SuccessCode;
        }

        private class FabricBotIssueRoutingLabelsAndMentions
        {
            public string[] labels;
            public string[] mentionees;

            public FabricBotIssueRoutingLabelsAndMentions(string[] label, string[] mentionees)
            {
                this.labels = label;
                this.mentionees = mentionees;
            }
        }
    }
}
