using UsersHandler.ClientHttp.Abstraction;

namespace UsersHandler.ClientHttp;

public class UsersHandlerClientHttp : IUsersHandlerClientHttp {
	private readonly HttpClient _httpClient;
	public UsersHandlerClientHttp(HttpClient httpClient) {
		_httpClient = httpClient;
	}
	public async Task<HttpResponseMessage> GetUserFromId(int userId, CancellationToken cancellationToken = default) {
		UriBuilder uriBuilder = new UriBuilder(_httpClient.BaseAddress + "GetUserFromId");
		uriBuilder.Query = $"authorId={userId}";

		return await _httpClient.GetAsync(uriBuilder.Uri, cancellationToken);
	}
}
