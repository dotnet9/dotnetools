using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Dotnetools.Blockchain.Keys;
using Dotnetools.Fluent.Validation;
using System.Threading.Tasks;
using Dotnetools.Fluent.ViewModels.AddWallet.Create;
using Dotnetools.Fluent.Models;
using Dotnetools.Fluent.ViewModels.AddWallet.HardwareWallet;
using Dotnetools.Fluent.ViewModels.Navigation;
using Dotnetools.Fluent.Helpers;
using Dotnetools.Models;
using Dotnetools.Helpers;
using NBitcoin;
using Dotnetools.Extensions;

namespace Dotnetools.Fluent.ViewModels.AddWallet;

[NavigationMetaData(Title = "Wallet Name")]
public partial class WalletNamePageViewModel : RoutableViewModel
{
	[AutoNotify] private string _walletName = "";
	private readonly string? _importFilePath;
	private readonly Lazy<Mnemonic> _mnemonic = new(() => new Mnemonic(Wordlist.English, WordCount.Twelve));

	public WalletNamePageViewModel(WalletCreationOption creationOption, string? importFilePath = null)
	{
		_importFilePath = importFilePath;

		_walletName = Services.WalletManager.WalletDirectories.GetNextWalletName("Wallet");

		EnableBack = true;

		var canExecute =
			this.WhenAnyValue(x => x.WalletName)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Select(x => !string.IsNullOrWhiteSpace(x) && !Validations.Any);

		NextCommand = ReactiveCommand.CreateFromTask(async () => await OnNextAsync(WalletName, creationOption), canExecute);

		this.ValidateProperty(x => x.WalletName, ValidateWalletName);
	}

	private async Task OnNextAsync(string walletName, WalletCreationOption creationOption)
	{
		IsBusy = true;

		// Makes sure we can create a wallet with this wallet name.
		await Task.Run(() => WalletGenerator.GetWalletFilePath(walletName, Services.WalletManager.WalletDirectories.WalletsDir));

		IsBusy = false;

		switch (creationOption)
		{
			case WalletCreationOption.AddNewWallet:
				Navigate().To(new RecoveryWordsViewModel(_mnemonic.Value, walletName));
				break;

			case WalletCreationOption.ConnectToHardwareWallet:
				Navigate().To(new ConnectHardwareWalletViewModel(walletName));
				break;

			case WalletCreationOption.RecoverWallet:
				Navigate().To(new RecoverWalletViewModel(walletName));
				break;

			case WalletCreationOption.ImportWallet when _importFilePath is { }:
				await ImportWalletAsync(walletName, _importFilePath);
				break;

			default:
				throw new InvalidOperationException($"{nameof(WalletCreationOption)} not supported: {creationOption}");
		}
	}

	private async Task ImportWalletAsync(string walletName, string filePath)
	{
		try
		{
			var keyManager = await ImportWalletHelper.ImportWalletAsync(Services.WalletManager, walletName, filePath);
			Navigate().To(new AddedWalletPageViewModel(keyManager));
		}
		catch (Exception ex)
		{
			await ShowErrorAsync("Import wallet", ex.ToUserFriendlyString(), "Wasabi was unable to import your wallet.");
			BackCommand.Execute(null);
		}
	}

	private void ValidateWalletName(IValidationErrors errors)
	{
		var error = WalletHelpers.ValidateWalletName(WalletName);
		if (error is { } e)
		{
			errors.Add(e.Severity, e.Message);
		}
	}

	protected override void OnNavigatedTo(bool isInHistory, CompositeDisposable disposables)
	{
		base.OnNavigatedTo(isInHistory, disposables);

		var enableCancel = Services.WalletManager.HasWallet();
		SetupCancel(enableCancel: enableCancel, enableCancelOnEscape: enableCancel, enableCancelOnPressed: enableCancel);
	}
}