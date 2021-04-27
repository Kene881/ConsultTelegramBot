using MCConsultBot.Database.Repositories;
using MCConsultBot.Telegram;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCConsultBot.State.ShowBranchesStates
{
    class ShowBranchAddressesInCityState : ICurrentState
    {
        private CityRepositories _cityRepositories = new CityRepositories();
        private string _selectedCity;
        public ShowBranchAddressesInCityState(string selectedCity)
        {
            _selectedCity = selectedCity;
        }

        private IEnumerable<string> GetAddressesByCity(string city)
        {
            //Алматы ул. Жарокова 137
            //var map = new Dictionary<string, IEnumerable<string>>();
            //map["Алматы"] = new string[] { "Жарокова 176", "Абая 98" };
            //map["Астана"] = new string[] { "Бокейхана 21" };
            //map["Караганда"] = new string[] { "Абая 11" };

            //return map[city];
            List<string> cNames = (List<string>)_cityRepositories.GetCitiesNames();
            string[] CityAddresses = null;

            if (cNames.Contains(city))
            {
                CityAddresses = (string[])_cityRepositories.GetAddressesByCity(city);
            }

            return CityAddresses;

        }


        public void PrerenderDefaultOutput(
            long chatId, 
            TelegramBotClientFacade telegramBotClientFacade)
        {
            telegramBotClientFacade.SendButtonMessageToChat(chatId, GetAddressesByCity(_selectedCity));
        }

        public void ProcessInput(
            long chatId, 
            string input, 
            TelegramBotClientFacade telegramBotClientFacade, 
            CurrentStateHolder stateHolder)
        {
            var addresses = (string[])GetAddressesByCity(_selectedCity);
            string address = "";

            for (int i = 0; i< addresses.Length; i++)
            {
                if (input == addresses[i])
                {
                    address = addresses[i];
                }
            }

            telegramBotClientFacade.SendMapMessageToChat(chatId, $"{_selectedCity} {address}");

            var initialState = new OnStartSelectState();
            initialState.PrerenderDefaultOutput(chatId, telegramBotClientFacade);
            stateHolder.SetNextState(initialState);
        }
    }
}
