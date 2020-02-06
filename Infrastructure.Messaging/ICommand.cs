using System;

namespace Infrastructure.Messaging
{
	public interface ICommand
    {
        Guid Id { get; }
    }
}