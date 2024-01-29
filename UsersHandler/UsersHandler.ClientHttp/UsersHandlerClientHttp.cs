using UsersHandler.ClientHttp.Abstraction;

namespace UsersHandler.ClientHttp;

public class UsersHandlerClientHttp : IUsersHandlerClientHttp {
	private readonly HttpClient _httpClient;
	public UsersHandlerClientHttp(HttpClient httpClient) {
		_httpClient = httpClient;
	}
	public async Task<HttpResponseMessage> GetUserFromId(int authorId, CancellationToken cancellationToken = default) {
		UriBuilder uriBuilder = new UriBuilder(_httpClient.BaseAddress + "GetAuthorFromId");
		uriBuilder.Query = $"authorId={authorId}";

		return await _httpClient.GetAsync(uriBuilder.Uri, cancellationToken);
	}
}
