﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker.Context.Features;

namespace Microsoft.Azure.Functions.Worker
{
    /// <summary>
    /// FunctionContext extension methods for binding data.
    /// </summary>
    public static class FunctionContextBindingFeatureExtensions
    {
        /// <summary>
        /// Gets the input binding data for the current function invocation.
        /// </summary>
        /// <param name="context">The function context instance.</param>
        /// <returns>The input binding data as a read only dictionary.</returns>
        public static IReadOnlyDictionary<string, object?> GetInputData(this FunctionContext context)
        {
            return context.GetBindings().InputData;
        }

        /// <summary>
        /// Gets the trigger meta data for the current function invocation.
        /// </summary>
        /// <param name="context">The function context instance.</param>
        /// <returns>The invocation trigger meta data as a read only dictionary.</returns>
        public static IReadOnlyDictionary<string, object?> GetTriggerMetadata(this FunctionContext context)
        {
            return context.GetBindings().TriggerMetadata;
        }

        /// <summary>
        /// Gets the invocation result of the current function invocation.
        /// </summary>
        /// <param name="context">The function context instance.</param>
        /// <returns>The invocation result value.</returns>
        public static object? GetInvocationResult(this FunctionContext context)
        {
            return context.GetBindings().InvocationResult;
        }

        /// <summary>
        /// Sets the invocation result of the current function invocation.
        /// </summary>
        /// <param name="context">The function context instance.</param>
        /// <param name="value">The invocation result value.</param>
        public static void SetInvocationResult(this FunctionContext context, object? value)
        {
            context.GetBindings().InvocationResult = value;
        }

        /// <summary>
        /// Gets the output binding entries for the current function invocation.
        /// </summary>
        /// <param name="context">The function context instance.</param>
        /// <returns>Collection of <see cref="BindingData"/></returns>
        public static IEnumerable<BindingData> GetOutputBindings(this FunctionContext context)
        {
            var bindingsFeature = context.GetBindings();

            foreach (var data in bindingsFeature.OutputBindingData)
            {
                // Gets binding type (http,queue etc) from function definition.
                string? bindingType = null;
                if (context.FunctionDefinition.OutputBindings.TryGetValue(data.Key, out var bindingData))
                {
                    bindingType = bindingData.Type;
                }

                yield return new BindingData(data.Key, data.Value, bindingType);
            }
        }

        /// <summary>
        /// Sets the value of an output binding entry for the current function invocation.
        /// </summary>
        /// <param name="context">The function context instance.</param>
        /// <param name="name">The name of the output binding entry to set the value for.</param>
        /// <param name="value">The value of the output binding entry.</param>
        /// <exception cref="InvalidOperationException">Throws if no output binding entry present for the name passed in.</exception>
        public static void SetOutputBinding(this FunctionContext context, string name, object? value)
        {
            var bindingFeature = context.GetBindings();

            if (bindingFeature.OutputBindingData.ContainsKey(name))
            {
                bindingFeature.OutputBindingData[name] = value;
            }
            else
            {
                throw new InvalidOperationException($"Output binding entry not present for {name}");
            }
        }
    }
}
