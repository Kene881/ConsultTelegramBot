using MCConsultBot.Database.Repositories;
using MCConsultBot.Infrastructure;
using MCConsultBot.PubSub;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace MCConsultBot
{


    #region Observer Demo 

    // ConcreteObserver
    //public class EventToConsoleLoger : ISubscriber
    //{
    //    private readonly ConsoleColor _consoleColor;
    //    public EventToConsoleLoger(ConsoleColor color)
    //    {
    //        _consoleColor = color;
    //    }
    //    public void Update()
    //    {
    //        Console.ForegroundColor = _consoleColor;
    //        Console.WriteLine($"Произошло событие в {DateTime.Now}");
    //        Console.ResetColor();
    //    }
    //}

    //public class Publisher
    //{
    //    private readonly ICollection<ISubscriber> _subscribers;
    //    private readonly Random _random;
    //    public Publisher()
    //    {
    //        _subscribers = new List<ISubscriber>();
    //        _random = new Random();
    //    }

    //    public void AddSubscriber(ISubscriber subscriber)
    //    {
    //        _subscribers.Add(subscriber);
    //    }

    //    public void RemoveSubscriber(ISubscriber subscriber)
    //    {
    //        var find = _subscribers.FirstOrDefault(p => p == subscriber);
    //        if(find != null)
    //        {
    //            _subscribers.Remove(find);
    //        }
    //    }

    //    public void NotifySubscribers()
    //    {
    //        foreach (var subscriber in _subscribers)
    //        {
    //            subscriber.Update();
    //        }
    //    }
    //    public void Run()
    //    {
    //        while (true)
    //        {
    //            NotifySubscribers();
    //            Thread.Sleep(TimeSpan.FromSeconds(5));
    //        }
    //    }
    //}

    public class PublisherDemo
    {
        public PublisherDemo()
        {
            //var subscribers = new ISubscriber[] 
            //{ 
            //    new EventToConsoleLoger(ConsoleColor.Red), 
            //    new EventToConsoleLoger(ConsoleColor.Green),
            //    new EventToConsoleLoger(ConsoleColor.Blue) 
            //};

            //var publisher = new Publisher();

            //publisher.AddSubscriber(subscribers[0]);
            //publisher.AddSubscriber(subscribers[1]);
            //publisher.AddSubscriber(subscribers[2]);

            //var x = Task.Run(() => { publisher.Run(); });

            //while (true)
            //{
            //    var nextCommand = Console.ReadLine();
            //    if(nextCommand == "remove")
            //    {
            //        publisher.RemoveSubscriber(subscribers[2]);
            //    }
            //}
        }
    }
    #endregion


    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var telegramBotToken = "1661815286:AAEYiXdV5hOlnpcSIDla8kgEwI0qubGp_0Q";
            var telegramBotClient = new TelegramBotClient(telegramBotToken);
            var facade = new Telegram.TelegramBotClientFacade(telegramBotClient);

            telegramBotClient.OnMessage += Bot_OnMessage;

            var eventRepository = new EventRepository();

            eventRepository.AddScheduleToEvent(
                SpecialDiscountEventPublisher.GetInstance(),
                TimeSpan.FromSeconds(15));

            eventRepository.RunWorker(facade);
            telegramBotClient.StartReceiving();
            
            Console.ReadLine();
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs args)
        {
            var processingMiddleware = new ProcessingMiddlewareBuilder()
                .AddTelegramClient((ITelegramBotClient)sender)
                .AddComponent(new IncomingMessageLogger())
                .AddComponent(new IncomingMessageThrottler(InMemoryRateLimiter.GetInstance()))
                .AddComponent(new MessageProcessingLogic(InMemoryStateRepository.GetInstance()))
                .Build();

            processingMiddleware.ProcessRequest(args);

            await Task.CompletedTask;
        }
    }
}
