namespace AuthService.Dtos;

public record AuthResponse(string Token, string Username, DateTime ExpiresAt);
