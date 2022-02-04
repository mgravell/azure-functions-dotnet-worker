﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace FunctionApp
{
    public class MyCustomMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                context.GetLogger(nameof(MyCustomMiddleware)).LogError(ex, "error in function invocation");
                 
                // To read input/trigger meta data.
                var inputs = context.GetInputData();
                var triggerMetaData = context.GetTriggerMetadata();


                //BindingData<HttpRequestData> httpReq = context.BindInput<HttpRequestData>();

                //BindingData<Blob> b = context.BindInput<Blob>();

                //context.BindInput<string>("productId"); // productId is read from inputbinding ref

                // context needs to expose inputbinding in easy way
                // context.BindInput overload can take this input binding ref
                // cache the conversion result.
                //

                // Get http request(null for non http invocations)
                var httpRequest = context.GetHttpRequestData();

                

                if (httpRequest != null)
                {
                    var newResponse = httpRequest.CreateResponse();
                    await newResponse.WriteAsJsonAsync(new { Status = "Failed", ErrorCode = "function-app-500" });

                    // Update invocation result.
                    context.SetInvocationResult(newResponse);

                    // OR Read the output bindings and update as needed
                    System.Collections.Generic.IEnumerable<BindingData> outputBindings = context.GetOutputBindings();

                    // Update the output for queue binding.
                    var queueOutputData = outputBindings.FirstOrDefault(a => a.Type == "queue");
                    if (queueOutputData != null)
                    {
                        context.SetOutputBinding(queueOutputData.Name, "Custom value from middleware");
                    }
                }
            }
        }
    }
}

