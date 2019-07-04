﻿using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Keycloak.Net.Tests
{
    public partial class KeycloakClientShould
    {
        [Theory]
        [InlineData("Insurance")]
        public async Task GetUsersAsync(string realm)
        {
            var result = await _client.GetUsersAsync(realm);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData("Insurance")]
        public async Task GetUsersCountAsync(string realm)
        {
            int result = await _client.GetUsersCountAsync(realm);
            Assert.True(result >= 0);
        }

        [Theory]
        [InlineData("Insurance")]
        public async Task GetUserAsync(string realm)
        {
            var users = await _client.GetUsersAsync(realm);
            string userId = users.FirstOrDefault()?.Id;

            var result = await _client.GetUserAsync(realm, userId);
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Theory]
        [InlineData("Insurance", "vermeulen")]
        public async Task GetUserGroupsAsync(string realm, string search)
        {
            var users = await _client.GetUsersAsync(realm, search: search);
            string userId = users.FirstOrDefault()?.Id;

            var result = await _client.GetUserGroupsAsync(realm, userId);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("Insurance", "vermeulen")]
        public async Task GetUserGroupsCountAsync(string realm, string search)
        {
            var users = await _client.GetUsersAsync(realm, search: search);
            string userId = users.FirstOrDefault()?.Id;

            int result = await _client.GetUserGroupsCountAsync(realm, userId);
            Assert.True(result >= 0);
        }
    }
}