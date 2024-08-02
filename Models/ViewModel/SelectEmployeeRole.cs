using lab6.DataAccess;
using lab6.Models.ViewModel;
using System.Collections.Generic;

namespace lab6.Models.ViewModel
{
    public class SelectEmployeeRole
    {
        public Employee Employee { get; set; }
        public List<RoleSelection> Roles { get; set; }
    }
}
