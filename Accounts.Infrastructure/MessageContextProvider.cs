using System;

namespace Accounts.Infrastructure
{
    public sealed class MessageContextProvider
    {
        private MessageContext? _context;

        public MessageContext Context
        {
            get => _context ?? throw new InvalidOperationException("Missing message context.");
            set => _context = value;
        }
    }
}