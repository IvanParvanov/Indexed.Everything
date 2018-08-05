using System;

namespace IndexedEverything.Tests.Misc
{
    //             ༼ つ ◕_◕ ༽つ
    internal class TestPerson
    {
        public static string Static { get; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public int Age { get; set; }
    }
}
