using AuthorsHandler.ClientHttp.Abstraction;

namespace AuthorsHandler.ClientHttp;

public class AuthorsHandlerClientHttp : IAuthorsHandlerClientHttp {
	private readonly HttpClient _httpClient;
	public AuthorsHandlerClientHttp(HttpClient httpClient) {
		_httpClient = httpClient;
	}
	public async Task<HttpResponseMessage> GetAuthorFromId(int authorId, CancellationToken cancellationToken = default) {
		UriBuilder uriBuilder = new UriBuilder(_httpClient.BaseAddress + "GetAuthorFromId");
		uriBuilder.Query = $"authorId={authorId}";

		return await _httpClient.GetAsync(uriBuilder.Uri, cancellationToken);
	}
}
