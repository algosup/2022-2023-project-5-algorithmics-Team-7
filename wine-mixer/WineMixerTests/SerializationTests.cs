using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WineMixer;
using WineMixer.Serialization;

namespace WineMixerTests
{
    public class SerializationTests
    {
        public Input TestInput = new Input()
        {
            TargetRecipe = new()
            {
                new ()
                {
                    WineAmount = 35.6,
                    WineName = "Chardonnay",
                    InitialTankIndex = 0,
                },
                new ()
                {
                    WineAmount = 30.2,
                    WineName = "Pinot Noir", 
                    InitialTankIndex = 1,
                },
                new()
                {
                    WineAmount = 34.2, 
                    WineName = "Cabernet",
                    InitialTankIndex = 2,
                },
            },

            TankSizes = new()
            {
                20,
                20,
                20,
                5,
                10,
                15,
                30,
                40,
                45,
                50,
                60,
            },
        };

        [Test]
        public void OutputJson()
        {
            var json = JsonSerializer.Serialize(TestInput, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
            Console.WriteLine(json);
        }
    }
}
