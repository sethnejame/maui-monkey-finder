using MonkeyFinder.Services;

namespace MonkeyFinder.ViewModel;

public partial class MonkeysViewModel : BaseViewModel
{
	MonkeyService _monkeyService;
	public ObservableCollection<Monkey> Monkeys { get; } = new();
	public MonkeysViewModel(MonkeyService monkeyService)
	{
		Title = "Monkey Finder";
		_monkeyService = monkeyService;
	}
}
