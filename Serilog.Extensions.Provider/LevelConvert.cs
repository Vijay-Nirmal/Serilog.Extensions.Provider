// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Serilog.Extensions.Provider
{
    internal static class LevelConvert
    {
        public static LogEventLevel ToSerilogLevel(this LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.None:
                case LogLevel.Critical:
                    return LogEventLevel.Fatal;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                case LogLevel.Information:
                    return LogEventLevel.Information;
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                case LogLevel.Trace:
                default:
                    return LogEventLevel.Verbose;
            }
        }
    }
}