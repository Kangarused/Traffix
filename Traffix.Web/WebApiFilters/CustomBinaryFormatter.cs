using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Traffix.Common.Providers;
using Newtonsoft.Json;

namespace Traffix.Web.WebApiFilters
{
    public class CustomBinaryFormatter : MediaTypeFormatter
    {
        private readonly ICryptoProvider _cryptoProvider;

        public CustomBinaryFormatter(Func<ICryptoProvider> cryptoServiceFactory)
        {
            _cryptoProvider = cryptoServiceFactory.Invoke();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("traffix/binary"));
        }

        public override bool CanReadType(Type type)
        {

            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            using (var ms = new MemoryStream())
            {
                readStream.CopyTo(ms);
                var decoded = _cryptoProvider.DecodeMessage(ms.ToArray());
                object result = JsonConvert.DeserializeObject(decoded, type);
                return Task.FromResult(result);
            }
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            var obj = JsonConvert.SerializeObject(value);
            var encoded = _cryptoProvider.EncodeMessage(obj);
            var writer = new BinaryWriter(writeStream);
            writer.Write(encoded);
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }
    }
}