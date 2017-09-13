using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Scanner {
	class ScannerImages {
		static string host = "192.168.0.101";
		static int port = 50003;
		static string outputPath = "output.jpg";
		static string command = "F?";
		static void Main(string[] args) {
			bool error = false;
			Image output;
			int imageSize;
			byte[] responseBuffer = new byte[100000];
			byte[] commandBuffer = System.Text.Encoding.ASCII.GetBytes(command + "\r\n");
			Socket scannerSocket = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);
			Console.WriteLine("Connecting to " + host + ":" + port + "...");
			try {
				scannerSocket.Connect(host, port);
			}
			catch(SocketException) {
				Console.WriteLine("Connection failed.");
				error = true;
			}
			if(!error) {
				Console.WriteLine("Connection established.");
				scannerSocket.Send(commandBuffer);
				scannerSocket.Receive(responseBuffer);
				int.TryParse(System.Text.Encoding.ASCII.GetString(responseBuffer.Take(9).ToArray()), out imageSize);
				output = Image.FromStream(new MemoryStream(responseBuffer.Skip(9).Take(imageSize).ToArray()));
				output.Save(outputPath, ImageFormat.Jpeg);
			}
			scannerSocket.Disconnect(true);
			Console.WriteLine("Press any key to exit.");
			Console.ReadLine();
		}
		
	}
}
