using System.CodeDom;
using System.Runtime.InteropServices;
using TutoPcCleaner.Helpers;

namespace TutoPcCleaner
{
    public partial class MainPage : ContentPage
    {

        Sysinfos Sysinfos = new Sysinfos();

        const string IGNORED = "Ignoré.";

        bool chkbFichiersTempChecked = true;
        bool chkbCorbeilleChecked = true;
        bool chkbWinUpdateChecked = true;
        bool chkbErrorsChecked = true;
        bool chkbLogsChecked = true;

        long totalCleanedSize = 0;

        public MainPage()
        {
            InitializeComponent();
            ShowSystemInfos();
            InitChkbStates();
        }

        private async void InfoButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                Uri uri = new Uri("https://jldigital.be");
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                //A faire si bug
            }
        }

        public void ShowSystemInfos()
        {
            //OS
            osVersion.Text = Sysinfos.GetWinVer();
            //CPU
            hardware.Text = Sysinfos.GetHardwareInfos();

        }

        public void InitChkbStates()
        {
            chkbFichiersTempChecked = Preferences.Get("chkbFichiersTempChecked", true);
            chkbFichiersTemp.IsChecked = chkbFichiersTempChecked;

            chkbErrorsChecked = Preferences.Get("chkbErrorsChecked", true);
            chkbErrors.IsChecked = chkbErrorsChecked;

            chkbCorbeilleChecked = Preferences.Get("chkbCorbeilleChecked", true);
            chkbCorbeille.IsChecked = chkbCorbeilleChecked;

            chkbLogsChecked = Preferences.Get("chkbLogsChecked", true);
            chkbLogs.IsChecked = chkbLogsChecked;

            chkbWinUpdateChecked = Preferences.Get("chkbWinUpdateChecked", true);
            chkbWinUpdate.IsChecked = chkbWinUpdateChecked;
        }

        private void ButtonClean_Clicked(object sender, EventArgs e)
        {
            //dbg.Text = "Infos : " + chkbCorbeilleChecked + " - " + chkbErrorsChecked + " - " + chkbFichiersTempChecked + " - " + chkbWinUpdateChecked + " - " + chkbLogsChecked;
            ResetValues();

            infos.IsVisible = false;

            if (chkbFichiersTempChecked)
            {
                ClearWindowsTempFolder();
            }

            if (chkbCorbeilleChecked)
            {
                EmptyRecycleBin();
            }

            if (chkbWinUpdateChecked)
            {
                ClearWinUpdate();
            }

            if (chkbErrorsChecked)
            {
                ClearWinWer();
            }

            if (chkbLogsChecked)
            {
                ClearWinLogs();
            }

            progression.Progress = 1;
            tableRecap.IsVisible = true;

            long totalCleanedSizeInMb = totalCleanedSize / 1000000;
            if (totalCleanedSizeInMb < 0) totalCleanedSizeInMb = 0;

            if(totalCleanedSizeInMb < 50)
            {
                totalSize.Text = "< 10 Mb supprimés.";
            }
            else
            {
                totalSize.Text = "~" + totalCleanedSizeInMb + " MB supprimés !";
            }
        }
        /// <summary>
        /// Vider la corbeille
        /// </summary>
        public void EmptyRecycleBin()
        {
            const int SHERB_NOCONFIRMATION = 0x00000001;

            try
            {
                SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION);
                detailCorbeille.Detail = "Les fichiers de la corbeilles ont bien été supprimés";
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erreur lors de la tentative de vidage de la corbeille : {e.Message}");
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SHEmptyRecycleBin(IntPtr hWnd, string pszRootPath, uint dwFlags);

        #region clearTemp
        /// <summary>
        /// Videl les fichiers temporaires
        /// </summary>
        public void ClearWindowsTempFolder()
        {
            string path = @"C:\Windows\Temp";

            if(Directory.Exists(path))
            {
                detailFichiersTemp.Detail = GetFilesCountInFolder(path) + " Fichiers supprimés.";

                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize = totalCleanedSize + size;

                ProcessDirectory(path);
            }
        }

        public void ResetValues()
        {
            totalCleanedSize = 0;
            progression.Progress = 0;
            tableRecap.IsVisible = false;
            totalSize.Text = "";

            detailCorbeille.Detail = IGNORED;
            detailErrors.Detail = IGNORED;
            detailFichiersTemp.Detail = IGNORED;
            detailLogs.Detail = IGNORED;
            detailWinUpdate.Detail = IGNORED;
        }

        public static long DirSize(DirectoryInfo dir)
        {
            long size = 0;
            FileInfo[] fis = dir.GetFiles();
            foreach(FileInfo fi in fis)
            {
                size += fi.Length;
            }

            DirectoryInfo[] dis = dir.GetDirectories();
            foreach(DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }

            return size;
        }

        public int GetFilesCountInFolder(string path)
        {
            try
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Count();
            }
            catch(UnauthorizedAccessException ex)
            {
                return -1;
            }
            catch(Exception ex)
            {
                return -1;
            }
        }

        public void ProcessDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries) ProcessFile(fileName);

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach(string subdirectory in subdirectoryEntries) ProcessDirectory(subdirectory);
        }

        public void ProcessFile (string path)
        {
            try
            {
                if (path.Contains("\\Temp"))
                {
                    File.Delete(path);
                }
                else if (path.Contains("\\SoftwareDistribution"))
                {
                    File.Delete(path);
                }
                else if (path.Contains("\\winevt\\Logs"))
                {
                    File.Delete(path);
                }
                else if (path.Contains("\\Windows\\WER"))
                {
                    File.Delete(path);
                }
            }
            catch(Exception e)
            {
                FileInfo fi = new FileInfo(path);
                totalCleanedSize -= fi.Length;
            }
        }
        #endregion

        public void ClearWinUpdate()
        {
            string path = @"C:\Windows\SoftwareDistribution\Download";

            if(Directory.Exists(path))
            {
                detailWinUpdate.Detail = GetFilesCountInFolder(path) + " Fichiers supprimés";

                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize = totalCleanedSize + size;

                ProcessDirectory(path);
            }
        }

        public void ClearWinWer()
        {
            string path = @"C:\ProgramData\Microsoft\Windows\WER";

            if (Directory.Exists(path))
            {
                detailErrors.Detail = GetFilesCountInFolder(path) + " Fichiers supprimés";

                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize = totalCleanedSize + size;

                ProcessDirectory(path);
            }
        }

        public void ClearWinLogs()
        {
            string path = @"C:\Windows\System32\winevt\Logs";

            if (Directory.Exists(path))
            {
                detailLogs.Detail = GetFilesCountInFolder(path) + " Fichiers supprimés";

                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize = totalCleanedSize + size;

                ProcessDirectory(path);
            }
        }

        private void chkbFichiersTemp_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbFichiersTempChecked = e.Value;
            Preferences.Set("chkbFichiersTempChecked", chkbFichiersTempChecked);
        }

        private void chkbCorbeille_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbCorbeilleChecked = e.Value;
            Preferences.Set("chkbCorbeilleChecked", chkbCorbeilleChecked);
        }

        private void chkbLogs_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbLogsChecked = e.Value;
            Preferences.Set("chkbLogsChecked", chkbLogsChecked);
        }

        private void chkbWinUpdate_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbWinUpdateChecked = e.Value;
            Preferences.Set("chkbWinUpdateChecked", chkbWinUpdateChecked);
        }

        private void chkbErrors_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbErrorsChecked = e.Value;
            Preferences.Set("chkbErrorsChecked", chkbErrorsChecked);
        }
    }

}
