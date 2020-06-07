using IdentityModel.Client;
using System;
using System.Net.Http;

namespace TestConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckIdentityServer();

            Console.ReadKey();
        }

        static async void CheckIdentityServer()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "PictureWhisperClient",
                ClientSecret = "SnVzdFJla2kgU29mdHdhcmU=",
                Scope = "PictureWhisperWebAPI"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine("Bearer " + tokenResponse.AccessToken);

            // call api
            //var clientApi = new HttpClient();
            //clientApi.SetBearerToken(tokenResponse.AccessToken);

            //var response = await clientApi.GetAsync("https://localhost:5001/api/download/picture/small/default/avatar.png");
            //if (!response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(response.StatusCode);
            //}
            //var content = await response.Content.ReadAsStreamAsync();
            //var image = Image.FromStream(content);
            //var dirctory = Path.Combine(Directory.GetCurrentDirectory(), "image");
            //if (!Directory.Exists(dirctory))
            //{
            //    Directory.CreateDirectory(dirctory);
            //}
            //var path = Path.Combine(Directory.GetCurrentDirectory(), "image/1.png");
            //image.Save(path);

            //var bytes = await File.ReadAllBytesAsync(@"C:\Users\Vnshng\Pictures\avatar\67714300_p0.png");
            //MultipartFormDataContent form = new MultipartFormDataContent();
            //HttpContent content = new StringContent("fileToUpload");
            //form.Add(content, "fileToUpload");
            //content = new ByteArrayContent(bytes);
            //content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            //{
            //    Name = "fileToUpload",
            //    FileName = "67714300_p0.png"
            //};
            //form.Add(content);
            //var response = await clientApi.PostAsync("https://localhost:5001/api/upload/picture/1/wallpaper", form);
            //if (!response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(response.StatusCode);
            //}
            //else
            //{
            //    var responseContent = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(JObject.Parse(responseContent));
            //}

            //var response = await clientApi.GetAsync("https://localhost:5001/api/user");
            //if (!response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(response.StatusCode);
            //}
            //else
            //{
            //    var content = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(JObject.Parse(content));
            //}
        }
    }
}
