using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Helpers;

public static class PermissionHelper
{
    public static Dictionary<string, List<string>> GetAllPermissions()
    {
        var result = new Dictionary<string, List<string>>();
        var nestedTypes = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
        foreach (var moduleType in nestedTypes)
        {
            var allField = moduleType.GetField("All", BindingFlags.Public | BindingFlags.Static);
            if (allField != null)
            {
                var permission = allField.GetValue(null) as List<string>;
                if (permission != null)
                {
                    result.Add(moduleType.Name, permission);
                }
            }
        }
        return result;

    }
    public static List<string> GetPermissionList()
    {
        return GetAllPermissions().SelectMany(x => x.Value).ToList();
    }

}
