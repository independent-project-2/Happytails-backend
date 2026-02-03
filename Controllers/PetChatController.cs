using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/chat")]
public class PetChatController : ControllerBase
{
    private readonly IConfiguration _config;

    public PetChatController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("pet")]
    public async Task<IActionResult> ChatAboutPets([FromBody] ChatRequest request)
    {
        var apiKey = _config["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            return BadRequest("API key not configured");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", apiKey);

        var body = new
        {
            messages = new[]
            {
                new { author = "system", content = "You are a friendly pet care assistant for the HappyTail app. Only answer questions about pets, animals, pet health tips, stray animal care, and safety." },
                new { author = "user", content = request.Message }
            }
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var endpoint = "https://api.generativeai.google/v1beta2/models/gemini-1.5/chat";

        var response = await client.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, error);
        }

        var resultJson = await response.Content.ReadAsStringAsync();

        try
        {
            var doc = JsonDocument.Parse(resultJson);
            var reply = doc.RootElement
                           .GetProperty("candidates")[0]
                           .GetProperty("content")
                           .GetString();

            return Ok(new { Reply = reply });
        }
        catch
        {
            // fallback if parsing fails
            return Ok(resultJson);
        }
    }
}

public class ChatRequest
{
    public string Message { get; set; }
}
