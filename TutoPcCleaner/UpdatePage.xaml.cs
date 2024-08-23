using TutoPcCleaner.Helpers;

namespace TutoPcCleaner;

public partial class UpdatePage : ContentPage
{
    Sysinfos Sysinfos = new Sysinfos();
    public UpdatePage()
	{
		InitializeComponent();
        ShowSystemInfos();
    }

    public void ShowSystemInfos()
    {
        //OS
        osVersion.Text = Sysinfos.GetWinVer();
        //CPU
        hardware.Text = Sysinfos.GetHardwareInfos();

    }

    private async void ImageButton_ram_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RamPage());
    }

    private async void ImageButton_tools_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ToolsPage());
    }

    private async void ImageButton_clean_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }


    private async void ImageButton_options_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OptionPage());
    }
}