﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OperatorPage : Page
    {
        public OperatorPage()
        {
            this.InitializeComponent();
        }

        private void ShowHotTankSlider(object sender, RoutedEventArgs e)
        {
            //SetHotTankSlider.Visibility="True";
        }

        private void NewModeSelected(object sender, SelectionChangedEventArgs e)
        {
            //Urban visa dialog här
            //kommunicera med PLC?
        }

        private void PhaseShiftButton(object sender, RoutedEventArgs e)
        {
            //just practice

            PhaseShiftResultText.Text = "Phase Shift: 4 seconds";
        }
    }


   


}
