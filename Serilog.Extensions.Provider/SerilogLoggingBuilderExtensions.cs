using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Serilog.Extensions.Provider
{
    /// <summary>
    /// Extends <see cref="ILoggingBuilder"/> with Serilog configuration methods.
    /// </summary>
    public static class SerilogLoggingBuilderExtensions
    {
        /// <summary>
        /// Add Serilog to the logging pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Logging.ILoggingBuilder" /> to add logging provider to.</param>
        /// <returns>The logger factory.</returns>
        public static ILoggingBuilder AddSerilog(this ILoggingBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>();

            builder.AddFilter<SerilogLoggerProvider>(null, LogLevel.Trace);

            return builder;
        }
    }
}
