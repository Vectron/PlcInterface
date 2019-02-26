using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlcInterface
{
    public interface IReadWrite
    {
        IDictionary<string, object> Read(IEnumerable<string> ioNames);

        object Read(string ioName);

        T Read<T>(string ioName);

        Task<IDictionary<string, object>> ReadAsync(IEnumerable<string> ioNames);

        Task<object> ReadAsync(string ioName);

        Task<T> ReadAsync<T>(string ioName);

        dynamic ReadDynamic(string ioName);

        Task<dynamic> ReadDynamicAsync(string ioName);

        void ToggleBool(string ioName);

        void Write(IDictionary<string, object> namesValues);

        void Write<T>(string ioName, T value);

        Task WriteAsync(IDictionary<string, object> namesValues);

        Task WriteAsync<T>(string ioName, T value);
    }
}