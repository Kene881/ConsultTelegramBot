using MCConsultBot.Database.Models;
using MCConsultBot.Database.Repositories;
using MCConsultBot.PubSub;
using MCConsultBot.State.ShowBranchesStates;
using MCConsultBot.Telegram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCConsultBot.State
{
    interface ICurrentState
    {
        /// <summary>
        /// Пользователь всегда присылает некий ввод. В зависимости от нашего
        /// состояния, мы по разному будем обрабатывать этот ввод.
        /// </summary>
        void ProcessInput(
            long chatId,
            string input,
            TelegramBotClientFacade telegramBotClientFacade,
            CurrentStateHolder stateHolder);

        void PrerenderDefaultOutput(
            long chatId,
            TelegramBotClientFacade telegramBotClientFacade);
    }


    /// <summary>
    /// Начальное окно выбора - список возможных операции
    /// </summary>
    class OnStartSelectState : ICurrentState
    {
        public void PrerenderDefaultOutput(
            long chatId,
            TelegramBotClientFacade telegramBotClientFacade)
        {
            telegramBotClientFacade
                    .SendButtonMessageToChat(chatId, GetButtons());
        }

        public void ProcessInput(
            long chatId,
            string input,
            TelegramBotClientFacade telegramBotClientFacade,
            CurrentStateHolder stateHolder)
        {
            var selectedButton = GetButtons()
                .FirstOrDefault(p => input
                    .Equals(p, StringComparison.InvariantCultureIgnoreCase));

            if (selectedButton == null)
            {
                telegramBotClientFacade
                    .SendButtonMessageToChat(chatId, GetButtons());
            }
            else
            {
                var nextState = ButtonToNextStateMapper(selectedButton);
                nextState.PrerenderDefaultOutput(chatId, telegramBotClientFacade);
                stateHolder.SetNextState(nextState);
            }
        }

        private ICurrentState ButtonToNextStateMapper(string button)
        {
            switch (button)
            {
                case "Показать расписание врача":
                    return new ShowDoctorScheduleState();
                case "Показать кабинет врача":
                    return new ShowDoctorRoomState();
                case "Показать филиалы медцентра":
                    return new ShowBrachesStartState();
                case "Подписаться на акции и скидки":
                    return new SubscribeToDiscountsState();
                case "Показать стоимость приема врача":
                    return new ShowDoctorPriceState();
                default:
                    return new OnStartSelectState();
            }
        }

        private IEnumerable<string> GetButtons()
        {
            return new string[]
            {
                "Показать расписание врача",
                "Показать стоимость приема врача",
                "Показать кабинет врача",
                "Показать филиалы медцентра",
                "Подписаться на акции и скидки"
            };
        }
    }

    class ShowDoctorPriceState : ICurrentState
    {
        private DoctorsRepository _doctorsRepository = new DoctorsRepository();
        private string GetPrice(string doctorName)
        {
            List<string> dNames = (List<string>)_doctorsRepository.GetDoctorsName();
            string price = "";

            if (dNames.Contains(doctorName))
            {
                price = _doctorsRepository.GetDoctorPriceByName(doctorName);
            }

            return price;
        }

        private IEnumerable<string> GetDoctorsList()
        {
            return _doctorsRepository.GetDoctorsName().ToArray();
        }

        public void ProcessInput(long chatId,
            string input,
            TelegramBotClientFacade telegramBotClientFacade,
            CurrentStateHolder stateHolder)
        {
            var doctorName = input;
            var price = GetPrice(doctorName);

            telegramBotClientFacade.SendTextMessageToChat(chatId, price);

            var nextState = new OnStartSelectState();
            nextState.PrerenderDefaultOutput(chatId, telegramBotClientFacade);
            stateHolder.SetNextState(nextState);
        }

        public void PrerenderDefaultOutput(
            long chatId,
            TelegramBotClientFacade telegramBotClientFacade)
        {
            telegramBotClientFacade.SendButtonMessageToChat(chatId, GetDoctorsList());
        }
    }

    class ShowDoctorRoomState : ICurrentState
    {
        private DoctorsRepository _doctorsRepository = new DoctorsRepository();
        //БД
        private string GetRoom(string doctorName)
        {
            //var dictionary = new Dictionary<string, string>();
            //dictionary["Осипова Татьяна"] = "232";
            //dictionary["Амангельдиев Оспан"] = "236";
            //dictionary["Виталий Ким"] = "105";

            //return dictionary.ContainsKey(doctorName) ? dictionary[doctorName] : "Не найден врач!";
            List<string> dNames = (List<string>)_doctorsRepository.GetDoctorsName();
            string RoomNumber = "";

            if (dNames.Contains(doctorName))
            {
                RoomNumber = _doctorsRepository.GetDoctorRoomByName(doctorName);
            }

            return RoomNumber;
        }

        private IEnumerable<string> GetDoctorsList()
        {
            return _doctorsRepository.GetDoctorsName().ToArray();
        }

        public void PrerenderDefaultOutput(
            long chatId,
            TelegramBotClientFacade telegramBotClientFacade)
        {
            telegramBotClientFacade.SendButtonMessageToChat(chatId, GetDoctorsList());
        }

        public void ProcessInput(long chatId, string input, TelegramBotClientFacade telegramBotClientFacade, CurrentStateHolder stateHolder)
        {
            var doctorName = input;
            var room = GetRoom(doctorName);

            telegramBotClientFacade.SendTextMessageToChat(chatId, room);

            var nextState = new OnStartSelectState();
            nextState.PrerenderDefaultOutput(chatId, telegramBotClientFacade);
            stateHolder.SetNextState(nextState);
        }
    }

    /// <summary>
    /// Показать расписание врача
    /// </summary>
    class ShowDoctorScheduleState : ICurrentState
    {
        private DoctorsRepository _doctorsRepository = new DoctorsRepository();
        private string GetSchedule(string doctorName)
        {
            List<string> dNames = (List<string>)_doctorsRepository.GetDoctorsName();
            string schedule = "";

            if (dNames.Contains(doctorName))
            {
                schedule = _doctorsRepository.GetDoctorScheduleByName(doctorName);
            }

            return schedule;
        }

        private IEnumerable<string> GetDoctorsList()
        {
            return _doctorsRepository.GetDoctorsName().ToArray();
        }

        public void ProcessInput(long chatId,
            string input,
            TelegramBotClientFacade telegramBotClientFacade,
            CurrentStateHolder stateHolder)
        {
            var doctorName = input;
            var schedule = GetSchedule(doctorName);

            telegramBotClientFacade.SendTextMessageToChat(chatId, schedule);

            var nextState = new OnStartSelectState();
            nextState.PrerenderDefaultOutput(chatId, telegramBotClientFacade);
            stateHolder.SetNextState(nextState);
        }

        public void PrerenderDefaultOutput(
            long chatId,
            TelegramBotClientFacade telegramBotClientFacade)
        {
            telegramBotClientFacade.SendButtonMessageToChat(chatId, GetDoctorsList());
        }
    }
}
