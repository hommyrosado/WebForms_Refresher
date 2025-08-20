<%@ Page Title="Hotels"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="WebformsDemonstration.Default" %>

<asp:Content ID="MainContentBlock" ContentPlaceHolderID="MainContent" runat="server">

    <!-- optional: DB status / messages -->
    <asp:Label ID="lblDbStatus" runat="server" CssClass="text-info" />
    <br />

    <!-- Lifecycle demo -->
    <asp:Label ID="lblLifecycle" runat="server" CssClass="text-muted" />
    <hr />

    <!-- Everything below updates asynchronously -->
    <asp:UpdatePanel ID="updMain" runat="server" UpdateMode="Conditional">
      <ContentTemplate>

        <!-- GRIDVIEW (paging + sorting) -->
        <h3 class="h4">Hotels</h3>
        <asp:GridView ID="gvHotels" runat="server" CssClass="table table-striped table-bordered"
            AutoGenerateColumns="False" AllowPaging="true" PageSize="5"
            AllowSorting="true" DataKeyNames="Id"
            OnPageIndexChanging="gvHotels_PageIndexChanging"
            OnSorting="gvHotels_Sorting">
          <Columns>
            <asp:BoundField DataField="Id" HeaderText="ID" SortExpression="Id" />
            <asp:BoundField DataField="Name" HeaderText="Hotel" SortExpression="Name" />
            <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" />
            <asp:BoundField DataField="Rating" HeaderText="Rating" SortExpression="Rating" />
            <asp:TemplateField HeaderText="Actions">
              <ItemTemplate>
                <asp:Button ID="btnAmenities" runat="server" Text="View Amenities"
                  CssClass="btn btn-sm btn-info"
                  CommandName="Amenities" CommandArgument='<%# Eval("Id") %>' />
                &nbsp;
                <asp:Button ID="btnAdd" runat="server" Text="Add to Session"
                  CssClass="btn btn-sm btn-primary"
                  CommandName="Add" CommandArgument='<%# Eval("Id") %>' />
              </ItemTemplate>
            </asp:TemplateField>
          </Columns>
        </asp:GridView>

        <!-- REPEATER: amenities for selected hotel -->
        <asp:Panel ID="pnlAmenities" runat="server" Visible="false" CssClass="mt-3">
          <h4 class="h5">Amenities</h4>
          <asp:Repeater ID="rptAmenities" runat="server">
            <HeaderTemplate><ul class="list-unstyled mb-2"></HeaderTemplate>
            <ItemTemplate><li>• <%# Eval("Name") %></li></ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
          </asp:Repeater>
        </asp:Panel>

        <hr />

        <!-- FORM WITH VALIDATION (AJAX save) -->
        <h4 class="h5">Add Hotel</h4>
        <asp:ValidationSummary ID="valSummary" runat="server" CssClass="text-danger" />
        <div class="row g-2">
          <div class="col-md-4">
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName"
              ErrorMessage="Name required" Display="Dynamic" CssClass="text-danger" />
          </div>
          <div class="col-md-4">
            <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" placeholder="City"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCity"
              ErrorMessage="City required" Display="Dynamic" CssClass="text-danger" />
          </div>
          <div class="col-md-2">
            <asp:TextBox ID="txtRating" runat="server" CssClass="form-control" placeholder="Rating 1–5"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRating"
              ErrorMessage="Rating required" Display="Dynamic" CssClass="text-danger" />
            <asp:RangeValidator runat="server" ControlToValidate="txtRating" Type="Integer"
              MinimumValue="1" MaximumValue="5" ErrorMessage="Rating must be 1–5"
              Display="Dynamic" CssClass="text-danger" />
          </div>
          <div class="col-md-2 d-grid">
            <asp:Button ID="btnAddHotel" runat="server" Text="Save (AJAX)"
              CssClass="btn btn-success" OnClick="btnAddHotel_Click" />
          </div>
        </div>
        <div class="mt-2">
          <asp:Label ID="lblAddStatus" runat="server" CssClass="text-success" />
        </div>

        <hr />

        <!-- Sessions quick link -->
        <asp:HyperLink ID="lnkCart" runat="server" NavigateUrl="~/Cart.aspx"
          Text="View Session Cart" CssClass="btn btn-outline-secondary" />

      </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Optional progress indicator for async postbacks -->
    <asp:UpdateProgress ID="prog" runat="server" AssociatedUpdatePanelID="updMain">
      <ProgressTemplate>
        <span class="text-muted">Loading…</span>
      </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>
