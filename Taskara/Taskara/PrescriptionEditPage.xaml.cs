using Microsoft.Win32;
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
	/// <summary>
	/// Nodo de un arbol de ejercicios para ser mostrado en la interfaz grafica o manipulado en el 
	/// modelo de vista.
	/// </summary>
	public class ExerciseTreeItem : ObservableObject
	{
		string _Name;
		/// <summary>
		/// Nombre del ejercicio
		/// </summary>
		public string Name
		{
			get { return _Name; }
			set { _Name = value; NotifyPropertyChanged("Name"); }
		}

		ICollection<DayOfWeek> _WeeklyBasis = new List<DayOfWeek>();
		public ICollection<DayOfWeek> WeeklyBasis
		{
			get { return _WeeklyBasis; }
			set { _WeeklyBasis = value; NotifyPropertyChanged("WeeklyBasis"); }
		}

		/// <summary>
		/// Hijos del nodo
		/// </summary>
		ObservableCollection<ExerciseTreeItem> _Children;
		public ObservableCollection<ExerciseTreeItem> Children
		{
			get { return _Children; }
		}

		ExerciseTreeItem _Parent;
		/// <summary>
		/// Padre de este nodo
		/// </summary>
		public ExerciseTreeItem Parent
		{
			get { return _Parent; }
			private set { _Parent = value; NotifyPropertyChanged("Parent"); }
		}

		bool _Visible = true;
		/// <summary>
		/// Indica si este nodo debe mostrarse en la representacion grafico del arbol
		/// </summary>
		public bool Visible
		{
			get { return _Visible; }
			set { _Visible = value; NotifyPropertyChanged("Visible"); }
		}

		public ExerciseTreeItem()
		{
			_Children = new ObservableCollection<ExerciseTreeItem>();
			_Children.CollectionChanged += _Children_CollectionChanged;
		}

		/// <summary>
		/// Construye un nodo con un nombre y una coleccion de hijos
		/// </summary>
		/// <param name="name"></param>
		/// <param name="children"></param>
		public ExerciseTreeItem(string name, IEnumerable<ExerciseTreeItem> children = null)
		{
			Name = name;
			_Children = new ObservableCollection<ExerciseTreeItem>();
			_Children.CollectionChanged += _Children_CollectionChanged;
			if (children != null)
				foreach (var item in children)
				{
					_Children.Add(item);
				}
		}

		void _Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			// Es necesario suscribirse un delegado para monitorear los cambios en los descendientes
			// para asegurar que un nodo se muestra solo si contiene al menos un hijo visible
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
				|| e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
			{
				foreach (ExerciseTreeItem item in e.NewItems)
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
				foreach (ExerciseTreeItem item in e.OldItems)
				{
					if (item.Parent == this) item.Parent = null;
					item.PropertyChanged -= child_PropertyChanged;
				}
			}
		}

		/// <summary>
		/// Si un hijo cambia su propiedad Visible, entonces se reevalua la visibilidad del padre
		/// </summary>		
		void child_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible") UpdateVisible();
		}

		/// <summary>
		/// Hace al nodo visible solo si contiene al menos un hijo visible
		/// </summary>
		private void UpdateVisible()
		{
			Visible = Children.Any(x => x.Visible);
		}

		/// <summary>
		/// Busca un nodo en varios arboles usando una ruta
		/// </summary>
		/// <param name="path">Ruta a buscar</param>
		/// <param name="range">Nodos raices de los arboles</param>
		/// <returns>Instancia del nodo encontrado o <value>null</value></returns>
		public static ExerciseTreeItem Find(IEnumerable<string> path, IEnumerable<ExerciseTreeItem> range)
		{
			var items = GetItemsInPath(path, range);
			return items.LastOrDefault();
		}

		/// <summary>
		/// Devuelve todos los nodos desde el padre hasta el ultimo descendiente en ese orden 
		/// para una ruta especifica.
		/// La ruta buscada tiene que existir.
		/// </summary>
		/// <param name="path">Ruta que se busca</param>
		/// <param name="items">Arboles donde buscar</param>
		/// <returns>Instancias de nodos correspondientes a la rama encontrada</returns>
		public static IEnumerable<ExerciseTreeItem> GetItemsInPath(IEnumerable<string> path, IEnumerable<ExerciseTreeItem> items)
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

		/// <summary>
		/// Retorna los items ancestros del nodo actual hasta el nodo actual
		/// en ese orden.
		/// </summary>
		/// <returns>Instancias de ancestros y nodo actual</returns>
		public IEnumerable<ExerciseTreeItem> GetItemsInPath()
		{
			if (Parent != null)
				foreach (var item in Parent.GetItemsInPath())
					yield return item;
			yield return this;
		}

		/// <summary>
		/// Calcula la ruta del nodo actual desde su mas lejano ancestro
		/// </summary>
		/// <returns>Nombres en la rama de nodos ancestros hasta el nombre del nodo actual</returns>
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
		ExerciseTreeItem _SelectedSourceExercise;
		public ExerciseTreeItem SelectedSourceExercise
		{
			get { return _SelectedSourceExercise; }
			set { _SelectedSourceExercise = value; NotifyPropertyChanged("SelectedSourceExercise"); }
		}

		ExerciseTreeItem _SelectedPrescriptionExercise;
		public ExerciseTreeItem SelectedPrescriptionExercise
		{
			get { return _SelectedPrescriptionExercise; }
			set { _SelectedPrescriptionExercise = value; NotifyPropertyChanged("SelectedPrescriptionExercise"); }
		}

		ObservableCollection<ExerciseTreeItem> _PrescriptionExercises = new ObservableCollection<ExerciseTreeItem>();
		public ObservableCollection<ExerciseTreeItem> PrescriptionExercises { get { return _PrescriptionExercises; } }

		ObservableCollection<ExerciseTreeItem> _AvailableExercises;
		public ObservableCollection<ExerciseTreeItem> AvailableExercises { get { return _AvailableExercises; } }

		List<ProgressWeekData> _Progress;
		public List<ProgressWeekData> Progress
		{
			get { return _Progress; }
			set { _Progress = value; NotifyPropertyChanged("Progress"); }
		}

		bool _IsNew;
		public bool IsNew
		{
			get { return _IsNew; }
			set { _IsNew = value; NotifyPropertyChanged("IsNew"); }
		}

		public PrescriptionViewModel()
		{
			var uri = new Uri("/Exercises.xml", UriKind.Relative);
			var resInfo = Application.GetContentStream(uri);
			var items = ExerciseExchangeDefinitionCollection.LoadXml(resInfo.Stream);

			var r = from c in items.ExerciseDefinitions
					group c by c.Group into g1
					select new ExerciseTreeItem(g1.Key,
						from d in g1
						group d by d.Skill into g2
						select new ExerciseTreeItem("Subhabilidad " + g2.Key,
							from e in g2 select new ExerciseTreeItem(e.Exercise)
							)
						);


			//var r = from c in items.ExerciseDefinitions
			//		group c by c.Level into g1
			//		select new ExerciseTreeItem(g1.Key,
			//			from d in g1
			//			group d by d.ShortDescription into g2
			//			select new ExerciseTreeItem(
			//				g2.Key,
			//				from e in g2
			//				let grp = string.IsNullOrWhiteSpace(e.Group) ? e.Exercise.Substring(0, 1) : e.Group
			//				group e by grp into g3
			//				select new ExerciseTreeItem(
			//					g3.Key,
			//					from f in g3
			//					group f by f.Skill into g4
			//					select new ExerciseTreeItem(g4.Key.ToString(),
			//						g4.Select(x => new ExerciseTreeItem(x.Exercise))
			//					)
			//				)
			//			)
			//		);
			_AvailableExercises = new ObservableCollection<ExerciseTreeItem>(r);

		}

		/// <summary>
		/// Añade a los ejercicios prescritos los seleccionados en el arbol
		/// </summary>
		public void AddSelected()
		{
			if (SelectedSourceExercise == null || !SelectedSourceExercise.Visible) return;
			Add(SelectedSourceExercise);
		}

		/// <summary>
		/// Añade un item a los ejercicios prescritos.
		/// El item debe existir en la lista de items disponibles
		/// </summary>
		/// <param name="tryItem">Item que va a añadir</param>
		private void Add(ExerciseTreeItem tryItem)
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
				PrescriptionExercises.Add(tryItem);
				tryItem.Visible = false;
			}
		}

		/// <summary>
		/// Inserta un item en la lista de ejercicios prescritos en un indice especificado
		/// </summary>
		/// <param name="tryItem">Item que va añadir</param>
		/// <param name="idx">Indice donde va quedar el nuevo elemento</param>
		public void Insert(ExerciseTreeItem tryItem, int idx)
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
				PrescriptionExercises.Insert(idx, tryItem);
				tryItem.Visible = false;
			}
		}

		/// <summary>
		/// Inserta el item seleccionado en la lista de ejercicios disponibles en la lista 
		/// de ejercicios prescritos, el ejercicio queda en el indice especificado.
		/// </summary>
		/// <param name="idx">Indice donde va a quedar el nuevo elemento</param>
		public void InsertSelected(int idx)
		{
			if (SelectedSourceExercise == null || !SelectedSourceExercise.Visible) return;
			Insert(SelectedSourceExercise, idx);
			SelectedSourceExercise = null;
		}

		/// <summary>
		/// Remueve de la lista de ejercicios prescritos el elemento seleccionado.
		/// Se establece su atributo visible en true, para que sea visible en el arbol
		/// de ejercicios disponibles.
		/// </summary>
		public void RemoveSelected()
		{
			var tmp = SelectedPrescriptionExercise;
			if (tmp == null) return;
			PrescriptionExercises.Remove(tmp);
			tmp.Visible = true;
			SelectedPrescriptionExercise = null;
		}

		/// <summary>
		/// Mueve un ejercicio prescrito seleccionado hacia arriba (decrementa su indice)
		/// </summary>
		public void MoveUpSelected()
		{
			if (SelectedPrescriptionExercise == null) return;
			var tmp = SelectedPrescriptionExercise;
			var idx = PrescriptionExercises.IndexOf(tmp);
			if (idx == 0) return;
			PrescriptionExercises.Remove(tmp);
			PrescriptionExercises.Insert(idx - 1, tmp);
		}

		/// <summary>
		/// Mueve un ejercicio prescrito seleccionado hacia abajo (decrementa su indice)
		/// </summary>
		public void MoveDownSelected()
		{
			if (SelectedPrescriptionExercise == null) return;
			var tmp = SelectedPrescriptionExercise;
			var idx = PrescriptionExercises.IndexOf(tmp);
			if (idx == PrescriptionExercises.Count - 1) return;
			PrescriptionExercises.Remove(tmp);
			PrescriptionExercises.Insert(idx + 1, tmp);
		}

		Prescription prescription;
		/// <summary>
		/// Prescripcion siendo editada
		/// </summary>
		public Prescription Prescription
		{
			get { return prescription; }
		}

		/// <summary>
		/// Prepara este modelo de vista para trabajar con una prescripcion existente o 
		/// crear una nueva.
		/// </summary>
		/// <param name="obj">
		/// si es un entero, indicará el identifiador de la prescripcion a evaluar.
		/// Si es un paciente, indicará el paciente asociado
		/// </param>
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
				prescription.Exercises = new List<Exercise>();
			}
		}

		/// <summary>
		/// Guarda los cambios en la prescripcion
		/// </summary>
		public void CloseView()
		{
			if (!IsNew || PrescriptionExercises.Count > 0)
			{
				BuildPrescription();
				App.Instance.Service.SavePrescription(prescription);
			}
		}

		/// <summary>
		/// Construye la vista en terminos de las clases que representan el estado visual
		/// a partir del objeto de Prescripcion (que seguramente proviene del servicio de
		/// almacenamiento)
		/// </summary>
		/// <param name="prescription">Prescripcion a desplegar</param>
		private void BuildView(Prescription prescription)
		{
			if (prescription.Exercises != null)
				foreach (var item in prescription.Exercises)
				{
					var treeItem = ExerciseTreeItem.Find(item.Path, AvailableExercises);
					// TODO: Manage unknown
					if (treeItem != null)
					{
						if (item.WeeklyBasis != null)
							treeItem.WeeklyBasis = item.WeeklyBasis.ToList();
						else
							treeItem.WeeklyBasis = new List<DayOfWeek>();
						Add(treeItem);
					}
				}

			var pid = App.Instance.Service.GetId(prescription.Patient);
			var reps = App.Instance.Service.ListProgressReportsByPatientId(pid);
			// TODO: Remove this
			// TEST CODE
			if (reps.Count == 0 && prescription.Exercises != null && prescription.Exercises.Count > 0)
			{
				var cdate = DateTime.Now.AddDays(-1000);
				var random = new Random();
				for (int i = 0; i < 20; i++)
				{
					var dd = new PrescriptionProgressReport();
					dd.Issued = cdate;
					dd.Prescription = prescription;
					dd.Progress = new List<ExerciseProgressReport>();
					var pp = new ExerciseProgressReport();
					pp.Exercise = prescription.Exercises.First();
					pp.GoodRepetitions = 10;
					pp.TotalRepetitions = 20;
					dd.Progress.Add(pp);
					reps.Add(dd);
					cdate = cdate.AddDays(random.Next(1, 8));
				}
			} // END TEST CODE
			var reportWeeksData = new List<ProgressWeekData>();
			reps.Sort((x, y) => (int)(x.Issued - y.Issued).TotalSeconds);
			Progress = null;

			if (reps.Count == 0) return;

			var dt = reps.FirstOrDefault().Issued;
			var cw = new ProgressWeekData();
			int weekDayIdx = 0;
			foreach (var item in reps)
			{
				if (item.Issued > dt)
				{
					var remaining = (item.Issued.Date - dt.Date).TotalDays - 1;
					for (int i = 1; i <= (int)remaining; i++)
					{
						var emptyDay = new ProgressCellData();
						emptyDay.Date = dt.Date.AddDays(i);
						weekDayIdx = Util.DayOfWeekToNumber(emptyDay.Date.DayOfWeek);
						cw.Days[weekDayIdx] = emptyDay;
						if (weekDayIdx == 6)
						{
							reportWeeksData.Add(cw);
							cw = new ProgressWeekData();
						}
					}
				}

				var dat = new ProgressCellData();
				dat.Report = item;
				dt = item.Issued;
				weekDayIdx = Util.DayOfWeekToNumber(dt.DayOfWeek);
				cw.Days[weekDayIdx] = dat;
				if (weekDayIdx == 6)
				{
					reportWeeksData.Add(cw);
					cw = new ProgressWeekData();
					weekDayIdx = 0;
				}
			}
			if (weekDayIdx > 0)
			{
				reportWeeksData.Add(cw);
			}
			Progress = reportWeeksData;
		}

		/// <summary>
		/// Convierte los datos en este modelo de vista en una prescripcion del modelo de objetos
		/// de la aplicacion (que puede ser guardado u exportado)
		/// </summary>
		private void BuildPrescription()
		{
			var list = new List<Exercise>();
			if (prescription.Exercises == null)
				prescription.Exercises = new List<Exercise>();
			foreach (var item in PrescriptionExercises)
			{
				var itemPath = item.GetPath();				
				var found = prescription.Exercises.FirstOrDefault(x => x.Path.SequenceEqual(itemPath));
				if (found == null)
				{
					found = new Exercise();
					found.Name = item.Name;
					found.Path = itemPath.ToArray();
					prescription.Exercises.Add(found);
				}
				found.WeeklyBasis = item.WeeklyBasis.ToArray();
				list.Add(found);
			}
			foreach (var item in prescription.Exercises.ToArray())
			{
				if (!list.Contains(item)) prescription.Exercises.Remove(item);
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
			ViewModel.CloseView();
			var dlg = new SaveFileDialog();
			dlg.Title = "Exportar archivo de Prescripcion";
			dlg.Filter = "Archivos de prescripcion (*.rxml)|*.rxml|Todos los archivos (*.*)|*.*";
			var r = dlg.ShowDialog();
			if (r == true)
			{
				using (var str = dlg.OpenFile())
				{
					ViewModel.Prescription.SaveXml(str);
					str.Close();
				}
			}
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

			ViewModel.SelectedPrescriptionExercise = null;
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
			var data = e.Data.GetData(typeof(ExerciseTreeItem)) as ExerciseTreeItem;
			ViewModel.SelectedSourceExercise = data;
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
			ViewModel.SelectedSourceExercise = e.NewValue as ExerciseTreeItem;
		}

		private void tvSource_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			ViewModel.AddSelected();
		}

		private void btnViewProgress_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
