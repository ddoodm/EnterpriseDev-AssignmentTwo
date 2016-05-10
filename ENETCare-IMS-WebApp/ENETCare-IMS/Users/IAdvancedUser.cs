using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Users
{
    public interface IAdvancedUser
    {
        District District { get; }
        decimal MaxApprovableLabour { get; }
        decimal MaxApprovableCost { get;}
        string Title { get; }
        string Name { get; }
    }
}
