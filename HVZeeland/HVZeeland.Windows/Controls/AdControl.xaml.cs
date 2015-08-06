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

namespace HVZeeland.Controls
{
    public sealed partial class AdControl : UserControl
    {
        public AdControl()
        {
            this.InitializeComponent();
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AdGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
