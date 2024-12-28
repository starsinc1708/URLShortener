namespace URLShortener.API.Models
{
	public record ShortenedUrl(string ShortCode, string OriginalUrl, DateTime CreatedAt);
}
