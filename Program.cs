using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;

public class Program
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task Main(string[] args)
    {
        var bspId = int.Parse(args[0]);
        var accessToken = args[1];
        var rootOutputFolder = @"Downloads";

        var outputFolder = Path.Combine(rootOutputFolder, bspId.ToString());
        Directory.CreateDirectory(outputFolder);

        var files = await GetFiles(bspId, accessToken);

        var filesToDownload = files.Where(x => x.Status == IataFileStatus.N).ToList();
        Console.WriteLine($"{filesToDownload.Count} files will be downloaded:");
        filesToDownload.ForEach(x => Console.WriteLine($"{x.Name} - {x.Id}"));
        Console.WriteLine();

        foreach (var file in filesToDownload)
        {
            Console.WriteLine($"Start downloading {file.Name} - {file.Id}");
            var bytes = await GetBytes(file.Id, accessToken);
            var path = Path.Combine(outputFolder, file.Name);
            await File.WriteAllBytesAsync(path, bytes);
            Console.WriteLine($"Downloaded {file.Name} - {file.Id} to {outputFolder}");
        }
    }
    private static async Task<List<IataFile>> GetFiles(int bspId, string accessToken)
    {
        var url = $"https://gateway-dmz.newbsplink.iata.org/api/file-management/files?bspId={bspId}&archived=false";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        var deserialized = JsonConvert.DeserializeObject<List<IataFile>>(body);
        return deserialized;
    }

    private static async Task<byte[]> GetBytes(string fileId, string accessToken)
    {
        var url = $"https://gateway-dmz.newbsplink.iata.org/api/file-management/files/{fileId}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsByteArrayAsync();
        return body;
    }
}
