using System.Web.Optimization;

namespace Traffix.Common.Utils
{
    public class TemplateBundle : Bundle
    {
        public TemplateBundle(string moduleName, string virtualPath)
            : base(virtualPath, new[] { new TemplateTransform(moduleName) })
        {
        }
    }
}