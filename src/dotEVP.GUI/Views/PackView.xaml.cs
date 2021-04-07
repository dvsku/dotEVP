using System.Windows.Controls;
using dotEVP.GUI.ViewModels;

namespace dotEVP.GUI.Views {
    public partial class PackView : UserControl {
        public PackView() {
            InitializeComponent();
            DataContext = new PackViewModel();
        }
    }
}