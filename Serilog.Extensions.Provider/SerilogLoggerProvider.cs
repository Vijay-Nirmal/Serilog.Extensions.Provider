// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using FrameworkLogger = Microsoft.Extensions.Logging.ILogger;

namespace Serilog.Extensions.Provider
{
    /// <summary>
    /// An <see cref="ILoggerProvider"/> that pipes events through Serilog.
    /// </summary>
    [ProviderAlias("Serilog")]
    public sealed class SerilogLoggerProvider : ILoggerProvider, ILogEventEnricher
    {
        internal const string _originalFormatPropertyName = "{OriginalFormat}";
        internal const string _scopePropertyName = "Scope";

        private readonly ILogger _logger;

        /// <summary>
        /// Construct a <see cref="SerilogLoggerProvider"/>.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public SerilogLoggerProvider(IConfiguration configuration)
        {
            var logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            _logger = logger.ForContext(new[] { this });
        }

        /// <summary>
        /// Create Logger Instance
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public FrameworkLogger CreateLogger(string categoryName)
        {
            return new SerilogLogger(this, _logger, categoryName);
        }

        /// <inheritdoc cref="IDisposable" />
        public IDisposable BeginScope<T>(T state)
        {
            if (CurrentScope != null)
                return new SerilogLoggerScope(this, state);

            // The outermost scope pushes and pops the Serilog `LogContext` - once
            // this enricher is on the stack, the `CurrentScope` property takes care
            // of the rest of the `BeginScope()` stack.
            var popSerilogContext = LogContext.Push(this);
            return new SerilogLoggerScope(this, state, popSerilogContext);
        }

        /// <inheritdoc />
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            List<LogEventPropertyValue> scopeItems = null;
            for (var scope = CurrentScope; scope != null; scope = scope.Parent)
            {
                scope.EnrichAndCreateScopeItem(logEvent, propertyFactory, out LogEventPropertyValue scopeItem);

                if (scopeItem != null)
                {
                    scopeItems = scopeItems ?? new List<LogEventPropertyValue>();
                    scopeItems.Add(scopeItem);
                }
            }

            if (scopeItems != null)
            {
                scopeItems.Reverse();
                logEvent.AddPropertyIfAbsent(new LogEventProperty(_scopePropertyName, new SequenceValue(scopeItems)));
            }
        }

        private readonly AsyncLocal<SerilogLoggerScope> _value = new AsyncLocal<SerilogLoggerScope>();

        internal SerilogLoggerScope CurrentScope
        {
            get => _value.Value;
            set => _value.Value = value;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Noting to Dispose
        }
    }
}
