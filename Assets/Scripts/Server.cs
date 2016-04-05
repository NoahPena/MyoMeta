using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System;
using System.Threading;

using ISuckAtCoding;


public class Server
{
    public Thread sampleUDPThread;
    private const int sampleUdpPort = 4568;

    public Server()
    {
        try
        {
            Debug.Log("Please work");
            sampleUDPThread = new Thread(new ThreadStart(StartReceiveFrom2));
            sampleUDPThread.Start();
            Debug.Log("OMG IT WORKED");
        }
        catch (Exception)
        {
            sampleUDPThread.Abort();
            Debug.Log("back to jail");
        }
    }

    public static void StartReceiveFrom2()
    {
        IPHostEntry localHostEntry;
        try
        {
            //Create a UDP socket.
            Socket soUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                //localHostEntry = Dns.GetHostByName(Dns.GetHostName());
                //localHostEntry = Dns.GetHostByName("localhost");
                localHostEntry = Dns.GetHostEntry("localhost");
            }
            catch (Exception)
            {
                Console.WriteLine("Local Host not found"); // fail
                return;
            }
            IPEndPoint localIpEndPoint = new IPEndPoint(localHostEntry.AddressList[0], sampleUdpPort);
            soUdp.Bind(localIpEndPoint);
            Debug.Log("Starting Loop");
            //bool variable = true;
            while (true)
            {
                //  Debug.Log(0);
                Byte[] received = new Byte[256];
                //  Debug.Log(1);
                IPEndPoint tmpIpEndPoint = new IPEndPoint(localHostEntry.AddressList[0], sampleUdpPort);
                //   Debug.Log(2);
                EndPoint remoteEP = (tmpIpEndPoint);
                //   Debug.Log(3);
                int bytesReceived = soUdp.ReceiveFrom(received, ref remoteEP);
                //   Debug.Log(4);
                String dataReceived = System.Text.Encoding.ASCII.GetString(received);
                //   Debug.Log(5);
                Console.WriteLine("SampleClient is connected through UDP.");
                //    Debug.Log(6);
                Console.WriteLine(dataReceived);
                //    Debug.Log(7);
                parseData(dataReceived);
                //    Debug.Log(8);
            }
            //Debug.Log("Kids in the back seat cause accidents. Accidents in the back seat cause kids");
        }
        catch (SocketException se)
        {
            Console.WriteLine("A Socket Exception has occurred!" + se.ToString());
        }
    }

    public static void parseData(String data)
    {
        Debug.Log(data);
        try
        {
            string[] names = data.Split(new char[] { ' ' });

            GlobalVariables.inAir = Convert.ToBoolean(names[0]);
            GlobalVariables.pitch = (float)Convert.ToDouble(names[1]);
            GlobalVariables.roll = (float)Convert.ToDouble(names[2]);
            GlobalVariables.gaz = (float)Convert.ToDouble(names[3]);
            GlobalVariables.yaw = (float)Convert.ToDouble(names[4]);
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }
    }

}
