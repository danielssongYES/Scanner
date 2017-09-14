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
		//Change as necessary for different readers on network
		static int inPort = 50003;
		static int outPort = 50002;
		//Documentation does not state unambiguously that the
		//device responds to image queries on a different port
		static string outputPath = "output.jpg";
		static string command = "F?";
		//Command for requesting last error image according to
		//documentation. May accept "I?" with better results
		static void Main(string[] args) {
			bool error = false;
			Image output;
			int imageSize;
			byte[] responseBuffer = new byte[102400];
			//Large 100k response buffer to store potentially expansive image data
			byte[] commandBuffer = System.Text.Encoding.ASCII.GetBytes(command + "\r\n");
			//Generate ASCII byte representation of command with terminating characters appended
			Socket inSocket = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);
			Socket outSocket = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);

			Console.WriteLine("Connecting to input port at " + host + ":" + inPort + "...");
			try {
				inSocket.Connect(host, inPort);
			}
			catch(SocketException) {
				Console.WriteLine("Connection to input port failed.");
				error = true;
			}
			Console.WriteLine("Connecting to output port at " + host + ":" + outPort + "...");
			try {
				outSocket.Connect(host, outPort);
			}
			catch(SocketException) {
				Console.WriteLine("Connection to output port failed.");
				error = true;
			}
			if(!error) {
				Console.WriteLine("Connection established.");
				inSocket.Send(commandBuffer);
				outSocket.Receive(responseBuffer);
				if(int.TryParse(System.Text.Encoding.ASCII.GetString(responseBuffer.Take(9).ToArray()), out imageSize)) {
					output = Image.FromStream(new MemoryStream(responseBuffer.Skip(9).Take(imageSize).ToArray()));
					output.Save(outputPath, ImageFormat.Jpeg);
					//In a valid response, the first nine bytes are an ASCII
					//character string representing the size of the image data
				}
				else
					Console.WriteLine("Invalid response from reader.");
			}
			if(inSocket.Connected) {
				inSocket.Disconnect(true);
				Console.WriteLine("Connection to input port closed.");
			}
			if(outSocket.Connected) {
				outSocket.Disconnect(true);
				Console.WriteLine("Connection to output port closed.");
			}
			Console.WriteLine("Press any key to exit.");
			Console.ReadLine();
		}
	}
}
