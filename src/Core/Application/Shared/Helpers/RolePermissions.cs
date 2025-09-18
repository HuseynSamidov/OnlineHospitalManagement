using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Helpers;

public static class RolePermissions
{
    public static readonly Dictionary<Roles, List<string>> Mapping = new()
    {
        // Admin hər şeyə icazəlidir
        { Roles.Admin, PermissionHelper.GetPermissionList() },

        // Doctor — bütün ticketləri görə, detallara baxa, update edə bilər
        { Roles.Doctor, new List<string>
            {
                Permissions.QueueTicket.GetAll,
                Permissions.QueueTicket.GetDetail,
                Permissions.QueueTicket.Update
            }
        },

        // Patient — yalnız öz ticketlərini yarada, görə və silə bilər
        { Roles.Patient, new List<string>
            {
                Permissions.QueueTicket.Create,
                Permissions.QueueTicket.GetMy,
                Permissions.QueueTicket.Delete
            }
        }
    };
}
