using TransactionService.Domain.Entities;

namespace TransactionService.Infrastructure.ExternalServices;

public class AntiFraudHttpClient
{
    private readonly HttpClient _httpClient;

    public AntiFraudHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> SendToAntiFraudAsync(Transaction transaction)
    {
        var body = new
        {
            transactionId = transaction.Id,
            sourceAccountId = transaction.SourceAccountId,
            value = transaction.Value
        };

        return await _httpClient.PostAsJsonAsync("api/antifraud/validate", body);
    }
}
