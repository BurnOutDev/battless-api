using System.Text.Json;

namespace Domain.Models
{
    public class ClientApplicationJson
    {
        public string DisplayName { get; set; }
        public string Code { get; set; }
        public JsonElement? Configuration { get; set; }
    }
}
