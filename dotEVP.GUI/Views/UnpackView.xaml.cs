using System.Windows.Controls;
using dotEVP.GUI.ViewModels;

namespace dotEVP.GUI.Views {
    public partial class UnpackView : UserControl {
        public UnpackView() {
            InitializeComponent();
            DataContext = new UnpackViewModel();
        }
    }
}