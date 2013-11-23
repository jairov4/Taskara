using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

		long _Repetitions;
		public long Repetitions
		{
			get { return _Repetitions; }
			set { _Repetitions = value; NotifyPropertyChanged("Repetitions"); }
		}

		ObservableCollection<ExcerciseTreeItem> _Children;
		public ObservableCollection<ExcerciseTreeItem> Children
		{
			get { return _Children; }
		}

		ExcerciseTreeItem _Parent;
		public ExcerciseTreeItem Parent
		{
			get { return _Parent; }
			private set { _Parent = value; NotifyPropertyChanged("Parent"); }
		}

		bool _Visible = true;
		public bool Visible
		{
			get { return _Visible; }
			set { _Visible = value; NotifyPropertyChanged("Visible"); }
		}

		public ExcerciseTreeItem()
		{
			_Children = new ObservableCollection<ExcerciseTreeItem>();
			_Children.CollectionChanged += _Children_CollectionChanged;
		}

		public ExcerciseTreeItem(string name, IEnumerable<ExcerciseTreeItem> children = null)
		{
			Name = name;
			_Children = new ObservableCollection<ExcerciseTreeItem>();
			_Children.CollectionChanged += _Children_CollectionChanged;
			if (children != null)
				foreach (var item in children)
				{
					_Children.Add(item);
				}
		}

		void _Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
				|| e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
			{
				foreach (ExcerciseTreeItem item in e.NewItems)
				{
					if (item.Parent != this)
					{
						if (item.Parent != null) throw new InvalidOperationException();
						item.Parent = this;
					}
					item.PropertyChanged += child_PropertyChanged;
				}
			}
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove
				|| e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace
				|| e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				foreach (ExcerciseTreeItem item in e.OldItems)
				{
					if (item.Parent == this) item.Parent = null;
					item.PropertyChanged -= child_PropertyChanged;
				}
			}
		}

		void child_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible") UpdateVisible();
		}

		private void UpdateVisible()
		{
			Visible = Children.Any(x => x.Visible);
		}

		public static ExcerciseTreeItem Find(IEnumerable<string> path, IEnumerable<ExcerciseTreeItem> range)
		{
			var items = GetItemsInPath(path, range);
			return items.LastOrDefault();
		}

		public static IEnumerable<ExcerciseTreeItem> GetItemsInPath(IEnumerable<string> path, IEnumerable<ExcerciseTreeItem> items)
		{
			var currentName = path.FirstOrDefault();
			var currentItem = items.FirstOrDefault(x => x.Name == currentName);
			yield return currentItem;

			var remaining = path.Skip(1);
			if (remaining.Count() == 0) yield break;

			var col = GetItemsInPath(remaining, currentItem.Children);
			foreach (var item in col)
			{
				yield return item;
			}
		}

		public IEnumerable<ExcerciseTreeItem> GetItemsInPath()
		{
			if (Parent != null)
				foreach (var item in Parent.GetItemsInPath())
					yield return item;
			yield return this;
		}

		public IEnumerable<string> GetPath()
		{
			if (Parent != null)
				foreach (var item in Parent.GetPath())
					yield return item;
			yield return Name;
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

		bool _IsNew;
		public bool IsNew
		{
			get { return _IsNew; }
			set { _IsNew = value; NotifyPropertyChanged("IsNew"); }
		}

		public PrescriptionViewModel()
		{
			AvailableExcercises.Add(new ExcerciseTreeItem("Nivel I", new[]
				{
					new ExcerciseTreeItem("an"),
					new ExcerciseTreeItem("in"),
					new ExcerciseTreeItem("kan"),
					new ExcerciseTreeItem("kun")
				}));
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
				tryItem.Visible = false;
			}
		}

		public void Insert(ExcerciseTreeItem tryItem, int idx)
		{
			if (tryItem.Children.Count > 0)
			{
				foreach (var item in tryItem.Children)
				{
					Insert(item, idx);
				}
			}
			else if (tryItem.Visible)
			{
				PrescriptionExcercises.Insert(idx, tryItem);
				tryItem.Visible = false;
			}
		}

		public void InsertSelected(int idx)
		{
			if (SelectedSourceExcercise == null || !SelectedSourceExcercise.Visible) return;
			Insert(SelectedSourceExcercise, idx);
			SelectedSourceExcercise = null;
		}

		public void RemoveSelected()
		{
			var tmp = SelectedPrescriptionExcercise;
			if (tmp == null) return;
			PrescriptionExcercises.Remove(tmp);
			tmp.Visible = true;
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

		Prescription prescription;
		public Prescription Prescription
		{
			get { return prescription; }
		}

		public void OpenView(object obj)
		{
			if (obj is long)
			{
				var id = (long)obj;
				prescription = App.Instance.Service.GetPrescriptionById(id);
				BuildView(prescription);
				IsNew = false;
			}
			else
			{
				IsNew = true;
				prescription = new Prescription();
				prescription.Issued = DateTime.Now;
				prescription.Patient = obj as Patient;
				prescription.Excercises = new List<Excercise>();
			}
		}

		public void CloseView()
		{
			if (!IsNew || PrescriptionExcercises.Count > 0)
			{
				BuildPrescription();
				App.Instance.Service.SavePrescription(prescription);
			}
		}

		private void BuildView(Prescription prescription)
		{
			foreach (var item in prescription.Excercises)
			{
				var treeItem = ExcerciseTreeItem.Find(item.Path, AvailableExcercises);
				// TODO: Manage unknown
				if (treeItem != null)
				{
					treeItem.Repetitions = item.Repetitions;
					Add(treeItem);
				}
			}
		}

		private void BuildPrescription()
		{
			var list = new List<Excercise>();
			foreach (var item in PrescriptionExcercises)
			{
				var itemPath = item.GetPath();
				var found = prescription.Excercises.FirstOrDefault(x => x.Path.SequenceEqual(itemPath));
				if (found == null)
				{
					found = new Excercise();
					found.Name = item.Name;
					found.Path = itemPath.ToArray();
					found.Repetitions = item.Repetitions;
					prescription.Excercises.Add(found);
				}
				list.Add(found);
			}
			foreach (var item in prescription.Excercises.ToArray())
			{
				if (!list.Contains(item)) prescription.Excercises.Remove(item);
			}
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

		PrescriptionViewModel ViewModel { get; set; }

		void PrescriptionEditPage_NavigatingOut(object sender, PageNavigationEventArgs e)
		{
			ViewModel.CloseView();
		}

		void PrescriptionEditPage_NavigatedIn(object sender, PageNavigationEventArgs e)
		{
			ViewModel = new PrescriptionViewModel();
			DataContext = ViewModel;
			ViewModel.OpenView(e.Parameter);
		}

		private void btnFinish_Click(object sender, RoutedEventArgs e)
		{
			Navigate(typeof(IndexPage));
		}

		Point mouse_down_point;

		private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed) return;
			var p = e.GetPosition(this);
			var d = p - mouse_down_point;
			var dd = Math.Sqrt(d.X * d.X + d.Y * d.Y);
			var dd_min = Math.Sqrt(SystemParameters.MinimumHorizontalDragDistance * SystemParameters.MinimumHorizontalDragDistance
				+ SystemParameters.MinimumVerticalDragDistance * SystemParameters.MinimumVerticalDragDistance);
			if (dd < dd_min) return;
			var draggedItem = tvSource.SelectedItem;
			if (draggedItem == null) return;

			ViewModel.SelectedPrescriptionExcercise = null;
			DragDropEffects finalDropEffect = DragDrop.DoDragDrop(tvSource, draggedItem, DragDropEffects.Move);
			Debug.WriteLine("Drag & Drop sucessfully");
		}

		private void TreeViewItem_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed) return;
			mouse_down_point = e.GetPosition(this);
		}

		private void ListBoxItem_DragEnter(object sender, DragEventArgs e)
		{
			var drop_target = sender as UIElement;
			var lbi = drop_target as ListBoxItem;
			if (lbi != null)
			{
				lbi.Background = new SolidColorBrush(Color.FromRgb(200, 0, 0));
			}
		}

		private void ListBoxItem_DragLeave(object sender, DragEventArgs e)
		{
			var drop_target = sender as UIElement;
			var lbi = drop_target as ListBoxItem;
			if (lbi != null)
			{
				lbi.Background = null;
			}
		}

		private void ListBoxItem_Drop(object sender, DragEventArgs e)
		{
			var data = e.Data.GetData(typeof(ExcerciseTreeItem)) as ExcerciseTreeItem;
			ViewModel.SelectedSourceExcercise = data;
			if (sender is ListBoxItem) // drop target is listboxitem
			{
				var target = sender as ListBoxItem;
				target.Background = null;
				var idx = lbTarget.ItemContainerGenerator.IndexFromContainer(target);
				ViewModel.InsertSelected(idx);
			}
			else if (sender == lbTarget) // drop target is listbox
			{
				var target = lbTarget;
				ViewModel.AddSelected();
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

		private void btnViewProgress_Click(object sender, RoutedEventArgs e)
		{
			if (!ViewModel.IsNew)
			{
				Navigate(typeof(PatientProgressPage));
			}
			else
			{
				MessageBox.Show("Lo siento, no puedes ver el progreso de una prescripcion nueva");
			}
		}
	}
}
