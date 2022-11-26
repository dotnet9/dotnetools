using NBitcoin;
using Dotnetools.WabiSabi.Backend.Rounds;
using Dotnetools.WabiSabi.Models;

namespace Dotnetools.WabiSabi.Client.RoundStateAwaiters;

public class UnexpectedRoundPhaseException : Exception
{
	public UnexpectedRoundPhaseException(uint256 roundId, Phase expected, RoundState roundState)
	{
		RoundId = roundId;
		Expected = expected;
		Actual = roundState.Phase;
		RoundState = roundState;
	}

	public uint256 RoundId { get; }
	public Phase Expected { get; }
	public Phase Actual { get; }
	public RoundState RoundState { get; }

	public override string Message => $"Round {RoundId} unexpected phase change. Waiting for '{Expected}' but the round is in '{Actual}' - ErrorCode:'{RoundState.EndRoundState}'.";
}