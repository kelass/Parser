using Newtonsoft.Json;
using Prysukha.Parser.Domain.Models;
using Prysukha.Parser.Infrastructure.Interfaces;

namespace Prysukha.Parser.Infrastructure.Services
{
    public class JsonSaver : IJsonSaver
    {
        private readonly string _currentDirectory = Directory.GetCurrentDirectory();
        public async Task SaveToJson(Item item)
        {
            string path = Path.Combine(_currentDirectory, "parsedItems.json");
            if (await IsEqual(item.ItemName, path))
            {
                Dictionary<string, Item> data = JsonConvert.DeserializeObject<Dictionary<string, Item>>(await File.ReadAllTextAsync(path))!;
                data.Add(item.ItemName, item);
                await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(data, Formatting.Indented));
            }
        }

        private async Task<bool> IsEqual(string containerName, string jsonPath)
        {
            string json = await File.ReadAllTextAsync(jsonPath);
            Dictionary<string, Item> items = JsonConvert.DeserializeObject<Dictionary<string, Item>>(json)!;

            if (items.ContainsKey(containerName))
                return false;

            return true;
        }
    }
}
