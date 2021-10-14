﻿using FastEndpoints.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace FastEndpoints;

internal record EndpointDefinitionCacheEntry(
    Func<object> EndpointFactory,
    IValidator? Validator,
    object? PreProcessors,
    object? PostProcessors);

[HideFromDocs]
public static class EndpointExecutor
{
    //key: route url for the endpoint
    internal static Dictionary<string, EndpointDefinitionCacheEntry> CachedEndpointDefinitions { get; } = new();

    //key: TEndpoint
    internal static Dictionary<Type, PropertyInfo[]> CachedServiceBoundProps { get; } = new();

    //note: this handler is called by .net for each http request
    public static Task HandleAsync(HttpContext ctx, CancellationToken cancellation)
    {
        var route = ((RouteEndpoint?)ctx.GetEndpoint())?.RoutePattern.RawText;
        if (route is null) throw new InvalidOperationException("Unable to instantiate endpoint!!!");

        var epDef = CachedEndpointDefinitions[route];

        var endpointInstance = epDef.EndpointFactory();

        ResolveServices(endpointInstance, ctx);

#pragma warning disable CS8601
        return (Task?)BaseEndpoint.ExecMethodInfo.Invoke(endpointInstance, new object[] { ctx, epDef.Validator, epDef.PreProcessors, epDef.PostProcessors, cancellation })
            ?? Task.CompletedTask;
#pragma warning restore CS8601
    }

    private static void ResolveServices(object endpointInstance, HttpContext ctx)
    {
        if (CachedServiceBoundProps.TryGetValue(endpointInstance.GetType(), out var props))
        {
            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo? prop = props[i];
                var serviceInstance = ctx.RequestServices.GetService(prop.PropertyType);
                prop.SetValue(endpointInstance, serviceInstance);
            }
        }
    }
}

