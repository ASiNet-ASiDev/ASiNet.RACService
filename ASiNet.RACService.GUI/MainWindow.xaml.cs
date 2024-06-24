using System.Windows;
using System.Windows.Input;

namespace ASiNet.RACService.GUI;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        ServiceContext.InstallOrReinstallService();
        InitializeComponent();
        AddToAutorun.Checked += OnChecked;
        AddToAutorun.IsChecked = ServiceContext.Autorun;
        AddressText.Text = $"\n{ServiceContext.IpAddresses.Select(x => x.ToString()).FirstOrDefault()}";
        if (ServiceContext.IsRun)
            StatusText.Text = "Running";
        else
            StatusText.Text = "Stopped";
    }

    private void OnChecked(object sender, RoutedEventArgs e)
    {
        ServiceContext.Autorun = AddToAutorun.IsChecked ?? false;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private async void RunService(object sender, RoutedEventArgs e)
    {
        if (await ServiceContext.RunService())
        {
            App.Current.Dispatcher.Invoke(() => StatusText.Text = "Running");
        }
    }

    private async void StopService(object sender, RoutedEventArgs e)
    {
        if (await ServiceContext.StopService())
        {
            App.Current.Dispatcher.Invoke(() => StatusText.Text = "Stopped");
        }
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        var w = new ServiceSettings();
        w.ShowDialog();
    }

    private void UpdateServiceInfo(object sender, RoutedEventArgs e)
    {
        try
        {
            AddToAutorun.IsChecked = ServiceContext.Autorun;
            AddressText.Text = $"\n{ServiceContext.IpAddresses.Select(x => x.ToString()).FirstOrDefault()}";
            if (ServiceContext.IsRun)
                StatusText.Text = "Running";
            else
                StatusText.Text = "Stopped";
        }
        catch (Exception)
        {

        }
    }

    private void InstallOrReinstallService(object sender, RoutedEventArgs e)
    {
        try
        {
            ServiceContext.InstallOrReinstallService();
        }
        catch (Exception)
        {

        }
    }
}