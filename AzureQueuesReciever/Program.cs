using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using BookLib;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace RecieverApp
{
    class Program
    {

        const string ServiceBusConnectionString = "Endpoint=sb://servicebuspractice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=44Xtdc5DTF2dnGjGiRYnYAVfyPVCW3ZVM6boz8Ty/gE=";
        const string QueueName = "practicequeue";
        static IQueueClient queueClient;
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();

        }

        static async Task MainAsync()
        {

            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after receiving all the messages.");

            RegisterOnMessageHandleAndRecieveMessages();

            Console.ReadKey();

            await queueClient.CloseAsync();


        }

        static void RegisterOnMessageHandleAndRecieveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)



        {
            //Converting the message from queue to Json 
            string bookJson = Encoding.UTF8.GetString(message.Body);

            //Converting / Deserializing our json to the object of Book class we have
            Books mybook = JsonConvert.DeserializeObject<Books>(bookJson);

            //Converting the Json to XML format and getting on console
            
            XmlDocument xmlBook =(XmlDocument)JsonConvert.DeserializeXmlNode(bookJson, "root");

            // Ignore: Console.WriteLine($"Recieved Messages: SequenceNumber { message.SystemProperties.SequenceNumber} Body: { Encoding.UTF8.GetString(message.Body)}");


            //Getting output of all types on Console
            Console.WriteLine($"Recieved Messages in Json form: { bookJson }");

            Console.WriteLine($"Recieved Messages in XML form: { XDocument.Parse(xmlBook.InnerXml) }");

            Console.WriteLine($"Recieved Messages in Object form: { mybook } where the id is {mybook.Id} and name is {mybook.Name}");


            

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        // Use this handler to examine the exceptions received on the message pump.
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

    }
}