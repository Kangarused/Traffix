using System;
using System.Text;
using System.Web.Optimization;

namespace Traffix.Common.Utils
{
    public class TemplateTransform : IBundleTransform
    {
        private readonly string _moduleName;
        public TemplateTransform(string moduleName)
        {
            _moduleName = moduleName;
        }

        public void Process(BundleContext context, BundleResponse response)
        {
            var strBundleResponse = new StringBuilder();
            // Javascript module for Angular that uses templateCache 
            strBundleResponse.AppendFormat(@"angular.module('{0}').run(['$templateCache',function(t){{",_moduleName);

            foreach (var file in response.Files)
            {
                // Get the partial page, remove line feeds and escape quotes
                var content = file.ApplyTransforms();
                content = content.Replace(Environment.NewLine, string.Empty);

                var templatePath = file.VirtualFile.VirtualPath;
                if (templatePath.StartsWith("/"))
                    templatePath = templatePath.Remove(0, 1);

                // Create insert statement with template
                strBundleResponse.AppendFormat(
                    @"t.put('{0}','{1}');", templatePath, content);
            }
            strBundleResponse.Append(@"}]);");

            response.Files = new BundleFile[] { };
            response.Content = strBundleResponse.ToString();
            response.ContentType = "text/javascript";
        }
    }
}