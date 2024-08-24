using Microsoft.VisualBasic;
using TutoPcCleaner.Helpers;

namespace TutoPcCleaner;

public partial class ToolsPage : ContentPage
{
    Sysinfos Sysinfos = new Sysinfos();
    public ToolsPage()
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

    private async void ImageButton_maj_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UpdatePage());
    }

    private async void ImageButton_ram_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RamPage());
    }

    private async void ImageButton_clean_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }


    private async void ImageButton_options_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OptionPage());
    }

    private void ButtonRestaurePoint_Clicked(object sender, EventArgs e)
    {
        dynamic restPoint = Interaction.GetObject("winmgmts:\\\\.\\root\\default:Systemrestore");

        if (restPoint != null)
        {
            if (restPoint.CreateRestonePoint("PC Cleaner restore point", 0, 100) == 0) 
            {
                restaureTxt.Text = "Point de restauration créé !";
            }
            else
            {
                restaureTxt.Text = "Echec lors de la création du point de restauration !";
            }
        }
    }
}