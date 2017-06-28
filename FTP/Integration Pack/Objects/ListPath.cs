﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SystemCenter.Orchestrator.Integration;
using FtpLib;

namespace SCORCHDev_FTP.Objects
{
    [Activity("List Path")]
    public class ListPath : IActivity
    {
        private ConnectionCredentials settings;
        private String userName = String.Empty;
        private String password = String.Empty;
        private String SCCMServer = String.Empty;

        private int ObjCount = 0;

        [ActivityConfiguration]
        public ConnectionCredentials Settings
        {
            get { return settings; }
            set { settings = value; }
        }
        public void Design(IActivityDesigner designer)
        {
            designer.AddInput("Source File Path");
            designer.AddInput("Destination File Path").WithFileBrowser().WithDefaultValue("\\myfileserver\fileShare");
            designer.AddInput("Overwrite Local File").WithListBrowser(new string[] { "True", "False" }).WithDefaultValue("True");
        }
        public void Execute(IActivityRequest request, IActivityResponse response)
        {
            String sourcePath = request.Inputs["Source File Path"].AsString();
            String savePath = request.Inputs["Destination File Path"].AsString();
            bool overwriteLocal = Convert.ToBoolean(request.Inputs["Overwrite Local File"].AsString());

            using (FtpConnection ftp = new FtpConnection(settings.FtpServer, settings.Port, settings.UserName, settings.Password))
            {
                ftp.Open();
                ftp.Login();

                if (ftp.FileExists(sourcePath))
                {
                    ftp.GetFile(sourcePath, savePath, overwriteLocal);
                }
                else
                {
                    response.LogErrorMessage("File does not exist at " + sourcePath);
                }
            }
        }
    }
}
