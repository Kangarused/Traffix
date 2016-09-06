using System.Threading.Tasks;
using Traffix.Common.IocAttributes;

namespace Traffix.Common.Providers
{
    public interface IDatabaseVersionReader
    {
        Task<long> GetDatabaseVersionAsync();
    }

    public interface IVersionProvider
    {
        string BuildVersion { get; }
        Task<long> GetDatabaseVersionAsync();
    }

    [Singleton]
    public class VersionProvider : IVersionProvider
    {
        private readonly IDatabaseVersionReader _databaseVersionReader;
        public string BuildVersion { get; internal set; }
        Task<long> IVersionProvider.GetDatabaseVersionAsync()
        {
            return _databaseVersionReader.GetDatabaseVersionAsync();
        }

        public VersionProvider(IDatabaseVersionReader databaseVersionReader, IWebAssemblyResolver assemblyResolver)
        {
            _databaseVersionReader = databaseVersionReader;
            BuildVersion = assemblyResolver.WebAssembly.GetName().Version.ToString();
        }
    }
}