using Dotnetools.Tor.Control.Exceptions;
using Dotnetools.Tor.Control.Messages;
using Dotnetools.Tor.Control.Messages.Events;
using Dotnetools.Tor.Control.Messages.Events.OrEvents;
using Dotnetools.Tor.Control.Messages.Events.StatusEvents;

namespace Dotnetools.Tor.Control.Utils;

/// <summary>Parses an incoming Tor control event based on its name.</summary>
public static class AsyncEventParser
{
	/// <exception cref="TorControlReplyParseException"/>
	public static IAsyncEvent Parse(TorControlReply reply)
	{
		if (reply.StatusCode != StatusCode.AsynchronousEventNotify)
		{
			throw new TorControlReplyParseException($"Event: Expected {StatusCode.AsynchronousEventNotify} status code.");
		}

		(string value, _) = Tokenizer.ReadUntilSeparator(reply.ResponseLines[0]);

		return value switch
		{
			StatusEvent.EventNameStatusClient or StatusEvent.EventNameStatusServer or StatusEvent.EventNameStatusGeneral => StatusEvent.FromReply(reply),
			CircEvent.EventName => CircEvent.FromReply(reply),
			StreamEvent.EventName => StreamEvent.FromReply(reply),
			NetworkLivenessEvent.EventName => NetworkLivenessEvent.FromReply(reply),
			OrConnEvent.EventName => OrConnEvent.FromReply(reply),
			_ => throw new NotSupportedException("This should never happen."),
		};
	}
}