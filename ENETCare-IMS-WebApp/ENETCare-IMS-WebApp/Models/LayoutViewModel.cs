using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ENETCare.IMS.WebApp.Models
{
    public class NavbarItems : IEnumerable<NavbarItems.NavbarItem>
    {
        public ICollection<NavbarItem> Items { get; private set; }

        public NavbarItems(params NavbarItem[] items)
        {
            this.Items = items;
        }
        
        public struct NavbarItem
        {
            public string Title { get; set; }
            public string BootstrapIcon { get; set; }
            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public bool Active { get; set; }
        }

        public IEnumerator<NavbarItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}