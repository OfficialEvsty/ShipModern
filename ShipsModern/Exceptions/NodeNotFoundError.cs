using System;

namespace ShipsForm.Exceptions
{
    class NodeNotFoundError : Exception
    {
        public NodeNotFoundError(string err_mess) : base(err_mess) { }
    }
}
