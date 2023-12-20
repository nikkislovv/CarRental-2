using Elasticsearch.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nest;
using System.Net;

namespace CatalogApi.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/{v:apiversion}/health")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly IElasticClient _elasticClient;
        public HealthController(
            HealthCheckService healthCheckService,
            IElasticClient elasticClient)
        {
            _healthCheckService = healthCheckService;
            _elasticClient = elasticClient;
        }

        [HttpGet("MsSqlHealth")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CheckMsSqlHealth()
        {
            HealthReport report = await _healthCheckService.CheckHealthAsync();
            var result = new
            {
                status = report.Status.ToString(),
                errors = report.Entries
                .Select(e => new { name = e.Key, status = e.Value.Status.ToString(), description = e.Value.Description.ToString() })
            };
            return report.Status == HealthStatus.Healthy
                ? Ok(result)
                : StatusCode((int)HttpStatusCode.ServiceUnavailable, result);
        }

        [HttpGet("ElasticsearchHealth")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CheckElasticsearchHealth()
        {
            WaitForStatus waitForStatus = WaitForStatus.Red;
            var response = await _elasticClient.Cluster.HealthAsync(new ClusterHealthRequest() { WaitForStatus = waitForStatus });
            var healthColor = response.Status.ToString().ToLowerInvariant();

            // this will give you the color code of the elastic search server health.
            if (response.IsValid)
            {
                switch (healthColor)
                {
                    case "green":
                        return Ok(new { message = "ElasticSearch Server is healthy" });

                    case "yellow":
                        return Ok(new { message = "ElasticSearch Server is healthy, ElasticSearch Server health check returned [YELLOW] (yellow is normal for single node clusters) " });

                    default: // Includes "red"
                        return Ok(new { message = $"ElasticSearch Server is healthy, ElasticSearch Server health check returned[{response.Status.ToString().ToUpper()}]" });
                }
            }
            else
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, response.OriginalException.Message);
        }

    }
}
