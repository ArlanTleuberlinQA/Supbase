using System.Text.Json.Serialization;
namespace Supbase.Posts
{
    public class SupbasePost

    {
        [JsonPropertyName("user")]

        public User User { get; set; }

        [JsonPropertyName("access_token")]

        public string Access_token { get; set; }

        [JsonPropertyName("id")]
        public string Transaction_id { get; set; }

        [JsonPropertyName("from_user")]
        public string Sender_id { get; set; }

        [JsonPropertyName("to_user")]
        public string To_user { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }


    }
 
public class User

    {

        [JsonPropertyName("id")]

        public string Id { get; set; }
        
        [JsonPropertyName("email")]
        public string Email {get; set; }

    }
}