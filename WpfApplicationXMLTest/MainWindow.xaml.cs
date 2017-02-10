using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Rar.ViewWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private bool IsDatePickerUsed(DependencyObject obj)
        {
            if (obj.DependencyObjectType.Name.Equals("DatePicker")) return true;
            else
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (IsDatePickerUsed(child)) return true;

                }
            }
            return false;
        }


        private void dataGridF6_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Count == 0) return;
            var cellInfo = e.AddedCells[0];

            if (cellInfo.Column.GetType().Equals(typeof(DataGridTemplateColumn)))
            {
                DataGridTemplateColumn column = (DataGridTemplateColumn)cellInfo.Column;
                DataTemplate myDataTemplate = column.CellEditingTemplate;
                DependencyObject obj = (DependencyObject)myDataTemplate.LoadContent();
                if (IsDatePickerUsed(obj))
                {
                    dataGridF6.BeginEdit();
                }
            }


        }
    }

  
  
}
