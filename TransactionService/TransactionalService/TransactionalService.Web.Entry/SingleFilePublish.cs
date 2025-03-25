using System.Reflection;
using Furion;

namespace TransactionalService.Web.Entry
{
    public class SingleFilePublish : ISingleFilePublish
    {
        public Assembly[] IncludeAssemblies()
        {
            return Array.Empty<Assembly>();
        }

        public string[] IncludeAssemblyNames()
        {
            return new[]
            {
                "TransactionalService.Application",
                "TransactionalService.Core",
                "TransactionalService.EntityFramework.Core",
                "TransactionalService.Web.Core"
            };
        }
    }
}