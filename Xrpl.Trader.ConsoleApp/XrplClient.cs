﻿#pragma warning disable CS8604
#pragma warning disable CS8603
using WebSocketSharp;
using Newtonsoft.Json;
using AustinHarris.JsonRpc;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Xrpl.Trader.ConsoleApp;

public class XrplClient
{
    private readonly ILogger<App> _logger;
    private readonly WebSocket _webSocket;
    private static int _requestId;
    private static readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _responses = new();
    public int MaximumRequestId { get; set; } = 999999;

    public XrplClient(WebSocket webSocket, ILogger<App> logger)
    {
        _webSocket = webSocket;
        _webSocket.OnMessage += ProcessMessage;
        _logger = logger;
    }

    public JsonRequest CreateRequest(string method, object data)
    {
        // Get the next available request ID
        var nextRequestId = Interlocked.Increment(ref _requestId);

        if (nextRequestId > MaximumRequestId)
        {
            // Reset the request ID to 0 and start again
            Interlocked.Exchange(ref _requestId, 0);

            nextRequestId = Interlocked.Increment(ref _requestId);
        }

        return new JsonRequest(method, data, nextRequestId);
    }

    public TResult SendRequest<TResult>(JsonRequest request, int timeout = 30000)
    {
        var tcs = new TaskCompletionSource<string>();

        var requestId = request.Id;

        var requestString = JsonConvert.SerializeObject(request);

        try
        {
            // Add req deets to responses so that we have an entry to match against when response comes back from XRPL
            _responses.TryAdd(Convert.ToString(requestId), tcs);

            // Send request to ripple
            _logger.LogInformation($"Sending request: {requestString}");
            _webSocket.Send(requestString);
            _logger.LogInformation("Request sent!");

            var task = tcs.Task;

            // Wait until the response is received by XRPL or we reach timeout of 30s
            Task.WaitAll(new Task[] { task }, timeout);

            if (task.IsCompleted)
            {
                // Parse result
                var response = JsonConvert.DeserializeObject<JsonResponse>(task.Result);

                var responseString = JsonConvert.SerializeObject(response);
                _logger.LogInformation($"Received response from XRPL: {responseString}");

                // Throw exception on error
                if (response.Error is not null) throw response.Error;

                // Return result
                return JsonConvert.DeserializeObject<TResult>(Convert.ToString(response.Result),
                    new JsonSerializerSettings
                    {
                        Error = (sender, args) => args.ErrorContext.Handled = true
                    });
            }
            else // Timeout response
            {
                _logger.LogError($"Client timeout of {timeout} milliseconds has expired, throwing timeout exception");
                throw new TimeoutException();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wront while trying to send request: {requestString}");
            throw;
        }
        finally
        {
            // Remove the request/response entry in the 'finally' block to avoid leaking memory
            _responses.TryRemove(Convert.ToString(requestId), out tcs);
        }
    }

    private void ProcessMessage(object sender, MessageEventArgs e)
    {
        // Check for pings
        if (e.IsPing)
        {
            _logger.LogInformation("Received ping");
            return;
        }

        _logger.LogInformation("Processing message");

        // Log when the message is binary
        if (e.IsBinary) _logger.LogInformation("Incoming message type is binary");

        _logger.LogInformation($"Incoming message data: {e.Data}");

        // Parse response from XRPL
        var response = JsonConvert.DeserializeObject<JsonResponse>(e.Data,
            new JsonSerializerSettings()
            {
                Error = (sender, args) => args.ErrorContext.Handled = true
            });

        // Check for an error
        if (response?.Error != null)
        {
            // Log error deets
            _logger.LogError($"Error message: {response.Error.message}");
            _logger.LogError($"Error code: {response.Error.code}");
            _logger.LogError($"Error data: {response.Error.data}");
        }

        // Set the response result
        if (_responses.TryGetValue(Convert.ToString(response.Id), out TaskCompletionSource<string> tcs))
        {
            tcs.TrySetResult(e.Data);
        }
        else
        {
            _logger.LogError($"Unexpected response received from XRPL for ID '{response.Id}'");
        }

        _logger.LogInformation("Finished processing incoming message from XRPL");
    }
}
