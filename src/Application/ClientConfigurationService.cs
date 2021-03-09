using AutoMapper;
using Domain;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application
{
    public class ClientConfigurationService : IClientConfigurationService
    {
        private GamblingDbContext _context;
        private IMapper _mapper;

        public ClientConfigurationService(GamblingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public JsonResponse<JsonElement?> GetConfigurationByCode(string code)
        {
            var result = new JsonResponse<JsonElement?>(success: false);

            var clientApp = _context.ClientApplications
                .Where(ca => ca.Code == code)
                .FirstOrDefault();

            if (clientApp != null)
            {
                result.Data = JsonSerializer.Deserialize<JsonElement>(clientApp.Configuration);
                result.Success = true;
            }
            else
            {
                result.Message = "Client application not found.";
            }

            return result;
        }

        public void AddClientApplication(ClientApplicationJson clientApplication)
        {
            var clientApp = _mapper.Map<ClientApplication>(clientApplication);

            _context.Add(clientApp);
            _context.SaveChanges();
        }

        public void EditConfigurationByCode(string code, JsonElement configuration)
        {
            var clientApp = _context.ClientApplications
               .Where(ca => ca.Code == code)
               .FirstOrDefault();

            clientApp.Configuration = configuration.ToString();

            _context.Update(clientApp);
            _context.SaveChanges();
        }

        public JsonResponse<ICollection<ClientApplicationJson>> ListClientApplications()
        {
            var result = new JsonResponse<ICollection<ClientApplicationJson>>(false);

            var apps = _context.ClientApplications.ToList().ToArray();

            var appsJson = _mapper.Map<ClientApplication[], ICollection<ClientApplicationJson>>(apps);

            result.Success = true;
            result.Data = appsJson;

            return result;
        }

        public void DeleteClientApplication(string code)
        {
            var app = _context.ClientApplications.Where(x => x.Code == code).FirstOrDefault();

            _context.ClientApplications.Remove(app);
            _context.SaveChanges();
        }
    }
}
