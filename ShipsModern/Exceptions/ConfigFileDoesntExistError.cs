
using System;

namespace ShipsForm.Exceptions
{
    class ConfigFileDoesntExistError : Exception
    {
        public ConfigFileDoesntExistError(string message = "Config file doesn't exist in context.") : base(message) { }
    }
}
