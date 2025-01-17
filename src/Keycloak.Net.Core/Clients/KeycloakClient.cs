﻿using Keycloak.Net.Core.Models.Root;
using Keycloak.Net.Models.Clients;
using Keycloak.Net.Models.ClientScopes;
using Keycloak.Net.Models.Common;
using Keycloak.Net.Models.Roles;
using Keycloak.Net.Models.Users;
using System;
using System.Net.Http;

namespace Keycloak.Net;

public partial class KeycloakClient
{
	public async Task<bool> CreateClientAsync(string realm,
											  Client client,
											  CancellationToken cancellationToken = default)
	{
		var response = await InternalCreateClientAsync(realm, client, cancellationToken: cancellationToken).ConfigureAwait(false);

		return response.IsSuccessStatusCode;
	}

	public async Task<string?> CreateClientAndRetrieveClientIdAsync(string realm,
																	Client client,
																	CancellationToken cancellationToken = default)
	{
		var response = await InternalCreateClientAsync(realm, client, cancellationToken: cancellationToken).ConfigureAwait(false);

		var locationPathAndQuery = response.Headers.Location!.PathAndQuery;
		var clientId = response.IsSuccessStatusCode
						   ? locationPathAndQuery[(locationPathAndQuery.LastIndexOf("/", StringComparison.Ordinal) + 1)..]
						   : null;
		return clientId;
	}

	private async Task<HttpResponseMessage> InternalCreateClientAsync(string realm,
																	  Client client,
																	  CancellationToken cancellationToken) =>
		(await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients")
								.PostJsonAsync(client, cancellationToken: cancellationToken)
								.ConfigureAwait(false)).ResponseMessage;

	public async Task<IEnumerable<Client>> GetClientsAsync(string realm,
														   string? clientId = null,
														   int? first = null,
														   int? max = null,
														   string? q = null,
														   bool? search = null,
														   bool? viewableOnly = null,
														   CancellationToken cancellationToken = default)
	{
		var queryParams = new Dictionary<string, object?>
		{
			[nameof(clientId)] = clientId,
			[nameof(first)] = first,
			[nameof(max)] = max,
			[nameof(q)] = q,
			[nameof(search)] = search,
			[nameof(viewableOnly)] = viewableOnly
		};

		return await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients")
									  .SetQueryParams(queryParams)
									  .GetJsonAsync<IEnumerable<Client>>(cancellationToken: cancellationToken)
									  .ConfigureAwait(false);
	}

	public async Task<Client> GetClientAsync(string realm,
											 string clientId,
											 CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}")
							   .GetJsonAsync<Client>(cancellationToken: cancellationToken)
							   .ConfigureAwait(false);

	public async Task<bool> UpdateClientAsync(string realm,
											  string clientId,
											  Client client,
											  CancellationToken cancellationToken = default)
	{
		var response = await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}")
											  .PutJsonAsync(client, cancellationToken: cancellationToken)
											  .ConfigureAwait(false);
		return response.ResponseMessage.IsSuccessStatusCode;
	}

	public async Task<bool> DeleteClientAsync(string realm,
											  string clientId,
											  CancellationToken cancellationToken = default)
	{
		var response = await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}")
											  .DeleteAsync(cancellationToken: cancellationToken)
											  .ConfigureAwait(false);
		return response.ResponseMessage.IsSuccessStatusCode;
	}

	public async Task<Credentials> GenerateClientSecretAsync(string realm,
															 string clientId,
															 CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/client-secret")
							   .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
							   .ReceiveJson<Credentials>()
							   .ConfigureAwait(false);

	public async Task<Credentials> GetClientSecretAsync(string realm,
														string clientId,
														CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/client-secret")
							   .GetJsonAsync<Credentials>(cancellationToken: cancellationToken)
							   .ConfigureAwait(false);

	public async Task<IEnumerable<ClientScope>> GetDefaultClientScopesAsync(string realm,
																			string clientId,
																			CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/default-client-scopes")
							   .GetJsonAsync<IEnumerable<ClientScope>>(cancellationToken: cancellationToken)
							   .ConfigureAwait(false);

	public async Task<bool> UpdateDefaultClientScopeAsync(string realm,
														  string clientId,
														  string clientScopeId,
														  CancellationToken cancellationToken = default)
	{
		var response = await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/default-client-scopes/{clientScopeId}")
											  .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
											  .ConfigureAwait(false);
		return response.ResponseMessage.IsSuccessStatusCode;
	}

	public async Task<bool> DeleteDefaultClientScopeAsync(string realm,
														  string clientId,
														  string clientScopeId,
														  CancellationToken cancellationToken = default)
	{
		var response = await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/default-client-scopes/{clientScopeId}")
											  .DeleteAsync(cancellationToken: cancellationToken)
											  .ConfigureAwait(false);
		return response.ResponseMessage.IsSuccessStatusCode;
	}

	[Obsolete("Not working yet")]
	public async Task<AccessToken> GenerateClientExampleAccessTokenAsync(string realm,
																		 string clientId,
																		 string? scope = null,
																		 string? userId = null,
																		 CancellationToken cancellationToken = default)
	{
		var queryParams = new Dictionary<string, object?>
						  {
							  [nameof(scope)] = scope,
							  [nameof(userId)] = userId
						  };

		return await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/evaluate-scopes/generate-example-access-token")
									  .SetQueryParams(queryParams)
									  .GetJsonAsync<AccessToken>(cancellationToken: cancellationToken)
									  .ConfigureAwait(false);
	}

	public async Task<IEnumerable<ClientScopeEvaluateResourceProtocolMapperEvaluation>>
		GetProtocolMappersInTokenGenerationAsync(string realm,
												 string clientId,
												 string? scope = null,
												 CancellationToken cancellationToken = default)
	{
		var queryParams = new Dictionary<string, object?>
						  {
							  [nameof(scope)] = scope
						  };

		return await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/evaluate-scopes/protocol-mappers")
									  .SetQueryParams(queryParams)
									  .GetJsonAsync<IEnumerable<ClientScopeEvaluateResourceProtocolMapperEvaluation>>(cancellationToken: cancellationToken)
									  .ConfigureAwait(false);
	}

	public async Task<IEnumerable<Role>> GetClientGrantedScopeMappingsAsync(string realm,
																			string clientId,
																			string roleContainerId,
																			string? scope = null,
																			CancellationToken cancellationToken = default)
	{
		var queryParams = new Dictionary<string, object?>
						  {
							  [nameof(scope)] = scope
						  };

		return await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/evaluate-scopes/scope-mappings/{roleContainerId}/granted")
									  .SetQueryParams(queryParams)
									  .GetJsonAsync<IEnumerable<Role>>(cancellationToken: cancellationToken)
									  .ConfigureAwait(false);
	}

	public async Task<IEnumerable<Role>> GetClientNotGrantedScopeMappingsAsync(string realm,
																			   string clientId,
																			   string roleContainerId,
																			   string? scope = null,
																			   CancellationToken cancellationToken = default)
	{
		var queryParams = new Dictionary<string, object?>
						  {
							  [nameof(scope)] = scope
						  };

		return await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/evaluate-scopes/scope-mappings/{roleContainerId}/not-granted")
									  .SetQueryParams(queryParams)
									  .GetJsonAsync<IEnumerable<Role>>(cancellationToken: cancellationToken)
									  .ConfigureAwait(false);
	}

	public async Task<string> GetClientProviderAsync(string realm,
													 string clientId,
													 string providerId,
													 CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/installation/providers/{providerId}")
							   .GetStringAsync(cancellationToken: cancellationToken)
							   .ConfigureAwait(false);

	public async Task<ManagementPermission> GetClientAuthorizationPermissionsInitializedAsync(string realm,
																							  string clientId,
																							  CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/management/permissions")
							   .GetJsonAsync<ManagementPermission>(cancellationToken: cancellationToken)
							   .ConfigureAwait(false);

	public async Task<ManagementPermission> SetClientAuthorizationPermissionsInitializedAsync(string realm,
																							  string clientId,
																							  ManagementPermission managementPermission,
																							  CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/management/permissions")
							   .PutJsonAsync(managementPermission, cancellationToken: cancellationToken)
							   .ReceiveJson<ManagementPermission>()
							   .ConfigureAwait(false);

	public async Task<bool> RegisterClientClusterNodeAsync(string realm,
														   string clientId,
														   IDictionary<string, object> formParams,
														   CancellationToken cancellationToken = default)
	{
		var response = await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/nodes")
											  .PostJsonAsync(formParams, cancellationToken: cancellationToken)
											  .ConfigureAwait(false);
		return response.ResponseMessage.IsSuccessStatusCode;
	}

	public async Task<bool> UnregisterClientClusterNodeAsync(string realm,
															 string clientId,
															 CancellationToken cancellationToken = default)
	{
		var response = await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/nodes")
											  .DeleteAsync(cancellationToken: cancellationToken)
											  .ConfigureAwait(false);
		return response.ResponseMessage.IsSuccessStatusCode;
	}

	public async Task<int> GetClientOfflineSessionCountAsync(string realm,
															 string clientId,
															 CancellationToken cancellationToken = default)
	{
		var result = await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/offline-session-count")
											.GetJsonAsync<CountDto>(cancellationToken: cancellationToken)
											.ConfigureAwait(false);
		return result.Count;
	}

	public async Task<IEnumerable<UserSession>> GetClientOfflineSessionsAsync(string realm,
																			  string clientId,
																			  int? first = null,
																			  int? max = null,
																			  CancellationToken cancellationToken = default)
	{
		var queryParams = new Dictionary<string, object?>
						  {
							  [nameof(first)] = first,
							  [nameof(max)] = max
						  };

		return await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/offline-sessions")
									  .SetQueryParams(queryParams)
									  .GetJsonAsync<IEnumerable<UserSession>>(cancellationToken: cancellationToken)
									  .ConfigureAwait(false);
	}

	public async Task<IEnumerable<ClientScope>> GetOptionalClientScopesAsync(string realm,
																			 string clientId,
																			 CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/optional-client-scopes")
							   .GetJsonAsync<IEnumerable<ClientScope>>(cancellationToken: cancellationToken)
							   .ConfigureAwait(false);

	public async Task<bool> UpdateOptionalClientScopeAsync(string realm,
														   string clientId,
														   string clientScopeId,
														   CancellationToken cancellationToken = default)
	{
		var response = await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/optional-client-scopes/{clientScopeId}")
											  .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
											  .ConfigureAwait(false);
		return response.ResponseMessage.IsSuccessStatusCode;
	}

	public async Task<bool> DeleteOptionalClientScopeAsync(string realm,
														   string clientId,
														   string clientScopeId,
														   CancellationToken cancellationToken = default)
	{
		var response = await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/optional-client-scopes/{clientScopeId}")
											  .DeleteAsync(cancellationToken: cancellationToken)
											  .ConfigureAwait(false);
		return response.ResponseMessage.IsSuccessStatusCode;
	}

	public async Task<GlobalRequestResult> PushClientRevocationPolicyAsync(string realm,
																		   string clientId,
																		   CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/push-revocation")
							   .PostAsync(new StringContent(""), cancellationToken: cancellationToken)
							   .ReceiveJson<GlobalRequestResult>()
							   .ConfigureAwait(false);

	public async Task<Client> GenerateClientRegistrationAccessTokenAsync(string realm,
																		 string clientId,
																		 CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/registration-access-token")
							   .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
							   .ReceiveJson<Client>()
							   .ConfigureAwait(false);

	public async Task<User> GetUserForServiceAccountAsync(string realm,
														  string clientId,
														  CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/service-account-user")
							   .GetJsonAsync<User>(cancellationToken: cancellationToken)
							   .ConfigureAwait(false);

	public async Task<int> GetClientSessionCountAsync(string realm,
													  string clientId,
													  CancellationToken cancellationToken = default)
	{
		var result = await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/session-count")
											.GetJsonAsync<CountDto>(cancellationToken: cancellationToken)
											.ConfigureAwait(false);
		return result.Count;
	}

	public async Task<GlobalRequestResult> TestClientClusterNodesAvailableAsync(string realm,
																				string clientId,
																				CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/test-nodes-available")
							   .GetJsonAsync<GlobalRequestResult>(cancellationToken: cancellationToken)
							   .ConfigureAwait(false);

	public async Task<IEnumerable<UserSession>> GetClientUserSessionsAsync(string realm,
																		   string clientId,
																		   int? first = null,
																		   int? max = null,
																		   CancellationToken cancellationToken = default)
	{
		var queryParams = new Dictionary<string, object?>
						  {
							  [nameof(first)] = first,
							  [nameof(max)] = max
						  };

		return await GetBaseUrl(realm).AppendPathSegment($"/admin/realms/{realm}/clients/{clientId}/user-sessions")
									  .SetQueryParams(queryParams)
									  .GetJsonAsync<IEnumerable<UserSession>>(cancellationToken: cancellationToken)
									  .ConfigureAwait(false);
	}

	public async Task<IEnumerable<Resource>> GetResourcesOwnedByClientAsync(string realm,
																			string clientId,
																			CancellationToken cancellationToken = default) =>
		await GetBaseUrl(realm).AppendPathSegment($"/realms/{realm}/protocol/openid-connect/token")
							   .PostUrlEncodedAsync(new List<KeyValuePair<string, string>>
													{
														new("grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket"),
														new("response_mode", "permissions"),
														new("audience", clientId)
													},
													cancellationToken: cancellationToken)
							   .ReceiveJson<IEnumerable<Resource>>()
							   .ConfigureAwait(false);

    public async Task<Token> GetTokenExchangeResponseAsync(string realm,
                                                           string clientId,
                                                           string userId,
                                                           string clientSecret,
                                                           CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
		{
			{ "grant_type", "urn:ietf:params:oauth:grant-type:token-exchange" },
			{ "client_id", clientId },
			{ "client_secret", clientSecret },
			{ "requested_subject", userId },
			{ "scope", "openid" }
		};

        return await GetBaseUrl(realm)
            .AppendPathSegment($"/realms/{realm}/protocol/openid-connect/token")
            .PostUrlEncodedAsync(parameters, cancellationToken: cancellationToken)
            .ReceiveJson<Token>()
            .ConfigureAwait(false);
    }

    public async Task<Token> GetTokenWithResourceOwnerPasswordCredentialsAsync(string realm,
																			   string clientId,
																			   string username,
																			   string password,
																			   string clientSecret,
																			   CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
		{
			{ "grant_type", "password" },
			{ "client_id", clientId },
			{ "client_secret", clientSecret },
			{ "username", username },
			{ "password", password }
		};

        return await GetBaseUrl(realm)
            .AppendPathSegment($"/realms/{realm}/protocol/openid-connect/token")
            .PostUrlEncodedAsync(parameters, cancellationToken: cancellationToken)
            .ReceiveJson<Token>()
            .ConfigureAwait(false);
    }
}