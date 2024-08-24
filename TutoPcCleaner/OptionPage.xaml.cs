using TutoPcCleaner.Helpers;
using System.Windows.Input;

namespace TutoPcCleaner;

public partial class OptionPage : ContentPage
{
    Sysinfos Sysinfos = new Sysinfos();
    public ICommand TapCommand => new Command<String>(async (url) => await Launcher.OpenAsync(url));
    public OptionPage()
	{
		InitializeComponent();
        ShowSystemInfos();
        BindingContext = this;
        paramSearchMaj.IsChecked = Preferences.Get("paramSearchMaj", true);
    }

    public void ShowSystemInfos()
    {
        //OS
        osVersion.Text = Sysinfos.GetWinVer();
        //CPU
        hardware.Text = Sysinfos.GetHardwareInfos();

    }

    private async void ImageButton_maj_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UpdatePage());
    }

    private async void ImageButton_tools_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ToolsPage());
    }

    private async void ImageButton_clean_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }


    private async void ImageButton_ram_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RamPage());
    }

    private void paramSearchMaj_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Preferences.Set("paramSearchMaj", e.Value);
    }
}