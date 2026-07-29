# Custom Command: Create a New Report

This command guides the creation of a new report module inside the `WebApp` project.

## Instructions
1. Prompt the user for:
   - Report Name (e.g., BaoCaoTuan)
   - Columns / Data fields to display
   - Stored Procedure or Table name for report data
2. Analyze the template controller: `WebApp/Controllers/BaoCaoThangController.cs` and view: `WebApp/Views/BaoCaoThang/Index.cshtml`.
3. Create the new MVC Controller: `WebApp/Controllers/{ReportName}Controller.cs`:
   - Inherit from `BaseController`.
   - Apply `[HasCredential(ControllerName = "{ReportName}")]` attribute.
   - Implement `Index()` returning the default view.
   - Implement `GetData()` to return JSON data for the report grid.
   - Implement `Download()` to export report data into an Excel spreadsheet using ClosedXML. Keep styles clean and reusable.
4. Create the Razor View directory: `WebApp/Views/{ReportName}/`.
5. Create the View file: `WebApp/Views/{ReportName}/Index.cshtml`:
   - Setup layout, title, Select2 filters (such as City, Project, Date range).
   - Configure DataTables to display grid items.
   - Add a download/export button referencing the controller's `Download` action.
6. Instruct the user to add the database credentials/menu config in the SQL DB for the new report.
