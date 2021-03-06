﻿//      *********    DO NOT MODIFY THIS FILE     *********
//      This file is regenerated by a design tool. Making 
//      changes to this file can cause errors.
namespace Expression.Blend.SampleData.ChangesetsSampleDataSource
{
	using System;

	public class ChangesetsSampleDataSource : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public ChangesetsSampleDataSource()
		{
			try
			{
				System.Uri resourceUri = new System.Uri("/Smeedee.Widget.SourceControl.SL;component/SampleData/ChangesetsSampleDataSource/ChangesetsSampleDataSource.xaml", System.UriKind.Relative);
				if (System.Windows.Application.GetResourceStream(resourceUri) != null)
				{
					System.Windows.Application.LoadComponent(this, resourceUri);
				}
			}
			catch (System.Exception )
			{ }
		}

		private ItemCollection _Changesets = new ItemCollection();
		public ItemCollection Changesets
		{
			get
			{
				return this._Changesets;
			}
		}
	}

	public class ItemCollection : System.Collections.ObjectModel.ObservableCollection<Item>
	{ }

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

		private ComplexPropertyType _Developer = new ComplexPropertyType();
		public ComplexPropertyType Developer
		{
			get
			{
				return this._Developer;
			}
			set
			{
				if (this._Developer != value)
				{
					this._Developer = value;
					this.OnPropertyChanged("Developer");
				}
			}
		}

		private string _Message = string.Empty;
		public string Message
		{
			get
			{
				return this._Message;
			}
			set
			{
				if (this._Message != value)
				{
					this._Message = value;
					this.OnPropertyChanged("Message");
				}
			}
		}

		private string _Date = string.Empty;
		public string Date
		{
			get
			{
				return this._Date;
			}
			set
			{
				if (this._Date != value)
				{
					this._Date = value;
					this.OnPropertyChanged("Date");
				}
			}
		}
	}

	public class ComplexPropertyType : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private string _Email = string.Empty;
		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				if (this._Email != value)
				{
					this._Email = value;
					this.OnPropertyChanged("Email");
				}
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
}
