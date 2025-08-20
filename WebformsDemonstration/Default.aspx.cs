using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

namespace WebformsDemonstration
{
    public partial class Default : System.Web.UI.Page
    {
        private string SortCol
        {
            get => ViewState["SortCol"] as string ?? "Name";
            set => ViewState["SortCol"] = value;
        }
        private string SortDir
        {
            get => ViewState["SortDir"] as string ?? "ASC";
            set => ViewState["SortDir"] = value;
        }

        // ----- Lifecycle demo
        protected void Page_Init(object sender, EventArgs e) { AppendLife("Init"); }

        protected void Page_Load(object sender, EventArgs e)
        {
            AppendLife("Load (IsPostBack=" + IsPostBack + ")");
            if (!IsPostBack)
            {
                // --- TEMP: verify we’re pointing at the correct physical SQLite file
                var cs = ConfigurationManager.ConnectionStrings["HotelDb"].ConnectionString;
                var builder = new SQLiteConnectionStringBuilder(cs);
                string logical = builder.DataSource;                              // e.g., |DataDirectory|\hotel.db
                string physical = Server.MapPath("~/App_Data/hotel.db");         // expected physical path
                bool exists = File.Exists(physical);
                long size = exists ? new FileInfo(physical).Length : 0;

                // show this once to help diagnose “file is not a database”
                lblDbStatus.Text = $"HotelDb -> {logical} => {physical} (exists={exists}, size={size} bytes)";

                // If you added SqlLite.EnsureCreated(), uncomment the next line to auto-create/seed the DB:
                // SqlLite.EnsureCreated();

                // quick DB smoke test + initial bind
                try
                {
                    var dt = SqlLite.Query("SELECT COUNT(1) AS Cnt FROM Hotels");
                    lblDbStatus.Text = $"DB OK — {dt.Rows[0]["Cnt"]} hotels found.";
                }
                catch (Exception ex)
                {
                    lblDbStatus.Text = "DB ERROR: " + ex.Message;
                }

                BindGrid();
            }

            gvHotels.RowCommand += gvHotels_RowCommand;
        }

        protected void Page_PreRender(object sender, EventArgs e) { AppendLife("PreRender"); }

        private void AppendLife(string msg)
        {
            lblLifecycle.Text += (lblLifecycle.Text.Length > 0 ? " → " : "") + msg;
        }

        // ----- Data binding
        private void BindGrid()
        {
            var dt = SqlLite.Query($"SELECT Id, Name, City, Rating FROM Hotels ORDER BY {SortCol} {SortDir}");
            gvHotels.DataSource = dt;
            gvHotels.DataBind();
        }

        protected void gvHotels_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvHotels.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvHotels_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (SortCol == e.SortExpression) SortDir = SortDir == "ASC" ? "DESC" : "ASC";
            else { SortCol = e.SortExpression; SortDir = "ASC"; }
            BindGrid();
        }

        // ----- Commands from GridView (Amenities + Add to Session)
        private void gvHotels_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Amenities")
            {
                var dt = SqlLite.Query("SELECT Name FROM Amenities WHERE HotelId=@id",
                    new SQLiteParameter("@id", id));

                rptAmenities.DataSource = dt;
                rptAmenities.DataBind();
                pnlAmenities.Visible = true;
                updMain.Update(); // partial update
            }
            else if (e.CommandName == "Add")
            {
                var dt = SqlLite.Query("SELECT Id, Name FROM Hotels WHERE Id=@id",
                    new SQLiteParameter("@id", id));

                var cart = Session["Cart"] as DataTable ?? dt.Clone();
                if (Session["Cart"] == null) Session["Cart"] = cart;

                foreach (DataRow r in dt.Rows) cart.ImportRow(r);

                lnkCart.Text = $"View Session Cart ({cart.Rows.Count})";
                updMain.Update();
            }
        }

        // ----- Add Hotel (AJAX) with validators
        protected void btnAddHotel_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            SqlLite.Execute(
                "INSERT INTO Hotels (Name, City, Rating) VALUES (@n,@c,@r)",
                new SQLiteParameter("@n", txtName.Text.Trim()),
                new SQLiteParameter("@c", txtCity.Text.Trim()),
                new SQLiteParameter("@r", int.Parse(txtRating.Text))
            );

            lblAddStatus.Text = "Saved!";
            txtName.Text = txtCity.Text = txtRating.Text = "";
            BindGrid(); // refresh via AJAX
        }
    }
}
