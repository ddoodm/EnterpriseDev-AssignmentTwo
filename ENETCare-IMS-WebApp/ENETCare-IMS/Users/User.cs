﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ENETCare.IMS.Users
{
    public abstract class EnetCareUser : IEnetCareUser
    {
        [Key]
        public int ID { get; private set; }

        [Required]
        public string Name { get; private set; }

        /// <summary>
        /// The User's position (title), ie "Site Engineer"
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// The page to which the User is directed upon log-in
        /// </summary>
        public abstract string HomePage { get; }

        protected EnetCareUser() { }

        protected EnetCareUser(string name)
        {
            this.Name = name;
        }
    }
}