#pragma warning disable CS8604
#pragma warning disable CS8603
using WebSocketSharp;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Xrpl.Trader.ConsoleApp.Request;
using Xrpl.Trader.ConsoleApp.Response;
using static Xrpl.Trader.ConsoleApp.Utility.Serialization;

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

    public TRequest CreateRequest<TRequest>(TRequest data) where TRequest : RequestBase
    {
        // Get the next available request ID
        var nextRequestId = Interlocked.Increment(ref _requestId);

        if (nextRequestId > MaximumRequestId)
        {
            // Reset the request ID to 0 and start again
            Interlocked.Exchange(ref _requestId, 0);

            nextRequestId = Interlocked.Increment(ref _requestId);
        }

        data.Id = nextRequestId;

        return data;
    }

    public ResponseBase<TResult> SendRequest<TResult, TRequest>(TRequest request, int timeout = 30000) where TRequest : RequestBase
    {
        var tcs = new TaskCompletionSource<string>();

        var requestId = request.Id;

        var requestString = Serialize(request);

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
                _logger.LogInformation($"Received response from XRPL: {task.Result}");

                // Throw exception on error
                if (task.Exception is not null) throw task.Exception;

                // Return result
                return Deserialize<ResponseBase<TResult>>(Convert.ToString(task.Result));
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
        var response = Deserialize<ResponseBase<object>>(e.Data);

        // Check for an error
        if (response is null || response.Id < 1)
        {
            _logger.LogError($"Null response: {response}");
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
