using System;
using System.Runtime.Serialization;

namespace PomodoroTimer
{
    [Serializable]
    class SprintItem
    {
        public static int nextId = 1;

        [field: NonSerialized]
        public int Id { get; set; }
        public int WorkInterval { get; set; }
        public int BreakInterval { get; set; }

        public SprintItem(int Work, int Break)
        {
            Id = nextId++;

            WorkInterval = Work;
            BreakInterval = Break;
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            Id = nextId++;
        }
    }
}
