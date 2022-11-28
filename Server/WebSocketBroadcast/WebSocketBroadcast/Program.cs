using System;
using System.Text.Json;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;


namespace WebSocketBroadcast
{
    public class CheckConnect : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            Send("OK");
            // ReSharper disable once FunctionNeverReturns
        }
    }
    
    class Program
    {
        static async Task Main(string[] args)
        {
            WebSocketServer wssv = new WebSocketServer("ws://localhost:8070");
            
            wssv.AddWebSocketService<CheckConnect>("/Echo");
            
            wssv.Start();

            bool shouldWork = true;

            while (shouldWork)
            {
                if (Console.ReadKey().Key == ConsoleKey.R)
                {
                    Console.WriteLine("Restart");
                    wssv.Stop();
                    await Task.Delay(10000);
                    Console.WriteLine("Restarted");
                    wssv.AddWebSocketService<CheckConnect>("/Echo");
                    wssv.Start();
                }
                
                if (Console.ReadKey().Key == ConsoleKey.E)
                {
                    shouldWork = false;
                    wssv.Stop();
                }

                if (Console.ReadKey().Key == ConsoleKey.P)
                {
                    wssv.Stop();
                }

                if (Console.ReadKey().Key == ConsoleKey.L)
                {
                    wssv.AddWebSocketService<CheckConnect>("/Echo");
                    wssv.Start();
                }
            }
            
        }
    }
}