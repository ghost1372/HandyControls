using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HandyControl.Controls
{
	public class GithubTimeLine : TreeView, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged(string propName)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		#region Property

		public OrderType OrderBy
		{
			get { return (OrderType)GetValue(OrderByProperty); }
			set { SetValue(OrderByProperty, value); }
		}

		public static readonly DependencyProperty OrderByProperty =
			DependencyProperty.Register("OrderBy", typeof(OrderType), typeof(GithubTimeLine), new PropertyMetadata(OrderType.DessendingTitleLabel, OnOrderByChanged));

		private static void OnOrderByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctl = (GithubTimeLine)d;
			ctl.OrderChanged((OrderType)e.NewValue);
		}

		#endregion

		public ObservableCollection<ContentMember> Members { get; set; }

		private string titleLabel;
		public string TitleLabel
		{
			get { return this.titleLabel; }
			set
			{
				if (this.titleLabel != value)
				{
					this.titleLabel = value;
					this.NotifyPropertyChanged("TitleLabel");
				}
			}
		}
		private string titleInfo;
		public string TitleInfo
		{
			get { return this.titleInfo; }
			set
			{
				if (this.titleInfo != value)
				{
					this.titleInfo = value;
					this.NotifyPropertyChanged("TitleInfo");
				}
			}
		}
		private Style titleStyle;
		public Style TitleStyle
		{
			get { return this.titleStyle; }
			set
			{
				if (this.titleStyle != value)
				{
					this.titleStyle = value;
					this.NotifyPropertyChanged("TitleStyle");
				}
			}
		}

		public GithubTimeLine()
		{
			this.Members = new ObservableCollection<ContentMember>();
		}

		public enum OrderType
		{
			AssendingTitleLabel,
			DessendingTitleLabel,
			AssendingTitleInfo,
			DessendingTitleInfo
		}
		internal void OrderChanged(OrderType OrderBy)
		{
			var sortedData = CollectionViewSource.GetDefaultView(ItemsSource);
			sortedData.SortDescriptions.Clear();
			switch (OrderBy)
			{
				case OrderType.AssendingTitleLabel:
					sortedData.SortDescriptions.Add(new SortDescription("TitleLabel", ListSortDirection.Ascending));

					break;
				case OrderType.DessendingTitleLabel:
					sortedData.SortDescriptions.Add(new SortDescription("TitleLabel", ListSortDirection.Descending));
					break;
				case OrderType.AssendingTitleInfo:
					sortedData.SortDescriptions.Add(new SortDescription("TitleInfo", ListSortDirection.Ascending));

					break;
				case OrderType.DessendingTitleInfo:
					sortedData.SortDescriptions.Add(new SortDescription("TitleInfo", ListSortDirection.Descending));
					break;
			}
		}
	}
	public class ContentMember : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged(string propName)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		private string contentTitle;
		public string ContentTitle
		{
			get { return this.contentTitle; }
			set
			{
				if (this.contentTitle != value)
				{
					this.contentTitle = value;
					this.NotifyPropertyChanged("ContentTitle");
				}
			}
		}
		private string contentInfo;
		public string ContentInfo
		{
			get { return this.contentInfo; }
			set
			{
				if (this.contentInfo != value)
				{
					this.contentInfo = value;
					this.NotifyPropertyChanged("ContentInfo");
				}
			}
		}
		private Style contentStyle;
		public Style ContentStyle
		{
			get { return this.contentStyle; }
			set
			{
				if (this.contentStyle != value)
				{
					this.contentStyle = value;
					this.NotifyPropertyChanged("ContentStyle");
				}
			}
		}
	}
}
