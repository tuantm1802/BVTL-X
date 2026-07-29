# Custom Command: Refactor Report Controller

This command helps reduce code duplication in report controllers by migrating manual ClosedXML Excel generation code to a shared helper.

## Instructions
1. Prompt the user for the target controller path (e.g., `WebApp/Controllers/BaoCaoThangController.cs`).
2. Analyze the Excel generation code inside the `Download()` or `ExportData()` method.
3. Identify styling and format boilerplates:
   - Font properties (New Times Roman, sizes)
   - Alignments and wrap text configurations
   - Borders and cell merging
   - Hardcoded header/footer coordinates
4. Refactor the code by leveraging the `Common/Report/ReportBuilder.cs` library (once created) or extract these setups into clean, localized private helper methods.
5. Goal: Reduce the controller's size to ~150 lines while retaining identical Excel format output.
6. Verify output consistency by comparing generated columns, headers, and footer details.
