using Domain.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace Application
{
    public interface IClientConfigurationService
    {
        void AddClientApplication(ClientApplicationJson clientApplication);
        void EditConfigurationByCode(string code, JsonElement configuration);
        JsonResponse<JsonElement?> GetConfigurationByCode(string code);
        JsonResponse<ICollection<ClientApplicationJson>> ListClientApplications();
        void DeleteClientApplication(string code);
    }
}