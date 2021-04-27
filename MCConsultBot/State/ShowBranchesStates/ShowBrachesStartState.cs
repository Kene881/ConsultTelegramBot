using MCConsultBot.Database.Repositories;
using MCConsultBot.Telegram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCConsultBot.State.ShowBranchesStates
{
    class ShowBrachesStartState : ICurrentState
    {
        private CityRepositories _cityRepositories = new CityRepositories();
        private IEnumerable<string> GetCities()
        {
            return _cityRepositories.GetCitiesNames().ToArray();
        }
        public void PrerenderDefaultOutput(
            long chatId, 
            TelegramBotClientFacade telegramBotClientFacade)
        {
            telegramBotClientFacade.SendButtonMessageToChat(chatId, GetCities());
        }

        public void ProcessInput(
            long chatId, 
            string input, 
            TelegramBotClientFacade telegramBotClientFacade, 
            CurrentStateHolder stateHolder)
        {
            var selectedCity = GetCities()
                .FirstOrDefault(p => p.Equals(input, StringComparison.InvariantCultureIgnoreCase));

            if(selectedCity == null)
            {
                telegramBotClientFacade
                    .SendTextMessageToChat(chatId, "Пожалуйста, повторите попытку");

                telegramBotClientFacade
                    .SendButtonMessageToChat(chatId, GetCities());
            }
            else
            {
                var nextState = GetSelectAddressState(selectedCity);
                nextState.PrerenderDefaultOutput(chatId, telegramBotClientFacade);
                stateHolder.SetNextState(nextState);
            }
        }

        private ICurrentState GetSelectAddressState(string selectedCity)
        {
            return new ShowBranchAddressesInCityState(selectedCity);
        }
    }
}
