﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ENETCare.IMS
{
    public class Client
    {
        [Key]
        public int ID               { get; private set; }

        [Required]
        public string Name          { get; private set; }

        [Required]
        public District District    { get; private set; }

        [Required]
        public string Location      { get; private set; }

        public string DescriptiveName
        {
            get
            {
                string format = "{0} ({1}, {2})";
                return String.Format(format, Name, Location, District);
            }
        }

        public Client(string name, string location, District district)
        {
            this.Name = name;
            this.Location = location;
            this.District = district;
        }
    }
}
