using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Users
{
    /// <summary>
    /// A User who is associated with a District in some way
    /// </summary>
    public interface ILocalizedUser
    {
        District District { get; }
    }
}
