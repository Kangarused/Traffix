using System;
using System.Collections;
using System.Web.Caching;
using System.Web.Hosting;

namespace Traffix.Common.Utils.EmbeddedResourcesUtils
{
    public class EmbeddedVirtualPathProvider : VirtualPathProvider
    {
        private readonly VirtualPathProvider _previous;

        public EmbeddedVirtualPathProvider(VirtualPathProvider previous)
        {
            _previous = previous;
        }

        public override bool FileExists(string virtualPath)
        {
            if (IsEmbeddedPath(virtualPath))
                return true;
            else
                return _previous.FileExists(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (IsEmbeddedPath(virtualPath))
            {
                return null;
            }
            else
            {
                return _previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return _previous.GetDirectory(virtualDir);
        }

        public override bool DirectoryExists(string virtualDir)
        {
            return _previous.DirectoryExists(virtualDir);
        }
        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsEmbeddedPath(virtualPath))
            {
                string fileNameWithExtension = virtualPath.Substring(virtualPath.LastIndexOf("/", StringComparison.Ordinal) + 1);
                string path = virtualPath.Replace("~/TraffixCommon/Embedded", "");
                var bits = path.Split('/');
                bits[2] = bits[2].Replace("-", "_");
                path = string.Join(".",bits);
                string nameSpace = typeof(EmbeddedResourceHttpHandler)
                                .Assembly
                                .GetName()
                                .Name;// Mostly the default namespace and assembly name are same
                string manifestResourceName = $"{nameSpace}.Content{path}";
                var stream = typeof(EmbeddedVirtualPathProvider).Assembly.GetManifestResourceStream(manifestResourceName);
                if (stream == null)
                {
                    throw new EmbeddedResourceException("Could not find embedded resource:" + manifestResourceName + ", check if the resource has the build action set to 'Embedded Resource'");
                }
                return new EmbeddedVirtualFile(virtualPath, stream);
            }
            else
                return _previous.GetFile(virtualPath);
        }

        private bool IsEmbeddedPath(string path)
        {
            return path.Contains("~/TraffixCommon/Embedded");
        }
    }
}
