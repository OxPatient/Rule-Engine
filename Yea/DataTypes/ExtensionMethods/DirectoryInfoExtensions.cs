#region Usings

using System.IO;
using System.Reflection;
using Yea.Reflection;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    public static class DirectoryInfoExtensions
    {
        public static void LoadAssemblies(this DirectoryInfo directory, bool recursive = false)
        {
            var searchOption = recursive
                                   ? SearchOption.AllDirectories
                                   : SearchOption.TopDirectoryOnly;

            foreach (var fileInfo in directory.GetFiles("*.dll", searchOption))
            {
                AssemblyName.GetAssemblyName(fileInfo.FullName).Load();
            }
        }
    }
}