using System.Net;
using FluentFTP;

namespace Client__ftp
{
    public partial class Form1 : Form
    {
        FtpClient client = new FtpClient("", "", "");

        public Form1()
        {
            InitializeComponent();
        }

        public void Connect(string host, NetworkCredential credential)
        {
            client!.Host = host;
            client.Credentials = credential;
            client.AutoConnect();
            client.SetWorkingDirectory("./");
        }

        public void Disconnect()
        {
            client!.Disconnect();
        }

        public void ShowLog()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + client.Host);

            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
            request.Credentials = new NetworkCredential(client.Credentials.UserName,
                client.Credentials.Password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();

            string messageBack;

            messageBack = response.StatusDescription;
            string[] split = messageBack.Split(' ');
            richTextBox1.AppendText("Client: Connected to " + client.Host + Environment.NewLine);
            richTextBox1.AppendText("Client: Sending command \"PWD\"" + Environment.NewLine);
            richTextBox1.AppendText(string.Format("Server: {0}", response.StatusDescription));

            response.Close();

            richTextBox1.AppendText("Client: Sending command \"LIST\"" + Environment.NewLine);
        }


        public void ShowDir()
        {
            var listing = client.GetListing("./");

            foreach (var item in listing)
            {
                switch (item.Type)
                {

                    case FtpObjectType.Directory:

                        richTextBox2.AppendText("Directory!  " + item.FullName + Environment.NewLine);
                        richTextBox2.AppendText("Modified date:  " + client.GetModifiedTime(item.FullName) + Environment.NewLine);

                        break;

                    case FtpObjectType.File:

                        richTextBox2.AppendText("File!  " + item.FullName);
                        richTextBox2.AppendText("File size:  " + client.GetFileSize(item.FullName) + Environment.NewLine);
                        richTextBox2.AppendText("Modified date:  " + client.GetModifiedTime(item.FullName) + Environment.NewLine);
                        richTextBox2.AppendText("Chmod:  " + client.GetChmod(item.FullName) + Environment.NewLine);

                        break;

                    case FtpObjectType.Link:
                        break;
                }
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Connect(Host_text.Text, new NetworkCredential(log_text.Text, pass_text.Text));
            ShowLog();
            ShowDir();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (client.IsConnected)
            {
                Disconnect();
                richTextBox1.AppendText("Client log out" + Environment.NewLine);
            }

        }


    }
}