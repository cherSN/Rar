﻿using System;
using System.Collections.Generic;
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

namespace Rar.ViewWpf
{

    public partial class SaveCompaniesWindow : Window
    {
        public SaveCompaniesWindow()
        {
            InitializeComponent();
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Rar.ViewModel.RarViewModel vm = DataContext as Rar.ViewModel.RarViewModel;
            vm.SaveCompaniesFileCommand.Execute(null);
        }


    }
}
