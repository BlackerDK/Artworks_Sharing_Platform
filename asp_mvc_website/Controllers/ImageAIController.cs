using asp_mvc_website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;

namespace asp_mvc_website.Controllers
{
    public class ImageAIController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> loadAI(PostArtworkModel model)
        {
            string base64Image = await LoadImage(model.File);

            if (base64Image == null)
            {

                return BadRequest("Failed to load image.");
            }
            string questionText = "Dựa vào bức ảnh này hãy miêu tả cho bức ảnh";
            string result = await ProcessImage(base64Image, questionText);
            var responseData = new
            {
                answer = result
            };
            return Json(responseData);
        }

        private async Task<string> ProcessImage(string base64Image, string questionText)
        {
            string jsonRequest = @"{
					""contents"":[
						{
							""parts"":[
								{""text"": """ + questionText + @"""},
								{
									""inline_data"": {
										""mime_type"":""image/jpeg"",
										""data"": """ + base64Image + @"""
									}
								}
							]
						}
					]
				}";

           // string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro-vision:generateContent?key=";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.PostAsync(apiUrl, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();


                    JObject jsonResponse = JObject.Parse(responseBody);


                    JArray candidates = (JArray)jsonResponse["candidates"];

  
                    foreach (JToken candidate in candidates)
                    {
                        JToken content = candidate["content"];
                        if (content != null)
                        {
                            JArray parts = (JArray)content["parts"];
                            if (parts != null && parts.Count > 0)
                            {
                                foreach (JToken part in parts)
                                {
                                    string text = (string)part["text"];
                                    if (text != null)
                                    {
                                        return text;
                                    }
                                }
                            }
                        }
                    }
                    return "No result found";
                }
                catch (Exception ex)
                {
                    return "An error occurred: " + ex.Message;
                }
            }
        }

        private async Task<string> LoadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        byte[] imageData = ms.ToArray();
                        string base64Image = Convert.ToBase64String(imageData);

                        return base64Image;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("File", "An error occurred while processing the file: " + ex.Message);
                    return null;
                }
            }
            else
            {
                ModelState.AddModelError("File", "Please select a file to upload.");
                return null;
            }
        }
    }
}
