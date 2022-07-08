namespace MonkeyFinder.ViewModel;

public class BaseViewModel : INotifyPropertyChanged
{
    bool _isBusy;
    string _title;

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value) return;

            _isBusy = value;
            //OnPropertyChanged(nameof(IsBusy)); We use CallerMemberName attr below to automatically ref. name of prop, gets set at compile time
            OnPropertyChanged();
            OnPropertyChanged(nameof(IstNotBusy));
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value) return;

            _title = value;
            OnPropertyChanged();
        }
    }

    public bool IstNotBusy => !IsBusy;

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
