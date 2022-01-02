/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Secure
{
    public class SecureChannelResponseMessage<T>: SecureChannelResponseMessage
    {
        public SecureChannelResponseMessage() : base() { }
        public SecureChannelResponseMessage(string message) : base(message) { }
        public SecureChannelResponseMessage(string message, bool success) : base(message, success) { }

        public SecureChannelResponseMessage(T data)
            : base()
        {
            this.Data = data;
            this.Success = true;
        }

        public SecureChannelResponseMessage(Exception ex):base(ex)
        {
        }

        public SecureChannelResponseMessage(bool success)
        {
            this.Success = success;
        }

        public T Data
        {
            get;
            set;
        }
    }

    public class SecureChannelResponseMessage
    {
        public SecureChannelResponseMessage() { }

        public SecureChannelResponseMessage(string message)
        {
            this.Message = message;
        }

        public SecureChannelResponseMessage(string message, bool success)
        {
            this.Message = message;
            this.Success = success;
        }

        public SecureChannelResponseMessage(bool success)
        {
            this.Success = success;
        }

        public SecureChannelResponseMessage(Exception ex)
        {
            this.Message = ex.Message;
            this.Success = false;
        }

        public string Message { get; set; }
        public bool Success { get; set; }
    }
}