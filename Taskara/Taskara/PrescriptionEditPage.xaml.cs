using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Taskara.Model;

namespace Taskara
{
	public class ExcerciseTreeItem : ObservableObject
	{
		string _Name;
		public string Name
		{
			get { return _Name; }
			set { _Name = value; NotifyPropertyChanged("Name"); }
		}

		ObservableCollection<ExcerciseTreeItem> _Children = new ObservableCollection<ExcerciseTreeItem>();
		public ObservableCollection<ExcerciseTreeItem> Children
		{
			get { return _Children; }
			set { _Children = value; NotifyPropertyChanged("Children"); }
		}

		string[] _Path;
		public string[] Path
		{
			get { return _Path; }
			set { _Path = value; NotifyPropertyChanged("Path"); }
		}

		bool _Visible;
		public bool Visible
		{
			get { return _Visible; }
			set { _Visible = value; NotifyPropertyChanged("Visible"); }
		}

		public ExcerciseTreeItem()
		{
			Visible = true;
		}
	}

	public class PrescriptionViewModel : ObservableObject
	{
		ExcerciseTreeItem _SelectedSourceExcercise;
		public ExcerciseTreeItem SelectedSourceExcercise
		{
			get { return _SelectedSourceExcercise; }
			set { _SelectedSourceExcercise = value; NotifyPropertyChanged("SelectedSourceExcercise"); }
		}

		ExcerciseTreeItem _SelectedPrescriptionExcercise;
		public ExcerciseTreeItem SelectedPrescriptionExcercise
		{
			get { return _SelectedPrescriptionExcercise; }
			set { _SelectedPrescriptionExcercise = value; NotifyPropertyChanged("SelectedPrescriptionExcercise"); }
		}

		ObservableCollection<ExcerciseTreeItem> _PrescriptionExcercises = new ObservableCollection<ExcerciseTreeItem>();
		public ObservableCollection<ExcerciseTreeItem> PrescriptionExcercises { get { return _PrescriptionExcercises; } }

		ObservableCollection<ExcerciseTreeItem> _AvailableExcercises = new ObservableCollection<ExcerciseTreeItem>();
		public ObservableCollection<ExcerciseTreeItem> AvailableExcercises { get { return _AvailableExcercises; } }

		public PrescriptionViewModel()
		{
			AvailableExcercises.Add(new ExcerciseTreeItem()
			{
				Name = "Nivel I",
				Children = new ObservableCollection<ExcerciseTreeItem>()
				{
					new ExcerciseTreeItem(){ Name="an"},
					new ExcerciseTreeItem(){ Name="in"},
					new ExcerciseTreeItem(){ Name="kan"},
					new ExcerciseTreeItem(){ Name="kun"},
				}
			});
			foreach (var item in AvailableExcercises)
			{
				BuildPaths(item, new string[0]);
			}
		}

		// recursivamente construye el arreglo de ruta
		private void BuildPaths(ExcerciseTreeItem item, string[] path)
		{
			item.Path = path;
			var pathlist = path.Concat(new[] { item.Name }).ToArray();
			foreach (var child in item.Children)
			{				
				BuildPaths(child, pathlist);
			}
		}

		public void AddSelected()
		{
			if (SelectedSourceExcercise == null || !SelectedSourceExcercise.Visible) return;
			Add(SelectedSourceExcercise);
		}

		private void Add(ExcerciseTreeItem tryItem)
		{
			if (tryItem.Children.Count > 0)
			{
				foreach (var item in tryItem.Children)
				{
					Add(item);
				}
			}
			else if (tryItem.Visible)
			{
				PrescriptionExcercises.Add(tryItem);
			}
			tryItem.Visible = false;
		}

		public void InsertSelected(int idx)
		{
			if (SelectedSourceExcercise == null || !SelectedSourceExcercise.Visible) return;
			PrescriptionExcercises.Insert(idx, SelectedSourceExcercise);
			SelectedSourceExcercise.Visible = false;
			SelectedSourceExcercise = null;
		}

		public void RemoveSelected()
		{
			var tmp = SelectedPrescriptionExcercise;
			if (tmp == null) return;
			PrescriptionExcercises.Remove(tmp);
			RestoreAncestorsTree(tmp, AvailableExcercises, 0);
			SelectedPrescriptionExcercise = null;
		}

		void RestoreAncestorsTree(ExcerciseTreeItem target, IEnumerable<ExcerciseTreeItem> actual, int depth)
		{
			if (depth < target.Path.Length)
			{
				var item = actual.FirstOrDefault(x => x.Name == target.Path[depth]);
				if (target != item) RestoreAncestorsTree(target, item.Children, depth + 1);
				item.Visible = true;
			}
			else
			{
				if (!actual.Contains(target)) return;
				target.Visible = true;
			}
		}

		public void MoveUpSelected()
		{
			if (SelectedPrescriptionExcercise == null) return;
			var tmp = SelectedPrescriptionExcercise;
			var idx = PrescriptionExcercises.IndexOf(tmp);
			if (idx == 0) return;
			PrescriptionExcercises.Remove(tmp);
			PrescriptionExcercises.Insert(idx - 1, tmp);
		}

		public void MoveDownSelected()
		{
			if (SelectedPrescriptionExcercise == null) return;
			var tmp = SelectedPrescriptionExcercise;
			var idx = PrescriptionExcercises.IndexOf(tmp);
			if (idx == PrescriptionExcercises.Count - 1) return;
			PrescriptionExcercises.Remove(tmp);
			PrescriptionExcercises.Insert(idx + 1, tmp);
		}
	}

	/// <summary>
	/// Lógica de interacción para CreateRecipe.xaml
	/// </summary>
	public partial class PrescriptionEditPage : Page
	{
		public PrescriptionEditPage()
		{
			InitializeComponent();
			NavigatedIn += PrescriptionEditPage_NavigatedIn;
			NavigatingOut += PrescriptionEditPage_NavigatingOut;
		}

		void PrescriptionEditPage_NavigatingOut(object sender, PageNavigationEventArgs e)
		{
			
		}

		PrescriptionViewModel ViewModel { get; set; }

		void PrescriptionEditPage_NavigatedIn(object sender, PageNavigationEventArgs e)
		{
			ViewModel = new PrescriptionViewModel();			
			DataContext = ViewModel;
		}

		private void btnFinish_Click(object sender, RoutedEventArgs e)
		{			
			Navigate(typeof(IndexPage));
		}

		Point mouse_down_point;
		UIElement drop_target;

		private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed) return;
			var p = e.GetPosition(this);
			var d = p - mouse_down_point;
			var dd = Math.Sqrt(d.X * d.X + d.Y * d.Y);
			if (dd < 2.0) return;
			var draggedItem = tvSource.SelectedItem;
			if (draggedItem == null) return;
			drop_target = null;
			ViewModel.SelectedPrescriptionExcercise = null;
			DragDropEffects finalDropEffect = DragDrop.DoDragDrop(tvSource, draggedItem, DragDropEffects.Move);

			if ((finalDropEffect == DragDropEffects.Move) && (drop_target != null))
			{
				var lbItem = drop_target as ListBoxItem;
				var lb = drop_target as ListBox;
				if (lbItem != null)
				{
					var index = lbTarget.ItemContainerGenerator.IndexFromContainer(lbItem);
					ViewModel.InsertSelected(index);
				}
				else if (lb == lbTarget)
				{
					ViewModel.AddSelected();
				}
			}
		}

		private void TreeViewItem_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed) return;
			mouse_down_point = e.GetPosition(this);
		}

		private void ListBoxItem_DragEnter(object sender, DragEventArgs e)
		{
			drop_target = sender as UIElement;
			var lbi = drop_target as ListBoxItem;
			if (lbi != null)
			{
				lbi.Background = new SolidColorBrush(Color.FromRgb(200, 0, 0));
			}
		}

		private void ListBoxItem_DragLeave(object sender, DragEventArgs e)
		{
			drop_target = sender as UIElement;
			var lbi = drop_target as ListBoxItem;
			if (lbi != null)
			{
				lbi.Background = null;
			}
		}

		private void ListBoxItem_Drop(object sender, DragEventArgs e)
		{
			drop_target = sender as UIElement;
			var lbi = drop_target as ListBoxItem;
			if (lbi != null)
			{
				lbi.Background = null;
			}
			e.Handled = true;
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.AddSelected();
		}

		private void btnRemove_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.RemoveSelected();
		}

		private void btnUp_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.MoveUpSelected();
		}

		private void btnDown_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.MoveDownSelected();
		}

		private void tvSource_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			ViewModel.SelectedSourceExcercise = e.NewValue as ExcerciseTreeItem;
		}

		private void tvSource_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			ViewModel.AddSelected();
		}
	}
}
