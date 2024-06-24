using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ASiNet.RACService.GUI;
/// <summary>
/// Логика взаимодействия для ServiceSettings.xaml
/// </summary>
public partial class ServiceSettings : Window
{
    public ServiceSettings()
    {
        InitializeComponent();
        _cnf = ServiceConfig.Read();
        PortTextB.Text = _cnf.Port.ToString();
        LoginTextB.Text = _cnf.Login;
        PasswordTextB.Text = _cnf.Password;
        ARAKeyboard.IsChecked = _cnf.AllowRemoteKeyboardAccess;
        ARADisk.IsChecked = _cnf.AllowRemoteDriveAccess;
        ARADiskWrite.IsChecked = _cnf.AllowRemoteDriveWriteAccess;
        ARADatabase.IsChecked = _cnf.AllowDatabaseRemoteAccess;
        ARADatabaseWrite.IsChecked = _cnf.AllowDatabaseRemoteWrite;
    }

    private ServiceConfig _cnf;

    private void ARAKeyboard_Checked(object sender, RoutedEventArgs e)
    {
        _cnf.AllowRemoteKeyboardAccess = ARAKeyboard.IsChecked ?? false;
    }

    private void ARADisk_Checked(object sender, RoutedEventArgs e)
    {
        _cnf.AllowRemoteDriveAccess = ARADisk.IsChecked ?? false;
    }

    private void ARADiskWrite_Checked(object sender, RoutedEventArgs e)
    {
        _cnf.AllowRemoteDriveWriteAccess = ARADiskWrite.IsChecked ?? false;
    }

    private void ARADatabase_Checked(object sender, RoutedEventArgs e)
    {
        _cnf.AllowDatabaseRemoteAccess = ARADatabase.IsChecked ?? false;
    }

    private void ARADatabaseWrite_Checked(object sender, RoutedEventArgs e)
    {
        _cnf.AllowDatabaseRemoteWrite = ARADatabaseWrite.IsChecked ?? false;
    }

    private void PasswordTextB_TextChanged(object sender, TextChangedEventArgs e)
    {
        _cnf.Password = string.IsNullOrWhiteSpace(PasswordTextB.Text) ? null : PasswordTextB.Text;
    }

    private void LoginTextB_TextChanged(object sender, TextChangedEventArgs e)
    {
        _cnf.Login = string.IsNullOrWhiteSpace(LoginTextB.Text) ? null : LoginTextB.Text;
    }

    private void PortTextB_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(int.TryParse(PortTextB.Text, out var result) && result > 0 && result <= ushort.MaxValue)
        {
            PortTextB.BorderBrush = new SolidColorBrush(Colors.Black);
            _cnf.Port = result;
        }
        else
        {
            PortTextB.BorderBrush = new SolidColorBrush(Colors.Red);
        }
    }

    private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        _cnf.AllowRemoteKeyboardAccess = ARAKeyboard.IsChecked ?? false;
        _cnf.AllowRemoteDriveAccess = ARADisk.IsChecked ?? false;
        _cnf.AllowRemoteDriveWriteAccess = ARADiskWrite.IsChecked ?? false;
        _cnf.AllowDatabaseRemoteAccess = ARADatabase.IsChecked ?? false;
        _cnf.AllowDatabaseRemoteWrite = ARADatabaseWrite.IsChecked ?? false;
        ServiceConfig.Write(_cnf);
        if (ServiceContext.IsRun)
        {
            await ServiceContext.StopService();
            await ServiceContext.RunService();
        }
        Close();
    }
}
