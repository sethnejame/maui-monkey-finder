using MonkeyFinder.Services;

namespace MonkeyFinder.ViewModel;

public partial class MonkeysViewModel : BaseViewModel
{
	MonkeyService _monkeyService;
	public ObservableCollection<Monkey> Monkeys { get; } = new();

	public Command GetMonkeysCommand { get; }

	public MonkeysViewModel(MonkeyService monkeyService)
	{
		Title = "Monkey Finder";
		_monkeyService = monkeyService;
		GetMonkeysCommand = new Command(async () => await GetMonkeysAsync());
	}

	async Task GetMonkeysAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			var monkeys = await _monkeyService.GetMonkeys();

			if (Monkeys.Count != 0)
				Monkeys.Clear();

			foreach (var monkey in monkeys)
				Monkeys.Add(monkey);
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			await Shell.Current.DisplayAlert("Unable to fetch monkey list: ", $"{ex.Message}", "Clear");
		}
		finally
		{
			IsBusy = false;
		}
	}
}
