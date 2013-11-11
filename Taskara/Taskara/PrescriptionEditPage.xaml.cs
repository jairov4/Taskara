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

		IList<ExcerciseTreeItem> _Children;
		public IList<ExcerciseTreeItem> Children
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
			Children = new List<ExcerciseTreeItem>();
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
				Children = new List<ExcerciseTreeItem>()
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
			foreach (var child in item.Children)
			{
				child.Path = path;
				var l = path.ToList();
				l.Add(child.Name);
				BuildPaths(child, l.ToArray());
			}
		}

		public void AddSelected()
		{
			if (SelectedSourceExcercise == null || !SelectedSourceExcercise.Visible) return;
			PrescriptionExcercises.Add(SelectedSourceExcercise);
			SelectedSourceExcercise.Visible = false;
			SelectedSourceExcercise = null;
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
			if (SelectedPrescriptionExcercise == null) return;
			SelectedPrescriptionExcercise.Visible = true;
			PrescriptionExcercises.Remove(SelectedPrescriptionExcercise);
			SelectedPrescriptionExcercise = null;
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
	}
}
