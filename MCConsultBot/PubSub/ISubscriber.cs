using MCConsultBot.Telegram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot;

namespace MCConsultBot.PubSub
{
    abstract class BaseSubscriber
    {
        public long ChatId { get; private set; }
        public BaseSubscriber(long chatId)
        {
            ChatId = chatId;
        }
        
        public abstract void Update(TelegramBotClientFacade facade);
    }

    class SpecialDiscountEventSubscriber : BaseSubscriber
    {
        public SpecialDiscountEventSubscriber(long chatId) : base(chatId) { }

        public override void Update(TelegramBotClientFacade facade)
        {
            var message = 
                "Уважаемый клиент! Рады сообщить, что в нашем медцентре" +
                " начинается неделя скидок - приведи друга и " +
                "забери его почку бесплатно!";

            facade.SendTextMessageToChat(ChatId, message);
        }
    }


    /// <summary>
    /// Событие - запуск скидок в медцентре
    /// </summary>
    class SpecialDiscountEventPublisher : IPublisher
    {
        private readonly ICollection<BaseSubscriber> _subscribers;

        private static SpecialDiscountEventPublisher _instnace;
        private SpecialDiscountEventPublisher()
        {
            _subscribers = new List<BaseSubscriber>();
        }

        public static IPublisher GetInstance()
        {
            if(_instnace == null)
            {
                _instnace = new SpecialDiscountEventPublisher();
            }

            return _instnace;
        }

        public void PublishEvent(TelegramBotClientFacade telegramBotClientFacade)
        {
            foreach (var item in _subscribers)
            {
                item.Update(telegramBotClientFacade);
            }
        }

        public void AddSubscriber(BaseSubscriber subscriber)
        {
            var alreadyExist = _instnace._subscribers
                .Any(p => p.ChatId == subscriber.ChatId);

            if (!alreadyExist)
            {
                _instnace._subscribers.Add(subscriber);
            }
        }

        public void RemoveSubscriber(BaseSubscriber subscriber)
        {
            var get = _instnace._subscribers
                .FirstOrDefault(p => p.ChatId == subscriber.ChatId);

            if (get != null)
            {
                _instnace._subscribers.Remove(get);
            }
        }
    }
}
