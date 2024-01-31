using Microsoft.Extensions.Configuration;
using UsersHandler.ClientHttp.Abstraction;

namespace UsersHandler.ClientHttp;

public class UsersHandlerClientHttp : IUsersHandlerClientHttp {
	private readonly HttpClient _httpClient;
	public UsersHandlerClientHttp(HttpClient httpClient, IConfiguration configuration) {
		_httpClient = httpClient;
		_httpClient.BaseAddress = new Uri(
			configuration["MusicalScoresHandlerClientHttp:UsersAPIBaseAddress"] 
			?? throw new FileLoadException("No such config <MusicalScoresHandlerClientHttp:UsersAPIBaseAddress>") 
		);
	}
	
	public async Task<HttpResponseMessage> GetUserFromId(int userId, CancellationToken cancellationToken = default) {
		UriBuilder uriBuilder = new UriBuilder(_httpClient.BaseAddress + "GetUserFromId");
		uriBuilder.Query = $"userId={userId}";

		return await _httpClient.GetAsync(uriBuilder.Uri, cancellationToken);
	}
}
