﻿using System.ComponentModel;
using System.Configuration.Install;

namespace WcfInstaller
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
