// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace IGeekFan.FreeKit.Extras.FreeSql
{
    [Serializable]
    internal class RecursionOverflowException : Exception
    {
        private int max_Deep;
        private string v;

        public RecursionOverflowException()
        {
        }

        public RecursionOverflowException(string? message) : base(message)
        {
        }

        public RecursionOverflowException(int max_Deep, string v)
        {
            this.max_Deep = max_Deep;
            this.v = v;
        }

        public RecursionOverflowException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected RecursionOverflowException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}