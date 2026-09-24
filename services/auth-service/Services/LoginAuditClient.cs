using System.Net.Http.Json;

namespace AuthService.Services;

public class LoginAuditClient(HttpClient httpClient, ILogger<LoginAuditClient> logger)
{
	public async Task<LoginAuditResult> RegisterAsync(string username, Guid? userId, bool successful, CancellationToken cancellationToken)
	{
		try
		{
			var request = new LoginAttemptRequest(username, userId, DateTime.UtcNow, successful);
			var response = await httpClient.PostAsJsonAsync("api/login-attempts", request, cancellationToken);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<LoginAuditResult>(cancellationToken: cancellationToken) ?? new LoginAuditResult(0, 0);
		}
		catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
		{
			logger.LogWarning(exception, "No se pudo registrar el intento de login para {Username}.", username);
			return new LoginAuditResult(0, 0);
		}
	}

	private sealed record LoginAttemptRequest(string Username, Guid? UserId, DateTime RegisteredAt, bool Successful);
}

public sealed record LoginAuditResult(int AttemptNumber, int ConsecutiveFailures);