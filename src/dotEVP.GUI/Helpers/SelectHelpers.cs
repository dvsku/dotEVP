using System.Windows.Forms;

namespace dotEVP.GUI.Helpers {
    public static class SelectHelpers {
        public static string SelectFolder() {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog {
                ShowNewFolderButton = true,
                Description = "Select folder"
            };
            DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult == DialogResult.OK) {
                return folderBrowserDialog.SelectedPath;
            }

            return string.Empty;
        }

        public static string SelectFile() {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog {
                Title = "Select file",
                Filter = "EVP (*.evp)|*.evp"
            };
            bool? dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == true)
                return openFileDialog.FileName;
            return string.Empty;
        }

        public static string SelectSaveFile() {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog {
                DefaultExt = ".evp",
                Filter = "EVP (*.evp)|*.evp"
            };
            bool? dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == true)
                return saveFileDialog.FileName;
            return string.Empty;
        }
    }
}