using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EventManagementAPI.Tests
{
    public class FakeSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();
        public IEnumerable<string> Keys => _store.Keys;
        public string Id => System.Guid.NewGuid().ToString();
        public bool IsAvailable => true;
        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);
    }

    public static class TestSessionExtensions
    {
        public static void SetInt32(this ISession session, string key, int value)
            => session.Set(key, System.BitConverter.GetBytes(value));
        public static int? GetInt32(this ISession session, string key)
            => session.TryGetValue(key, out var data) ? System.BitConverter.ToInt32(data, 0) : null;
        public static void SetString(this ISession session, string key, string value)
            => session.Set(key, System.Text.Encoding.UTF8.GetBytes(value));
        public static string? GetString(this ISession session, string key)
            => session.TryGetValue(key, out var data) ? System.Text.Encoding.UTF8.GetString(data) : null;
    }
}
