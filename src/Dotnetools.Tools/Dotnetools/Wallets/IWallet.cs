using System.Collections.Generic;
using System.Threading.Tasks;
using Dotnetools.Blockchain.TransactionOutputs;
using Dotnetools.Blockchain.Transactions;
using Dotnetools.WabiSabi.Client;

namespace Dotnetools.Wallets;

public interface IWallet
{
	string WalletName { get; }
	bool IsUnderPlebStop { get; }
	bool IsMixable { get; }

	/// <summary>
	/// Watch only wallets have no key chains.
	/// </summary>
	IKeyChain? KeyChain { get; }

	IDestinationProvider DestinationProvider { get; }
	int AnonScoreTarget { get; }
	bool ConsolidationMode { get; }
	TimeSpan FeeRateMedianTimeFrame { get; }
	bool RedCoinIsolation { get; }

	Task<bool> IsWalletPrivateAsync();

	Task<IEnumerable<SmartCoin>> GetCoinjoinCoinCandidatesAsync();

	Task<IEnumerable<SmartTransaction>> GetTransactionsAsync();
}