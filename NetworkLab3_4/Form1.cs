using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace NetworkLab3_4
{
    public partial class Form1 : Form
    {
        private string ip = "192.168.1.";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && numericUpDown1.Value <= numericUpDown2.Value)
            {
                listView1.Items.Clear();
                int BeginPort = Convert.ToInt32(numericUpDown1.Value);
                int EndPort = Convert.ToInt32(numericUpDown2.Value);
                string IP = listBox1.SelectedItem.ToString();
                IPAddress addr = IPAddress.Parse(IP);
                for (int i = BeginPort; i <= EndPort; i++)
                {
                    //Создаем и инициализируем сокет
                    IPEndPoint ep = new IPEndPoint(addr, i);
                    Socket soc = new Socket(AddressFamily.InterNetwork,
                                            SocketType.Stream,
                                            ProtocolType.Tcp);

                    //Пытаемся соединиться с сервером
                    IAsyncResult asyncResult = soc.BeginConnect(ep,
                                               new AsyncCallback(ConnectCallback),
                                               soc);

                    if (!asyncResult.AsyncWaitHandle.WaitOne(100, false))
                    {
                        soc.Close();
                        listView1.Items.Add("Порт " + i.ToString()).SubItems.Add(" закрыт");
                        listView1.Refresh();
                    }
                    else
                    {
                        soc.Close();
                        listView1.Items.Add("Порт " + i.ToString()).SubItems.Add(" открыт");
                        //listView1.Items[i].SubItems.Add(" открыт");
                    }
                }
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
            }
            catch (Exception e)
            {
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Thread.Sleep(500);

            Parallel.For(0, 254, (i, loopState) =>
            {
                var ping = new Ping();
                var iP = IPAddress.Parse(ip + i.ToString());
                var pingReply = ping.Send(iP);
                this.BeginInvoke((Action)delegate ()
                {
                    if (pingReply.Status == IPStatus.Success)
                    {
                        listBox1.Items.Add(iP.ToString());
                    }
                });
            });
            MessageBox.Show("Сканирование сети завершено");
        }
    }
}