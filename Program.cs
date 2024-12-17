// See https://aka.ms/new-console-template for more information


using System.Net.Sockets;
using System.Net;
using System.Text;

int port = 2323;
string directoryPath = @"D:\Учеба\4 курс\java тренировочные проекты\фото";
TcpListener listener = new TcpListener(IPAddress.Any, port);
listener.Start();

Console.WriteLine("Сервер запущен на порту " + port);

while (true)
{
    using (TcpClient client = listener.AcceptTcpClient())
    {
        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream);
        
        writer.AutoFlush = true;
        string request = reader.ReadLine();

        if (!string.IsNullOrEmpty(request))
        {
            string[] requestParts = request.Split(' ');
            string httpMeth = requestParts[0];
            string uri = requestParts[1];

            if (httpMeth == "GET")
            {
                if (uri.EndsWith("/"))
                {
                    string[] files = Directory.GetFiles(directoryPath);

                    StringBuilder html = new StringBuilder();

                    string name = Path.GetFileName(directoryPath);


                    html.Append("HTTP/1.1 200 OK\r\n");
                    html.Append("Content-Type: text/html\r\n\r\n");
                    html.Append("<html><head><meta charset=\"utf-8\"></head><body style=background-color:green>");
                    html.Append($"<h1>Содержимое каталога с названием {name}: </h1>");
                    html.Append("<ul>");

                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        string fileUrl = $"/{fileName}";
                        html.Append($"<li style=\"font-size: 30px;\"><a href=\"{fileUrl}\">{fileName}</a></li>");
                    }

                    html.Append("</ul></body></html>");
                    byte[] buffer = Encoding.Default.GetBytes(html.ToString());
                    writer.Write(html.ToString());
                }
                else
                {
                    string fileName = Path.GetFileName(uri);
                    string filePath = Path.Combine(directoryPath, fileName);

                    if (File.Exists(filePath))
                    {
                        string contentType;
                        if (fileName.EndsWith(".html"))
                            contentType = "html";
                        else
                            contentType = "text/plain";

                        byte[] buffer = File.ReadAllBytes(filePath);

                        writer.Write("HTTP/1.1 200 OK\r\n");
                        writer.Write($"Content-Type: {contentType}\r\n");
                        writer.Write($"Content-Length: {buffer.Length}\r\n\r\n");
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    else
                        writer.Write("HTTP/1.1 404 Not Found\r\n\r\n");
                }
            }
        }
    }
}
