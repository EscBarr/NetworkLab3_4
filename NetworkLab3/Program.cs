//See https://aka.ms/new-console-template for more information
using NetworkLab3;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

int BeginPort = 25565;
int EndPort = 25566;
string IP = "127.0.0.1";
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
        Console.WriteLine("Порт " + i.ToString() + " закрыт");
    }
    else
    {
        soc.Close();
        Console.WriteLine("Порт " + i.ToString() + " открыт");
    }
}

static void ConnectCallback(IAsyncResult ar)
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

void ScanNetwork()
{
    Ping ping;
    IPAddress iP;
    PingReply pingReply;
    IPHostEntry iPHost;
    string ip = "192.168.1.";
    string name;
    Parallel.For(0, 254, (i, loopState) =>
    {
        ping = new Ping();
        iP = IPAddress.Parse(ip + i.ToString());
        pingReply = ping.Send(iP);
        if (pingReply.Status == IPStatus.Success)
        {
            try
            {
                iPHost = Dns.GetHostEntry(ip);
                string name = iPHost.HostName;
            }
            catch (Exception ex)
            {
                name = "Unresolved";
            }
        }
    });
}

//SocketPortScaner mainWindow = new SocketPortScaner();