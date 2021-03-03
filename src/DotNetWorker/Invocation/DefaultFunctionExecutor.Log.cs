﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.Functions.Worker.Context.Features;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.Functions.Worker.Invocation
{
    internal partial class DefaultFunctionExecutor
    {
        private static class Log
        {
            private static readonly Action<ILogger, string, string, Exception?> _functionBindingFeatureUnavailable =
                WorkerMessage.Define<string, string>(LogLevel.Warning, new EventId(2, nameof(FunctionBindingFeatureUnavailable)),
                    "The feature " + nameof(IModelBindingFeature) + " was not available for invocation '{invocationId}' of function '{functionName}'. Unable to process input bindings.");

            public static void FunctionBindingFeatureUnavailable(ILogger<DefaultFunctionExecutor> logger, FunctionContext context)
            {
                _functionBindingFeatureUnavailable(logger, context.Invocation.Id, context.Invocation.FunctionId, null);
            }
        }
    }
}
