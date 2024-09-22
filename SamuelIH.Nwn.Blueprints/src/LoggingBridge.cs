using System;

namespace SamuelIH.Nwn.Blueprints
{
    public sealed class LoggingBridge
    {
        public Action<string> Error { get; set; } = Console.Error.WriteLine;
        public Action<string> Warning { get; set; } = Console.WriteLine;
        public Action<string> Info { get; set; } = Console.WriteLine;
    }
}