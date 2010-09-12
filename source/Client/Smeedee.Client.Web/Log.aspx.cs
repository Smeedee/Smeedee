using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web
{
    public partial class Log : System.Web.UI.Page
    {
        private DataView dataView;
        protected void Page_Load(object sender, EventArgs e)
        {
            dataView = buildTable();
            LogGrid.DataSource = dataView; // dataSource.OrderByDescending((entry) => entry.TimeStamp);
            LogGrid.DataBind();
            LogGrid.Sorting += DoSort;

            DeleteAllButton.Click += (obj, evt) =>
            {
                new LogEntryDatabaseRepository().Delete(new AllSpecification<LogEntry>());
            };
        }

        protected void DoSort(object sender, GridViewSortEventArgs e)
        {
            dataView.Sort = e.SortExpression + " " + ToggleSortDirection();
            LogGrid.DataSource = dataView;
            LogGrid.DataBind();
        }

        protected string ToggleSortDirection()
        {
            ViewState["SortDir"] = ViewState["SortDir"] as String == "ASC" ? "DESC" : "ASC";
            return ViewState["SortDir"] as String;
        }

        private DataView buildTable()
        {
            var logEntries = new LogEntryDatabaseRepository().Get(new AllSpecification<LogEntry>());
            var table = new DataTable();
            var columns = new DataColumn[]
                              {
                                  new DataColumn("Message"),
                                  new DataColumn("Severity"),
                                  new DataColumn("Source"),
                                  new DataColumn("TimeStamp")
                              };
            table.Columns.AddRange(columns);
            foreach (LogEntry entry in logEntries)
            {
                DataRow row = table.NewRow();
                row[0] = entry.Message;
                row[1] = entry.Severity;
                row[2] = entry.Source;
                row[3] = entry.TimeStamp;
                table.Rows.Add(row);
            }
            var view = new DataView(table);
            view.Sort = "TimeStamp DESC";
            return view;
        }

        private void ReplaceNewLines(DataRow row)
        {
            for (int i = 0; i<row.ItemArray.Length; i++)
            {
                if (row[i] is string && row[i] != null)
                {
                    row[i] = (row[i] as string).Replace("\n", "<br>");
                }
            }
        }
    }
}