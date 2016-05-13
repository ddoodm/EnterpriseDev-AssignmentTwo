﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Data.DataAccess
{
    public class DistrictRepo : GenericRepo<District>
    {
        public static DistrictRepo New
        {
            get { return new DistrictRepo(); }
        }

        public Districts AllDistricts
        {
            get
            {
                using (var db = new EnetCareDbContext())
                {
                    return new Districts(db.Districts.ToList<District>());
                }
            }
        }
    }
}
