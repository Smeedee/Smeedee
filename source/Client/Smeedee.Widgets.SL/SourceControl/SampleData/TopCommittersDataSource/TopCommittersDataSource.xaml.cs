﻿//      *********    DO NOT MODIFY THIS FILE     *********
//      This file is regenerated by a design tool. Making
//      changes to this file can cause errors.
namespace Expression.Blend.SampleData.TopCommittersDataSource
{
	using System; 

// To significantly reduce the sample data footprint in your production application, you can set
// the DISABLE_SAMPLE_DATA conditional compilation constant and disable sample data at runtime.
#if DISABLE_SAMPLE_DATA
	internal class TopCommittersDataSource { }
#else

	public class TopCommittersDataSource : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public TopCommittersDataSource()
		{
			try
			{
				System.Uri resourceUri = new System.Uri("/Smeedee.Widget.SourceControl.SL;component/SampleData/TopCommittersDataSource/TopCommittersDataSource.xaml", System.UriKind.Relative);
				if (System.Windows.Application.GetResourceStream(resourceUri) != null)
				{
					System.Windows.Application.LoadComponent(this, resourceUri);
				}
			}
			catch (System.Exception)
			{
			}
		}

		private ItemCollection _Data = new ItemCollection();

		public ItemCollection Data
		{
			get
			{
				return this._Data;
			}
		}

		private string _SinceDate = string.Empty;

		public string SinceDate
		{
			get
			{
				return this._SinceDate;
			}

			set
			{
				if (this._SinceDate != value)
				{
					this._SinceDate = value;
					this.OnPropertyChanged("SinceDate");
				}
			}
		}
	}

	public class ItemCollection : System.Collections.ObjectModel.ObservableCollection<Item>
	{ 
	}

	public class Item : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private string _Name = string.Empty;

		public string Name
		{
			get
			{
				return this._Name;
			}

			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}

		private string _Firstname = string.Empty;

		public string Firstname
		{
			get
			{
				return this._Firstname;
			}

			set
			{
				if (this._Firstname != value)
				{
					this._Firstname = value;
					this.OnPropertyChanged("Firstname");
				}
			}
		}

		private double _NumberOfCommits = 0;

		public double NumberOfCommits
		{
			get
			{
				return this._NumberOfCommits;
			}

			set
			{
				if (this._NumberOfCommits != value)
				{
					this._NumberOfCommits = value;
					this.OnPropertyChanged("NumberOfCommits");
				}
			}
		}

		private string _ImageUrl = string.Empty;

		public string ImageUrl
		{
			get
			{
				return this._ImageUrl;
			}

			set
			{
				if (this._ImageUrl != value)
				{
					this._ImageUrl = value;
					this.OnPropertyChanged("ImageUrl");
				}
			}
		}
	}
#endif
}