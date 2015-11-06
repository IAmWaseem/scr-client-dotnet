using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SCR
{
	public class SocketHandler
	{
		private IPEndPoint address;
		private int port;
		private Socket socket;
		private bool verbose;

		public SocketHandler(string host, int port, bool verbose)
		{
			this.address = new IPEndPoint(Dns.GetHostEntry(host).AddressList[0], port);
			this.port = port;
			this.verbose = verbose;
			this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			//socket = new UdpClient();
			//socket.ExclusiveAddressUse = false;
			//socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			//socket.Client.Bind(address);
		}

		public void Send(string msg)
		{
			try
			{
				var data = Encoding.ASCII.GetBytes(msg);
				socket.SendTo(data, data.Length, SocketFlags.None, (EndPoint)address);
			}
			catch (Exception ex)
			{
				// Error
			}
		}

		public string Receive()
		{
			try
			{
				EndPoint ep = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
				var data = new byte[1024];
				socket.ReceiveFrom(data, ref ep);
				return Encoding.ASCII.GetString(data);
			}
			catch(Exception ex)
			{
				// Error
			}
			return null;
		}

		public string Receive(int timeout)
		{
			try
			{
				EndPoint ep = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
				var data = new byte[1024];
				socket.ReceiveTimeout = timeout;
				socket.ReceiveFrom(data, ref ep);
				socket.ReceiveTimeout = 0;
				return Encoding.ASCII.GetString(data);
			}
			catch (Exception ex)
			{
				// Error
			}
			return null;
		}

		public void Close()
		{
			socket.Close();
		}
	}
}
