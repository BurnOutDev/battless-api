using System.Text.Json;
using Api.Controllers;
using Application;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationMiddleware.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ClientConfigurationController : BaseController
    {
        private IClientConfigurationService _clientConfigurationService;

        public ClientConfigurationController(IClientConfigurationService clientConfigurationService)
        {
            _clientConfigurationService = clientConfigurationService;
        }

        [AllowAnonymous]
        [HttpGet("[action]/{code}")]
        public IActionResult ConfigurationByCode(string code)
        {
            var result = _clientConfigurationService.GetConfigurationByCode(code);

            return Ok(result);
        }

        [HttpPost("[action]")]
        public IActionResult AddClientApplication(ClientApplicationJson clientApplication)
        {
            _clientConfigurationService.AddClientApplication(clientApplication);

            return Ok();
        }

        [HttpPost("[action]/{code}")]
        public IActionResult EditConfigurationByCode(string code, JsonElement configuration)
        {
            _clientConfigurationService.EditConfigurationByCode(code, configuration);

            return Ok();
        }

        [HttpPost("[action]/{code}")]
        public IActionResult DeleteClientApplication(string code)
        {
            _clientConfigurationService.DeleteClientApplication(code);

            return Ok();
        }

        [HttpGet("[action]")]
        public IActionResult ClientApplications()
        {
            var result = _clientConfigurationService.ListClientApplications();

            return Ok(result);
        }
    }
}
