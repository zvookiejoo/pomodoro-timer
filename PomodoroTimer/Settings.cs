using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PomodoroTimer
{
    static class Settings
    {
        public static List<SprintItem> SprintItems = new List<SprintItem>();

        private static readonly string filename = @"settings.bin";

        static void Initialize()
        {
            for (int i = 0; i < 3; i++)
            {
                SprintItems.Add(new SprintItem(25, 5));
            }

            SprintItems.Add(new SprintItem(25, 15));
        }

        public static void Load()
        {
            if (!File.Exists(filename))
            {
                Initialize();
                Save();

                return;
            }

            Stream configStream = File.OpenRead(filename);
            BinaryFormatter deserializer = new BinaryFormatter();
            SprintItems = (List<SprintItem>) deserializer.Deserialize(configStream);
            configStream.Close();

            SprintItem.nextId = SprintItems.Last().Id + 1;
        }

        public static void Save()
        {
            BinaryFormatter serializer = new BinaryFormatter();
            Stream configStream = File.OpenWrite(filename);
            serializer.Serialize(configStream, SprintItems);
            configStream.Close();
        }
    }

}
