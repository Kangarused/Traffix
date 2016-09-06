using System.IO;
using System.Web.Hosting;

namespace Traffix.Common.Utils.EmbeddedResourcesUtils
{
    public class EmbeddedVirtualFile : VirtualFile
    {
        private readonly Stream _stream;

        public EmbeddedVirtualFile(string virtualPath, Stream stream)
            : base(virtualPath)
        {
            _stream = stream;
        }

        public override Stream Open()
        {
            return _stream;
        }
    } 
}
