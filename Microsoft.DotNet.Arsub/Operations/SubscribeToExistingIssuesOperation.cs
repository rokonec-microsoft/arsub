// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.DotNet.Arsub.Options;
using System.Linq;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.DotNet.Arsub.Operations
{
    internal class SubscribeToExistingIssuesOperation : Operation
    {
        SubscribeToExistingIssuesOptions _options;
        public SubscribeToExistingIssuesOperation(SubscribeToExistingIssuesOptions options)
            : base(options)
        {
            _options = options;
        }

        public override async Task<int> ExecuteAsync()
        {
            if (_options.Labels?.Count() == 0)
                throw new InvalidOperationException("No label(s) specified");

            var graphClient = new GraphQLHttpClient("https://api.github.com/graphql", new NewtonsoftJsonSerializer());
            graphClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.GitHubPat}");
            graphClient.HttpClient.DefaultRequestHeaders.Add("User-Agent", "arsub/0.1");

            var issuesToSubscribeVariables = new IssuesToSubscribeVariables
            {
                query = $"is:issue state:open {IncludingLabelsFilter()} repo:{_options.Repo} {DateRangeFilter()}",
                after = (string)null,
            };
            var issuesToSubscribeRequest = new GraphQLRequest
            {
                Query = @"
                query IssuesToSubscribe($query:String!, $after:String) {
                  search(query: $query, type: ISSUE, first: 100, after: $after) {
                    nodes {
                      ... on Issue {
                        __typename
                        id
                        number
                        title
                        updatedAt
                        url
                        labels(first: 100) {
                          nodes {
                            name
                          }
                        }
                      }
                    }
                    pageInfo {
                      endCursor
                      hasNextPage
                    }
                    issueCount
                  }
                }",
                OperationName = "IssuesToSubscribe",
                Variables = issuesToSubscribeVariables,
            };

            var subscribeToIssueMutation = new GraphQLRequest
            {
                Query = @"
                mutation SubscribeToIssue($issue:String!, $state:SubscriptionState!) {
                  __typename
                  updateSubscription(input: {state: $state, subscribableId: $issue}) {
                    clientMutationId
                  }
                }",
                OperationName = "SubscribeToIssue"
            };

            int countSubscribed = 0;
            bool nextPage = true;

            while (nextPage)
            {
                Logger.LogDebug($"GraphQL query to {graphClient.HttpClient.BaseAddress} with query:{issuesToSubscribeRequest.Query}\nusing variables:\n{JsonConvert.SerializeObject(issuesToSubscribeRequest.Variables, Formatting.Indented)}");
                var issuesQueryResult = await graphClient.SendQueryAsync<dynamic>(issuesToSubscribeRequest);
                if (issuesQueryResult.Errors?.Any() == true)
                {
                    Logger.LogError($"GraphQL query errors: {JsonConvert.SerializeObject(issuesQueryResult.Errors, Formatting.Indented)}");
                    return Constants.ErrorCode;
                }
                Logger.LogDebug($"GraphQL query result: \n{JsonConvert.SerializeObject(issuesQueryResult.Data, Formatting.Indented)}");

                dynamic data = issuesQueryResult.Data;

                foreach (dynamic issue in data.search.nodes)
                {
                    // TODO: test if already subscribed by events and ignore if unsubscribed afer it have received that label

                    subscribeToIssueMutation.Variables = new
                    {
                        issue = issue.id,
                        state = SubscriptionStatusStrategy,
                    };

                    Logger.LogDebug($"GraphQL mutation {graphClient.HttpClient.BaseAddress} with mutation:{subscribeToIssueMutation.Query}\nusing variables:\n{JsonConvert.SerializeObject(subscribeToIssueMutation.Variables, Formatting.Indented)}");
                    if (!_options.WhatIf)
                    {
                        var mutation = await graphClient.SendMutationAsync<dynamic>(subscribeToIssueMutation);
                        if (mutation.Errors?.Any() != true)
                        {
                            Logger.LogDebug($"GraphQL query mutation result: \n{JsonConvert.SerializeObject(mutation, Formatting.Indented)}");
                            countSubscribed++;
                        }
                        else
                        {
                            Logger.LogError($"GraphQL mutation errors: {JsonConvert.SerializeObject(mutation.Errors, Formatting.Indented)}");
                            return Constants.ErrorCode;
                            // TODO: retry?
                        }
                    }
                    Console.WriteLine($"{issue.url} => {issue.title}");
                }

                if (issuesQueryResult.Errors?.Any() != true && data.search.pageInfo.hasNextPage == true)
                {
                    issuesToSubscribeVariables.after = (string)data.search.pageInfo.endCursor;
                }
                else
                {
                    nextPage = false;
                }
            }

            if (countSubscribed > 0)
            {
                Console.WriteLine($"Succesfully changed subscription to {countSubscribed} issues");
            }
            else
            {
                Console.WriteLine($"No issue found!");
            }

            return Constants.SuccessCode;
        }

        protected virtual string SubscriptionStatusStrategy => "SUBSCRIBED";

        private string DateRangeFilter()
        {
            if (_options.Since.HasValue && _options.Before.HasValue)
                return $" updated:{Date8601(_options.Since)}..{Date8601(_options.Before)}";
            if (_options.Since.HasValue && !_options.Before.HasValue)
                return $" updated:>={Date8601(_options.Since)}";
            if (!_options.Since.HasValue && _options.Before.HasValue)
                return $" updated:<={Date8601(_options.Before)}";

            return string.Empty;
        }

        private string Date8601(DateTimeOffset? dateTime)
        {
            return $"{dateTime:s}Z";
        }

        private string IncludingLabelsFilter()
        {
            return string.Join(" ", _options.Labels.Select(s => $"label:{s}").ToArray());
        }
    }

    internal class IssuesToSubscribeVariables
    {
        public string query { get; set; }
        public string after { get; set; }
    }
}
