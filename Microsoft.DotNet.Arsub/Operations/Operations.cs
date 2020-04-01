// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.DotNet.Arsub.Helpers;
using Microsoft.DotNet.Arsub.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Microsoft.DotNet.Arsub.Operations
{
    internal abstract class Operation : IDisposable
    {
        protected ILoggerFactory _loggerFactory;
        private ILogger _logger;

        protected ILogger Logger { get { return _logger; } }

        public Operation(ICommandLineOptions options)
        {
            // Because the internal logging tends to be chatty and non-useful,
            // we remap the --verbose switch onto 'info', --debug onto highest level, and the
            // default level onto warning

            var logLevelOptions = options as ILogLevelOptions;
            LogLevel level = LogLevel.Warning;
            if (logLevelOptions?.Debug ?? false)
            {
                level = LogLevel.Debug;
            }
            else if (logLevelOptions?.Verbose ?? false)
            {
                level = LogLevel.Information;
            }
            _loggerFactory = LoggerFactory.Create(builder => {
                builder
                    .AddFilter("", level)
                    .AddConsole();
            });

            _logger = _loggerFactory.CreateLogger(this.GetType().FullName);

            // to not apply local settings for config command or any other options not derived from ConfigurableCommandLineOptions
            if (options is ConfigurableCommandLineOptions)
            {
                LocalSettings.ApplySettings(options as ConfigurableCommandLineOptions, _logger);
            }
        }

        public abstract Task<int> ExecuteAsync();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _loggerFactory.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
