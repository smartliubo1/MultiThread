using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsyncHttp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Async Http!");

            var server = new AsyncHttpServer(123);
            var t = Task.Run(() => server.Start());

            Console.WriteLine("Listing on port 123");

            Console.WriteLine("Try to connect:");

            Console.WriteLine(  );

            GetResponseAsync("http://localhost:123/").GetAwaiter().GetResult();

            Console.WriteLine();
            Console.WriteLine("");
            Console.ReadLine();

            server.Stop().GetAwaiter().GetResult();
        }

        static async Task GetResponseAsync(string url)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await client.GetAsync(url);
                string responseHeaders = httpResponseMessage.Headers.ToString();
                string response = 
                    await httpResponseMessage.Content.ReadAsStringAsync();
                Console.WriteLine("Response headers");
                Console.WriteLine(responseHeaders);
                Console.WriteLine("Response body:");
                Console.WriteLine(response);
            }
        }

        class AsyncHttpServer
        {
            readonly HttpListener _listener;
            const string RESPONSE_TEMPLATE =
                "<html><head><title>Test</title></head>" +
                "<body><h2>Test page</h2>" +
                "<h4>Today is :{0}</h4>" +
                "</body></html>";
            public AsyncHttpServer(int portNumber)
            {
                _listener = new HttpListener();

                _listener.Prefixes.Add($"http://localhost:{portNumber}/");
            }

            public async Task Start()
            {
                _listener.Start();
                while (true)
                {
                    var context = await _listener.GetContextAsync();
                    Console.WriteLine("Client Connect....");
                    string response = string.Format(RESPONSE_TEMPLATE,DateTime.Now);

                    using (var sw=new StreamWriter(context.Response.OutputStream))
                    {
                        await sw.WriteAsync(response);
                        await sw.FlushAsync();
                    }
                }
            }

            public async Task Stop()
            {
                _listener.Abort();
            }
        }
    }
}
