﻿using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

namespace GoogleMapsRoutingAddIn.UI
{
	internal class ChromePaneViewModel : ViewStatePane
	{
		private const string _viewPaneID = "GoogleMapsRoutingAddIn_UI_ChromePane";

		/// <summary>
		/// Consume the passed in CIMView. Call the base constructor to wire up the CIMView.
		/// </summary>
		public ChromePaneViewModel(CIMView view)
			: base(view) { }

		/// <summary>
		/// Create a new instance of the pane.
		/// </summary>
		internal static ChromePaneViewModel Create(uint instanceId)
		{
			var view = new CIMGenericView();
			view.ViewType = _viewPaneID;
			Pane pane = FrameworkApplication.Panes.FindPane(instanceId);
			if (pane != null) {
				pane.Close();
			}
			return FrameworkApplication.Panes.Create(_viewPaneID, new object[] { view }) as ChromePaneViewModel;
		}

		#region Properties
		// private static string _browserAddress = @"https://www.google.com/maps/dir/34.02598574976299,+-118.28419092661454/34.02177367883946,+-118.28277413741719/";

		public static string _browserAddress = "";

		public string BrowserAddress
		{
			get
			{
				return _browserAddress;
			}
			set
			{
				SetProperty(ref _browserAddress, value, () => BrowserAddress);
			}
		}

		private ICommand _openResourceCmd = null;
		private ICommand _openEmbeddedResourceCmd = null;
		private ICommand _openFileResourceCmd = null;

		public ICommand OpenResourceCmd
		{
			get
			{
				if (_openResourceCmd == null)
				{
					_openResourceCmd = new RelayCommand(new Action<Object>((sender) =>
					{
						var menu_item = sender as System.Windows.Controls.MenuItem;
						var text = menu_item.Header;
						var address = $@"resource:/{text}";
						this.BrowserAddress = address;
					}), () => { return true; });
				}
				return _openResourceCmd;
			}
		}

		public ICommand OpenEmbeddedResourceCmd
		{
			get
			{
				if (_openEmbeddedResourceCmd == null)
				{
					_openEmbeddedResourceCmd = new RelayCommand(new Action<Object>((sender) =>
					{
						var menu_item = sender as System.Windows.Controls.MenuItem;
						var text = menu_item.Header;
						var address = $@"embeddedresource:/{text}";
						this.BrowserAddress = address;
					}), () => { return true; });
				}
				return _openEmbeddedResourceCmd;
			}
		}

		public ICommand OpenFileResourceCmd
		{
			get
			{
				if (_openFileResourceCmd == null)
				{
					_openFileResourceCmd = new RelayCommand(new Action<Object>((sender) =>
					{
						var fileFilter = BrowseProjectFilter.GetFilter("esri_browseDialogFilters_browseFiles");
						fileFilter.FileExtension = "*.*";
						fileFilter.BrowsingFilesMode = true;

						var dlg = new OpenItemDialog()
						{
							BrowseFilter = fileFilter,
							Title = "Browse Content"
						};
						if (!dlg.ShowDialog().Value)
							return;
						var item = dlg.Items.First();
						//format the URL
						var address = $@"file:///{item.Path.Replace(@"\", @"/")}";
						this.BrowserAddress = address;
					}), () => { return true; });
				}
				return _openFileResourceCmd;
			}
		}

		#endregion

		#region Pane Overrides

		/// <summary>
		/// Must be overridden in child classes used to persist the state of the view to the CIM.
		/// </summary>
		public override CIMView ViewState
		{
			get
			{
				_cimView.InstanceID = (int)InstanceID;
				return _cimView;
			}
		}

		/// <summary>
		/// Called when the pane is initialized.
		/// </summary>
		protected async override Task InitializeAsync()
		{
			await base.InitializeAsync();
		}

		/// <summary>
		/// Called when the pane is uninitialized.
		/// </summary>
		protected async override Task UninitializeAsync()
		{
			await base.UninitializeAsync();
		}

		#endregion Pane Overrides
	}

}