using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PortScanner 
{
  public partial class Form1: Form 
  {
    public Form1() 
    {
      InitializeComponent();
    }

    private async void ScanButton_Click(object sender, EventArgs e) 
    {
      if (!IPAddress.TryParse(IpAddressTextBox.Text, out IPAddress ipAddress)) 
      {
        MessageBox.Show("Invalid IP address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (!int.TryParse(PortFromTextBox.Text, out int portFrom) || portFrom < 1 || portFrom > 65535) 
      {
        MessageBox.Show("Invalid port number (from).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (!int.TryParse(PortToTextBox.Text, out int portTo) || portTo < 1 || portTo > 65535 || portTo < portFrom) 
      {
        MessageBox.Show("Invalid port number (to).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      string openPorts = "";
      
      await Task.Run(() =>
        {
          for (int port = portFrom; port <= portTo; port++) 
          {
            using(TcpClient tcpClient = new TcpClient()) 
            {
              try 
              {
                Task connectTask = tcpClient.ConnectAsync(ipAddress, port);

                if (Task.WhenAny(connectTask, Task.Delay(500)).Result == connectTask && tcpClient.Connected)
                {
                  openPorts += port + ", ";
                  listBox1.Invoke((MethodInvoker)(() => 
                  {
                    listBox1.Items.Add(port.ToString());
                  }));
                }
              } 

              catch {}
            }
          }
        });
        
      if (openPorts.Length > 0) 
      {
        openPorts = openPorts.Substring(0, openPorts.Length - 2);
        MessageBox.Show("Open ports: " + openPorts, "Scan results", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } 
      else 
      {
        MessageBox.Show("No open ports found.", "Scan results", MessageBoxButtons.OK,
          MessageBoxIcon.Information);
      }
    }
  }
}