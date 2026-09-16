using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace LoanFlow.IntegrationTests.Support;

public sealed class StubExternalService : HttpMessageHandler
{
    private readonly ConcurrentQueue<RecordedRequest> _requests = new();
    private Func<HttpResponseMessage> _respond = Succeed;

    public IReadOnlyList<RecordedRequest> Requests => [.. _requests];

    public void RespondWith(HttpStatusCode statusCode) => _respond = () => new HttpResponseMessage(statusCode);

    public void FailWith(Exception exception) => _respond = () => throw exception;

    public void Reset()
    {
        _requests.Clear();
        _respond = Succeed;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var body = request.Content is null ? "{}" : await request.Content.ReadAsStringAsync(cancellationToken);
        _requests.Enqueue(new RecordedRequest(request.Method, request.RequestUri!.AbsolutePath, JsonDocument.Parse(body).RootElement));
        return _respond();
    }

    private static HttpResponseMessage Succeed() => new(HttpStatusCode.OK);
}

public sealed record RecordedRequest(HttpMethod Method, string Path, JsonElement Body);
