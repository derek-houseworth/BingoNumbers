﻿using BingoNumbers.ViewModels;

namespace BingoNumbers.Views;
 

public partial class MainPage : ContentPage
{

	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}
