using LadeviVentasApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LadeviVentasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public TestingController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _configuration = configuration;
        }

        [HttpPost("seed-data")]
        public async Task<IActionResult> SeedTestData([FromBody] object testData = null)
        {
            // Protecciones
            if (!_env.IsEnvironment("Testing"))
                return NotFound();

            var testToken = Request.Headers["X-Test-Token"].FirstOrDefault();
            var expectedToken = _configuration["IntegrationTestSeedToken"];

            if (string.IsNullOrEmpty(testToken) || testToken != expectedToken)
                return Unauthorized("Invalid test token");

            try
            {
                await SeedMinimalTestData();
                return Ok(new { message = "Test data seeded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            if (!_env.IsEnvironment("Testing"))
                return NotFound();

            return Ok(new
            {
                status = "healthy",
                environment = _env.EnvironmentName,
                timestamp = DateTime.UtcNow
            });
        }

        private async Task SeedMinimalTestData()
        {
            // Solo agregar datos mínimos necesarios para que los tests funcionen

            // 1. Verificar que existe el usuario de testing
            var testUserExists = await _context.ApplicationUsers
                .AnyAsync(u => u.CredentialsUser.Email == "gpribi@admin.com");

            if (!testUserExists)
            {
                Console.WriteLine("Warning: Test user 'gpribi@admin.com' not found");
                // Aquí podrías crear el usuario si es necesario
            }

            // 2. Agregar cualquier configuración básica que necesiten los tests
            // Por ejemplo: países por defecto, roles, etc.

            await _context.SaveChangesAsync();
        }
    }
}