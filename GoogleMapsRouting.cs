using ArcGIS.Core.CIM;
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
using GoogleMapsRoutingAddIn.UI;

namespace GoogleMapsRoutingAddIn
{
	internal class GoogleMapsRouting : MapTool
    {
        string startX = "";
        string startY = "";
        uint id = 0;
        public GoogleMapsRouting()
        {
        }

        protected override Task OnToolActivateAsync(bool active)
        {
            return base.OnToolActivateAsync(active);
        }

        protected override void OnToolMouseDown(MapViewMouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left) {
                e.Handled = true;
            }
        }

        protected override async Task HandleMouseDownAsync(MapViewMouseButtonEventArgs e)
        {
            await QueuedTask.Run(() =>
            {
                MapPoint mapPoint = MapView.Active.ClientToMap(e.ClientPoint);
                MapPoint coords = (MapPoint)GeometryEngine.Instance.Project(mapPoint, SpatialReferences.WGS84);

                CultureInfo culture = new CultureInfo("en-US");
                if (startX == "" && startY == "")
                {
                    startX = coords.X.ToString(culture);
                    startY = coords.Y.ToString(culture);
                    ChromePaneViewModel._browserAddress = "";
                }
                else
                {
                    string endX = coords.X.ToString(culture);
                    string endY = coords.Y.ToString(culture);
                    ChromePaneViewModel._browserAddress = string.Format("https://www.google.com/maps/dir/{0},+{1}/{2},+{3}/", startY, startX, endY, endX);
                    startX = "";
                    startY = "";
                }
            });

            if (ChromePaneViewModel._browserAddress != "") {
                id = ChromePaneViewModel.Create(id).InstanceID;
            }
        }
	}
}
