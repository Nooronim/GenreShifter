using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;

namespace GenreShifterProt4
{
    class FireBaseHelper
    {
        public FireBaseHelper()
        {

        }
        FirebaseClient firebase = new FirebaseClient("https://genre-shifter-default-rtdb.europe-west1.firebasedatabase.app/");

        public async Task<List<PlayerStats>> GetAllStats()
        {
            return (await firebase
                .Child("Stats")
                .OnceAsync<PlayerStats>()).Select(item => new PlayerStats
                {
                    Name = item.Object.Name,
                    Score = item.Object.Score,
                }).ToList();
        }

        public async Task AddStats(string name, int score)
        {
            await firebase
                .Child("Stats")
                .PostAsync(new PlayerStats() { Name = name, Score = score });
        }


    }
}