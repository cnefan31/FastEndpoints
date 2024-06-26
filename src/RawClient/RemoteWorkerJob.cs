// Copyright (c) 2024 Vanderlande Industries GmbH All rights reserved. 
//  
// The copyright to the computer program(s) herein is the property of 
// Vanderlande Industries. The program(s) may be used and/or copied 
// only with the written permission of the owner or in accordance with 
// the terms and conditions stipulated in the contract under which the 
// program(s) have been supplied. 
// File: RemoteWorkerJob.cs 
// History: 
//

using Contracts;
using FastEndpoints;
using Microsoft.Extensions.Hosting;

namespace RawClient;

internal class RemoteWorkerJob : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            CreateOrderCommand remoteCommand = new CreateOrderCommand();
            try
            {
                var result = await remoteCommand.RemoteExecuteAsync();
                Console.WriteLine(result.EventTime.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
