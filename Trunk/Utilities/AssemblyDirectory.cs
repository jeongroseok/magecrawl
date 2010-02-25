using System.IO;
using System.Reflection;

namespace Magecrawl.Utilities
{
    public static class AssemblyDirectory
    {
        public static string CurrentAssemblyDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }      
        }
    }
}
