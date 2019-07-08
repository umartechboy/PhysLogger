using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace UpdateServer
{
    public partial class UpdateClient : Form
    {
        public UpdateClient()
        {
            InitializeComponent();
        }

        FTP ftpClient;
        private void button1_Click(object sender, EventArgs e)
        {
            ftpClient = new FTP("ftp.drivehq.com", "umartechboy3", "W1nbow5*b");
            File.WriteAllText("testfile.txt", "Test Content");
            ftpClient.createDirectory("FivePointNineUpdateServer");
            ftpClient.createDirectory("FivePointNineUpdateServer/PhysLogger");
            ftpClient.upload
            var list = ftpClient.directoryListDetailed("");

        }

        private void analyzeB_Click(object sender, EventArgs e)
        {
            string dir = @"C:\Users\umar.hassan\GoogleDrive\LUMS\PhysLogger\PhysLogger\bin";
            string wd = @"proj\bin";
            //var source = DirectoryItem.Scan(dir).Flatten();
            //var target = DirectoryItem.Scan(wd).Flatten();
            //var coms = new List<UpdaterCommand>();
            //foreach (var fe in source)
            //{
            //    if (target.Find(fet =>
            //        fet.MD5 == fe.MD5
            //        && fet.GetType() == fe.GetType()
            //        && fet.RelativePath == fe.RelativePath) == null)
            //    {
            //        if (fe is FileItem)
            //        coms.Add(new UpdateOrCopyCommand(fe));
            //    }
            //}
            //foreach (var fe in source)
            //{
            //    if (target.Find(fet =>
            //        fet.GetType() == fe.GetType()
            //        && fet.RelativePath == fe.RelativePath) == null)
            //    {
            //        if (fe is DirectoryItem)
            //            coms.Add(new UpdateOrCopyCommand(fe));
            //    }
            //}
            //foreach (var fet in target)
            //{
            //    if (source.Find(fe =>
            //        fet.GetType() == fe.GetType()
            //        && fet.RelativePath == fe.RelativePath) == null)
            //        coms.Add(new DeleteCommand(fet));
            //}
        }
    }
}
