﻿using Microsoft.Owin;
using Owin;
using System;
using System.IO;

[assembly: OwinStartupAttribute(typeof(ENETCare_IMS_WebApp.Startup))]
namespace ENETCare_IMS_WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureDataDirectoryPath();
            ConfigureAuth(app);
        }

        /// <summary>
        /// Sets the path of "DataDirectory" to the
        /// absolute path of the data directory
        /// within the Data project.
        /// </summary>
        private void ConfigureDataDirectoryPath()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string immediateDirectoryName = Path.GetFileName(Path.GetDirectoryName(appDirectory));

            string path = Path.GetFullPath(Path.Combine(
                appDirectory, @"..\ENETCare-IMS-Data\"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }
    }
}
