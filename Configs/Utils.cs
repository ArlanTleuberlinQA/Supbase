using System.Net;
using System.Text;
using System.Text.Json;

namespace Supabase.Configs.UniversalMethods {
  public static class UniversalMethods

  {
    public static string GenerateRandomEmail(string baseName, string domain = "gmail.com") {
      var random = new Random();
      int suffix = random.Next(10000, 99999);
      return $"{baseName.ToLower()}+{suffix}@{domain}";
    }
    public static string GenerateRandomPassword(int length = 12) {
      const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      const string lower = "abcdefghijklmnopqrstuvwxyz";
      const string digits = "0123456789";
      const string special = "!@#$%^&*()-_+=<>?";
      var allChars = upper + lower + digits + special;
      var random = new Random();
      var passwordChars = new List < char > {
        upper[random.Next(upper.Length)],
        lower[random.Next(lower.Length)],
        digits[random.Next(digits.Length)],
        special[random.Next(special.Length)]
      };
      for (int i = passwordChars.Count; i < length; i++) {
        passwordChars.Add(allChars[random.Next(allChars.Length)]);
      }

      return new string(passwordChars.OrderBy(x => random.Next()).ToArray());
    }
    public static string ApiKey => "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6IndmZXlwaGllcXdwbmtrcWd5dmNpIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDE4NTMwNDYsImV4cCI6MjA1NzQyOTA0Nn0.DiNBH49NzmtMNS51RI307D6IZiPL7CQiRfN46F8KePY";
    public static async Task < HttpResponseMessage > SendGetRequest(string url, HttpClient client) {
      client.DefaultRequestHeaders.Add("apikey", ApiKey);
      return await client.GetAsync(url);
    }

    public static async Task < HttpResponseMessage > SendPostRequest(string url, HttpClient client, object payload = null) {
      if (payload == null) {
        payload = new {
          email = GenerateRandomEmail("test"),
            password = GenerateRandomPassword()
        };
      }
      if (client.DefaultRequestHeaders.Contains("apikey"))
        client.DefaultRequestHeaders.Remove("apikey");
      client.DefaultRequestHeaders.Add("apikey", ApiKey);
      var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
      return await client.PostAsync(url, content);
    }
    public static async Task < HttpResponseMessage > SendPostTransactionRequest(string url, HttpClient client, object payload, string token) {
      if (client.DefaultRequestHeaders.Contains("apikey"))
        client.DefaultRequestHeaders.Remove("apikey");
      client.DefaultRequestHeaders.Add("apikey", ApiKey);

      if (client.DefaultRequestHeaders.Contains("Authorization"))
        client.DefaultRequestHeaders.Remove("Authorization");
      client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

      if (client.DefaultRequestHeaders.Contains("Prefer"))
        client.DefaultRequestHeaders.Remove("Prefer");
      client.DefaultRequestHeaders.Add("Prefer", "return=representation");

      var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
      return await client.PostAsync(url, content);
    }
    public static async Task < HttpResponseMessage > SendGetTransactionRequest(string url, HttpClient client, string token) {
      if (client.DefaultRequestHeaders.Contains("apikey"))
        client.DefaultRequestHeaders.Remove("apikey");
      client.DefaultRequestHeaders.Add("apikey", ApiKey);

      if (client.DefaultRequestHeaders.Contains("Authorization"))
        client.DefaultRequestHeaders.Remove("Authorization");
      client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

      if (client.DefaultRequestHeaders.Contains("Prefer"))
        client.DefaultRequestHeaders.Remove("Prefer");
      client.DefaultRequestHeaders.Add("Prefer", "return=representation");

      return await client.GetAsync(url);
    }
    public static async Task < HttpResponseMessage > SendInvalidPostRequest(string url, HttpClient client) {
      var payload = new

      {};
      var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
      return await client.PostAsync(url, content);
    }

    public static async Task < HttpResponseMessage > SendDeleteRequest(string url, HttpClient client) {
      return await client.DeleteAsync(url);
    }

    public static async Task < T > DeserializeSingleOrFirst < T > (HttpResponseMessage response) {
      var json = await response.Content.ReadAsStringAsync();
      var options = new JsonSerializerOptions {
        PropertyNameCaseInsensitive = true
      };

      try {
        var list = JsonSerializer.Deserialize < List < T >> (json, options);
        if (list != null && list.Count > 0)
          return list[0];
      } catch {}

      try {
        return JsonSerializer.Deserialize < T > (json, options);
      } catch (JsonException ex) {
        throw new Exception($"Не вдалося десеріалізувати: {ex.Message}\nJSON: {json}");
      }
    }
    public static bool IsStatusCodeOk(HttpResponseMessage response) {
      return response.StatusCode == HttpStatusCode.OK;
    }
    public static bool IsStatusCodeCreated(HttpResponseMessage response) {
      return response.StatusCode == HttpStatusCode.Created;
    }
    public static bool IsStatusCodeNotFound(HttpResponseMessage response) {
      return response.StatusCode == HttpStatusCode.NotFound;
    }
    public static bool IsStatusCodeBadRequest(HttpResponseMessage response) {
      return response.StatusCode == HttpStatusCode.BadRequest;
    }
    public static void LogOperationTimes(double createTime, double editTime, double deleteTime) {
      double totalTime = createTime + editTime + deleteTime;

      Console.WriteLine($"Create time: {createTime}ms");
      Console.WriteLine($"Edit time: {editTime}ms");
      Console.WriteLine($"Delete time: {deleteTime}ms");
      Console.WriteLine($"Total time: {totalTime}ms");
    }
  }

}