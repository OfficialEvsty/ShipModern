using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipsForm.Exceptions
{
    class JsonFileEmptyError : Exception
    {
        public JsonFileEmptyError(string message) : base(message) { }
    }
}
