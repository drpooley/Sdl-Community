﻿using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.HunspellDictionaryManager.Commands;
using Sdl.Community.HunspellDictionaryManager.Helpers;
using Sdl.Community.HunspellDictionaryManager.Model;
using Sdl.Community.HunspellDictionaryManager.Ui;

namespace Sdl.Community.HunspellDictionaryManager.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Private Fields
		private ObservableCollection<HunspellLangDictionaryModel> _dictionaryLanguages = new ObservableCollection<HunspellLangDictionaryModel>();
		private HunspellLangDictionaryModel _selectedDictionaryLanguage;
		private string _newDictionaryLanguage;
		private string _labelVisibility = Constants.Hidden;
		private ICommand _createHunspellDictionaryCommand;
		private ICommand _cancelCommand;
		private MainWindow _mainWindow;
		#endregion

		#region Constructors
		public MainWindowViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			LoadStudioLanguageDictionaries();
		}
		#endregion

		#region Public Properties
		public HunspellLangDictionaryModel SelectedDictionaryLanguage
		{
			get => _selectedDictionaryLanguage;
			set
			{
				_selectedDictionaryLanguage = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<HunspellLangDictionaryModel> DictionaryLanguages
		{
			get => _dictionaryLanguages;
			set
			{
				_dictionaryLanguages = value;
				OnPropertyChanged();
			}
		}

		public string NewDictionaryLanguage
		{
			get => _newDictionaryLanguage;
			set
			{
				_newDictionaryLanguage = value;
				OnPropertyChanged();
			}
		}

		public string LabelVisibility
		{
			get => _labelVisibility;
			set
			{
				_labelVisibility = value;
				OnPropertyChanged();
			}
		}
		#endregion

		#region Commands
		public ICommand CreateHunspellDictionaryCommand => _createHunspellDictionaryCommand ?? (_createHunspellDictionaryCommand = new CommandHandler(CreateHunspellDictionaryAction, true));
		public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new CommandHandler(CancelAction, true));
		#endregion

		#region Private Methods
		/// <summary>
		/// Load .dic and .aff files from the installed Studio location -> HunspellDictionaries folder
		/// </summary>
		private void LoadStudioLanguageDictionaries()
		{
			var studioPath = Utils.GetInstalledStudioPath();
			var hunspellDictionaryFolderPath = Path.Combine(Path.GetDirectoryName(studioPath), Constants.HunspellDictionaries);

			// get .dic files from Studio HunspellDictionaries folder
			var dictionaryFiles = Directory.GetFiles(hunspellDictionaryFolderPath, "*.dic").ToList();
			foreach (var hunspellDictionary in dictionaryFiles)
			{
				var hunspellLangDictionaryModel = new HunspellLangDictionaryModel()
				{
					DictionaryFile = hunspellDictionary,
					DisplayName = Path.GetFileNameWithoutExtension(hunspellDictionary)					
				};

				_dictionaryLanguages.Add(hunspellLangDictionaryModel);
			}

			// get .aff files from Studio HunspellDictionaries folder
			var affFiles = Directory.GetFiles(hunspellDictionaryFolderPath, "*.aff").ToList();
			foreach (var affFile in affFiles)
			{
				var dictLang = _dictionaryLanguages
					.Where(d => Path.GetFileNameWithoutExtension(d.DictionaryFile).Equals(Path.GetFileNameWithoutExtension(affFile)))
					.FirstOrDefault();

				if(dictLang != null)
				{
					dictLang.AffFile = affFile;
				}
			}
		}

		private void CreateHunspellDictionaryAction()
		{
			var newDictionaryFilePath = Path.Combine(Path.GetDirectoryName(SelectedDictionaryLanguage.DictionaryFile), $"{NewDictionaryLanguage}.dic");
            var newAffFilePath = Path.Combine(Path.GetDirectoryName(SelectedDictionaryLanguage.AffFile), $"{NewDictionaryLanguage}.aff");
			
			File.Copy(SelectedDictionaryLanguage.DictionaryFile, newDictionaryFilePath, true);
			File.Copy(SelectedDictionaryLanguage.DictionaryFile, newAffFilePath, true);

			//LabelVisibility = Constants.Visible;
		}

		private void CancelAction()
		{
			if(_mainWindow.IsLoaded)
			{
				_mainWindow.Close();
			}
		}
		#endregion
	}
}