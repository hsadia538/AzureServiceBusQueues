using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;


namespace SenderApp
{
    class Program
    {

        const string ServiceBusConnectionString = "Endpoint=sb://servicebuspractice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=44Xtdc5DTF2dnGjGiRYnYAVfyPVCW3ZVM6boz8Ty/gE=";
        const string QueueName = "PracticeQueue";
        static IQueueClient queueClient;

        static async Task Main(string[] args)
        {


            const int numberOfMessages = 10;

            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

           
            Console.WriteLine("Press ENTER key to exit after sending all the messages.");
            

            await SendMessageAsync(numberOfMessages);

            Console.ReadKey();

            await queueClient.CloseAsync();

        }

        static async Task SendMessageAsync( int numberOfMessagesSent)
        {


            try
            {
                for ( var i=0; i < numberOfMessagesSent; i++)
                {
                    string messageBody = $"message {i}";

                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));


                    Console.WriteLine($"Sending message : {messageBody}");

                    await queueClient.SendAsync(message);
                }

            }

            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {e.Message}");


            }
        }

    }
}
