using System;
using System.Text;
using System.Threading.Tasks;
using BookLib;
using Microsoft.Azure.ServiceBus;
using Json.Net;
using Newtonsoft.Json;

namespace SenderApp
{
    class Program
    {

        const string ServiceBusConnectionString = "Endpoint=sb://servicebuspractice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=44Xtdc5DTF2dnGjGiRYnYAVfyPVCW3ZVM6boz8Ty/gE=";
        const string QueueName = "PracticeQueue";
        static IQueueClient queueClient;

        static async Task Main(string[] args)

        {

            string id;
            string message;

            Console.WriteLine("Enter random id");
            id = Console.ReadLine();

            int ID = int.Parse(id);

            Console.WriteLine("Enter name of book");
            message = Console.ReadLine();

            //making a book object to be converted in JSON
            Books mybook = new Books(message, ID);

            string bookJson = JsonConvert.SerializeObject(mybook);

            //const int numberOfMessages = 10;

            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);


            Console.WriteLine("Press ENTER key to exit after sending all the messages.");


            await SendMessageAsync(bookJson);

            Console.ReadKey();

            await queueClient.CloseAsync();

        }

        static async Task SendMessageAsync(string jsonToSend)
        {


            try
            {



                var message = new Message(Encoding.UTF8.GetBytes(jsonToSend));


                Console.WriteLine($"Sending message : {jsonToSend}");

                await queueClient.SendAsync(message);


            }

            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {e.Message}");


            }
        }

    }
}
