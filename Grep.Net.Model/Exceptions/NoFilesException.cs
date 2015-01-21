using System;
using System.Linq;

namespace Grep.Net.Model.Exceptions
{
    public class NoFilesException : ArgumentException
    {
        public NoFilesException()
        {
        }

        public NoFilesException(string mes) : base(mes)
        {
        }
    }
}