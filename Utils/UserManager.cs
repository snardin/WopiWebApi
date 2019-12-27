using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WopiWebApi.Utils
{
    public class UserManager
    {
        private static List<string> _nickNames = new List<string>()
        {
            "Wolverine",
            "Spider-Man",
            "Thor",
            "Iron Man",
            "Hulk",
            "Captain America",
            "Daredevil",
            "Punisher",
            "Deadpool",
            "Silver Surfer",
            "Gambit",
            "Cyclops",
            "Mr. Fantastic",
            "Nightcrawler",
            "Nick Fury",
            "Human Torch",
            "Iceman",
            "Professor X",
            "Colossus",
            "Doctor Strange",
            "Storm",
            "Jean Grey",
            "Rogue",
            "Elektra",
            "Thing",
            "Black Bolt",
            "She-Hulk",
            "Invisible Woman",
            "Black Panther",
            "Beast",
            "Sentry",
            "Hawkeye",
            "Luke Cage",
            "Iron Fist",
            "Cable",
            "Moon Knight",
            "Angel",
            "Psylocke",
            "War Machine",
            "Black Cat",
            "Captain Marvel",
            "Quicksilver",
            "Spider-Woman",
            "Domino",
            "Vision",
            "Black Widow",
            "Blade",
            "Speedball",
            "Morph",
            "Nova",
            "Wasp",
            "Wonder Man",
            "Beta Ray Bill",
            "Falcon",
            "Jessica Jones"
        };

        private static Dictionary<string, string> _userList = null;

        private static int _counter = 0;

        public UserManager()
        {
            _userList = new Dictionary<string, string>();
        }

        public string GetNickName(string key)
        {
            string value = String.Empty;
            if (!string.IsNullOrEmpty(key))
            {
                if (!_userList.TryGetValue(key, out value))
                {
                    if (_nickNames.Count() > 0)
                    {
                        value = _nickNames.ElementAt(0);
                        _nickNames.RemoveAt(0);
                        _userList.Add(key, value);
                    }
                }
            }

            if (String.IsNullOrEmpty(value))
            {
                value = $"Test User {_counter.ToString()}";
                _counter++;
            }

            return value;
        }
    }
}
