using System;
using System.Windows.Input;
using TutoPcCleaner.Helpers;

namespace TutoPcCleaner;

public partial class UpdatePage : ContentPage
{
    Sysinfos Sysinfos = new Sysinfos();

    public ICommand TapCommand => new Command<String>(async (url) => await Launcher.OpenAsync(url));
    int version = 1;
    public UpdatePage()
	{
		InitializeComponent();
        ShowSystemInfos();
        BindingContext = this;
        CheckVersion();
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


    private async void ImageButton_Options_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OptionPage());
    }

    private async void ButtonUpdateSoft_Clicked(object sender, EventArgs e)
    {
        try
        {
            Uri uri = new Uri("https://jldigital.be");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {

        }
    }

    public async void CheckVersion()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://www.anthony-cardinale.fr/_public/_dev/v1";
                string s = await client.GetStringAsync(url);
                int lastVersion = int.Parse(s);

                loadingGraph.IsVisible = false;
                loadingText.IsVisible = false;

                if (lastVersion > version)
                {
                    ShowUpdatePage();
                }
                else
                {
                    ShowDefaultPage();
                }
            }
        }
        catch (Exception ex)
        {
            //Pas d'internet
        }

    }

    public void ShowUpdatePage()
    {
        updateLink.IsVisible = true;
        updateSub.IsVisible = true;
        updateTitle.IsVisible = true;
        updateVersion.IsVisible = true;
    }

    public void ShowDefaultPage()
    {
        defaultLink.IsVisible = true;
        defaultSub.IsVisible = true;
        defaultTitle.IsVisible = true;
        defaultVersion.IsVisible = true;
    }
}