using Prysukha.Parser.Domain.Models;

namespace Prysukha.Parser.Infrastructure.Interfaces
{
    public interface IJsonSaver
    {
        Task SaveToJson(Item item);
    }
}
