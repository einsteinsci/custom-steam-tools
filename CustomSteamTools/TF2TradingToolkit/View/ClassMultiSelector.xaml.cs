using System;
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
using CustomSteamTools.Schema;
using TF2TradingToolkit;

namespace TF2TradingToolkit.View
{
	/// <summary>
	/// Interaction logic for ClassMultiSelector.xaml
	/// </summary>
	public partial class ClassMultiSelector : UserControl
	{
		public HashSet<PlayerClass> SelectedClassses
		{ get; private set; }

		public event MultiClassSelectorEventHandler SelectionChanged;

		public ClassMultiSelector()
		{
			InitializeComponent();

			SelectedClassses = new HashSet<PlayerClass>();
		}

		private void _fireSelectionChanged(PlayerClass c, SelectorActionType t)
		{
			if (SelectionChanged != null)
			{
				SelectionChanged(this, new MultiClassSelectorEventArgs(c, t));
			}
		}

		private void _toggle(PlayerClass c)
		{
			if (!SelectedClassses.Contains(c))
			{
				SelectedClassses.Add(c);
				_fireSelectionChanged(c, SelectorActionType.Add);
			}
			else
			{
				SelectedClassses.Remove(c);
				_fireSelectionChanged(c, SelectorActionType.Remove);
			}
		}

		private void ScoutBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(PlayerClass.Scout);
		}

		private void SoldierBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(PlayerClass.Soldier);
		}

		private void PryoBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(PlayerClass.Pyro);
		}

		private void DemomanBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(PlayerClass.Demoman);
		}

		private void HeavyBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(PlayerClass.Heavy);
		}

		private void EngineerBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(PlayerClass.Engineer);
		}

		private void MedicBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(PlayerClass.Medic);
		}

		private void SniperBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(PlayerClass.Sniper);
		}

		private void SpyBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(PlayerClass.Spy);
		}
	}
}
