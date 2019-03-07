using System.Collections.Generic;
using System.Reflection;

namespace LT.Component.TypeFinder
{
    public interface ITypeFinder
    {
        List<Assembly> GetFilteredAssemblyList();
    }
}
